using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Isostopy.StepSystem
{
	/// <summary>
	/// Ejecuta una cadena de pasos. </summary>
	/// Buscando entre los hijos del GameObject cualquier componente Step que aparezca, para ir activandolos en el orden en que aparecen en la jerarquia.

	[AddComponentMenu("Isostopy/Step System/Step Chain")]
	public class StepChain : MonoBehaviour
	{
		/// <summary> Lista de Steps en el orden en el que aparecen en la jerarquia. </summary>
		private List<Step> stepList = new List<Step>();
		/// <summary> Indice del Step que esta actualmente activo. </summary>
		[SerializeField][HideInInspector] private int currentStepIndex = -1;
		/// <summary> Paso que esta actualmente activo. </summary>
		private Step currentStep
		{
			get
			{
				if (currentStepIndex < 0 || currentStepIndex >= stepList.Count)
					return null;
				return stepList[currentStepIndex];
			}
		}

		/// Si tiene o no que iniciar automaticamente la cadena de Steps.
		[Space][SerializeField] private bool playOnStart = true;
		/// Si se esta ejecutando la cadena de pasos.
		public bool playing { get; private set; }


		// ------------------------------------------------------
		#region Start

		private void Awake()
		{
			// Encontrar todos los pasos en los hijos de este objeto.
			Step[] stepComponents = GetComponentsInChildren<Step>(true);
			foreach (var step in stepComponents)
			{
				stepList.Add(step);
				step.SetChain(this);
			}
		}

		IEnumerator Start()
		{
			AddCustomHierarchyListners();

			// Espera un frame para que todos los Steps hayan ejecutado su Start, y empezar la cadena.
			if (playOnStart)
			{
				yield return new WaitForEndOfFrame();
				Play();
			}
		}

		private void OnDisable()
		{
			Restart();
		}

		#endregion


		// ------------------------------------------------------
		#region Step Management

		/// <summary>
		/// Notifica a esta cadena que un Step ha terminado. </summary>
		public void StepEnded(Step step)
		{
			if (playing == false)
				return;
			if (step != currentStep)
				return;

			// Activar el siguiente paso si quedan en la lista.
			currentStepIndex++;
			if (currentStepIndex >= stepList.Count)
			{
				playing = false;
				currentStepIndex = -1;
				return;
			}
			currentStep.Activate();
		}

		/// <summary>
		/// Inicia la ejecuccion de la cadena de Steps desde el primero. </summary>
		public void Play()
		{
			if (stepList.Count == 0)
				return;

			if (currentStepIndex >= 0)
				Restart();

			playing = true;
			currentStepIndex = 0;
			stepList[currentStepIndex].Activate();

			RedrawHierarchy();
		}

		/// <summary>
		/// Devuelve la cadena de Steps a su estado inicial, parandola si esta en marcha. </summary>
		public void Restart()
		{
			int i = currentStepIndex;        // Restear de atras alante desde currentStep.
			if (i >= stepList.Count) i = stepList.Count - 1;

			while (i >= 0)
			{
				stepList[i].Restart();
				i--;
			}

			playing = false;
			currentStepIndex = -1;

			RedrawHierarchy();
		}

		/// <summary>
		/// Ir al Step objetivo. Asegurando de dejar terminados los pasos anteriores y reseteados los posteriores. <para></para>
		/// [ ! ] - Esto no esta probado y es posible que no funcione correctamente. </summary>
		public void GoToStep(Step step)
		{
			// Error si este manager no tiene el paso indicado.
			if (!stepList.Contains(step))
			{
				Debug.LogError("El step manager (" + transform.name + ") no contiene el step (" + step.transform.name + ").");
				return;
			}

			// Si el paso indicado es el actual, dejarlo como esta.
			if (step == stepList[currentStepIndex])
				return;

			int i = currentStepIndex;
			currentStepIndex = stepList.IndexOf(step);
			// Si el objetivo esta por encima del actual, ir activando y terminando todos hasta llegar al nuestro.
			if (i < currentStepIndex)
			{
				stepList[i].End();
				i++;

				while (i < currentStepIndex)
				{
					stepList[i].Activate();
					stepList[i].End();
					i++;
				}
			}
			// Si esta por debajo, ir reseteandolos.
			else
			{
				while (i > currentStepIndex)
				{
					stepList[i].Restart();
					i--;
				}
			}

			// Activar el nuevo paso actual.
			stepList[currentStepIndex].Activate();
			RedrawHierarchy();
		}

		#endregion


		// ------------------------------------------------------
		#region Custom Hierarchy

		/* La idea es que se vaya resaltando el Step que esta activo en la jerarquia para que sepamos por donde va la ejecucion.
		 * Pero las cosas del editor (using UnityEditor) no se incluyen en las builds finales,
		 * por lo que usarlas en un script normal como este hara que el juego final de un error.
		 * Para evitarlo, usamos las directivas #if y #endif. Que nos permite excluir codigo de la compilacion. */

		/// <summary>
		/// Añade listeners a los eventos del editor que necesitamos para personalizar el aspecto de la ventana de Hierarchy. </summary>
		void AddCustomHierarchyListners()
		{
#if UNITY_EDITOR
			{
				// Añadimos un listner al evento de redibujado de la ventana Hierarchy.
				EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchyItem;
				// Añadimos un listener al evento de cambio de estado del editor entre PlayMode y EditMode.
				EditorApplication.playModeStateChanged += OnPlayModeChange;
			}
#endif
		}

		/// <summary> Fuerza que se redibuje la ventana de Hierarchy en el editor. </summary>
		void RedrawHierarchy()
		{
#if UNITY_EDITOR
			EditorApplication.RepaintHierarchyWindow();
#endif
		}


#if UNITY_EDITOR

		/// A esta funcion se llama al salir y entrar del modo Play.
		void OnPlayModeChange(PlayModeStateChange state)
		{
			///	Al salir del PlayMode, borramos los listeners porque lo que queremos es pintar diferente
			///	el GameObject del Step que esta activo, y solo pueden estar activos en PlayMode.

			if (state != PlayModeStateChange.ExitingPlayMode)
				return;

			EditorApplication.hierarchyWindowItemOnGUI -= OnDrawHierarchyItem;
			EditorApplication.playModeStateChanged -= OnPlayModeChange;

			RedrawHierarchy();
		}

		/// A esta funcion se la llama por cada GameObject visible en la ventana de Hierarchy.
		void OnDrawHierarchyItem(int instanceId, Rect selectionRect)
		{
			/// InstanceId nos dice por que objeto ha sido llamada la funcion. Pero hay que convertirlo a GameObject.
			/// SelectoinRect es el especio en la ventana de la jerarquia que ocupa la label de este objeto.

			if (playing == false)
				return;

			/// GameObject del elemento de la jerarquia por el que se ha llamado esta función.
			GameObject hierarchyItem = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			if (hierarchyItem == null)
				return;
			/// GameObject del currentStep.
			GameObject activeStep = stepList[currentStepIndex].gameObject;
			/// Color amarillo.
			Color yellow = Color.yellow;
			/// Color del editor de Unity en modo oscuro. El claro es (194, 194, 194, 255).
			Color defaultBackground = new Color32(56, 56, 56, 255);

			// Pintar el Step activo.
			if (activeStep == hierarchyItem)
				DrawLabel(hierarchyItem.name, selectionRect, yellow, Color.black, FontStyle.BoldAndItalic);
			// Pintar los padres del Step activo.
			//else if (activeStep.transform.IsChildOf(hierarchyItem.transform))
			//	DrawLabel(hierarchyItem.name, selectionRect, yellow, Color.black, FontStyle.Normal);
			// Pintar los steps por los que ya hemos pasado.
			//else if (hierarchyItem.activeSelf && hierarchyItem.activeInHierarchy)
			//{
			//	int objStepIndex = stepList.IndexOf( hierarchyItem.GetComponent<Step>() );
			//	if (objStepIndex >= 0 && objStepIndex <= currentStep)
			//		DrawLabel(hierarchyItem.name, selectionRect, defaultBackground, yellow, FontStyle.Normal);
			//}
		}

		void DrawLabel(string name, Rect rect, Color backgroundColor, Color textColor, FontStyle fontStyle)
		{
			// Dibujar el rectangulo de fondo con el color indicado.
			EditorGUI.DrawRect(rect, backgroundColor);
			// Y el nombre en negrita del color indicado.
			EditorGUI.LabelField(rect, name, new GUIStyle()
			{
				normal = new GUIStyleState() { textColor = textColor },
				fontStyle = fontStyle
			});
		}

#endif
		#endregion
	}
}
