using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Isostopy.StepSystem
{
	/// <summary>
	/// Controla la cadena de pasos en la escena. </summary>
	/// Buscando entre los hijos del GameObject cualquier componente Step que aparezca, para ir activandolos en el orden en que aparecen en la jerarquia.

	[AddComponentMenu("Isostopy/Step System/Step Chain")]
	public class StepChain : MonoBehaviour
	{
		/// <summary> Lista de Steps en el orden en el que aparecen en la jerarquia. </summary>
		List<Step> stepList = new List<Step>();
		/// <summary> Indice del evento que esta actualmente activo. </summary>
		[SerializeField][HideInInspector] int currentStep = 0;

		/// <summary> Si tiene o no que iniciar automaticamente la cadena de Steps. </summary>
		[Space][SerializeField] bool playOnStart = true;


		// ------------------------------------------------------
		#region Start

		IEnumerator Start()
		{
			AddCustomHierarchyListners();
			FindStepsIn(transform);

			if (playOnStart)
			{
				// Espera un frame para que todos los Steps hayan ejecutado su Start. Y empezar la cadena de eventos.
				yield return new WaitForEndOfFrame();
				Play();
			}
		}

		private void OnDisable()
		{
			Cancel();
		}

		#endregion


		// ------------------------------------------------------
		#region StepList

		/// <summary>
		/// Encuentra y añade a la lista todos los Step que aparecen en los hijos del transform indicado. </summary>
		void FindStepsIn(Transform parent)
		{
			Transform child;
			Step[] stepComponents;

			for (int i = 0; i < parent.childCount; i++)
			{
				child = parent.GetChild(i);
				stepComponents = child.GetComponents<Step>();

				foreach (Step step in stepComponents)
					AddStep(step);

				// Si alguno de los hijos tiene hijos tambien, buscar Steps entre ellos.
				if (child.childCount > 0)
					FindStepsIn(child);         /// funcion recursiva
			}
		}

		/// <summary>
		/// ¿Esta en indice indicado entre los limites de la lista de Steps? </summary>
		bool IsValidStepIndex(int index)
		{
			if (stepList == null) return false;
			if (index < 0 || index >= stepList.Count)
				return false;
			return true;
		}

		/// <summary> Elimina un Step de la cadena de ejecucion. </summary>
		public void DeleteStep(Step step)
		{
			if (stepList.Contains(step))
				stepList.Remove(step);
		}

		/// <summary> Añade un Step a la cadena de ejecucion. </summary>
		public void AddStep(Step step, int index = -1)
		{
			if (stepList.Contains(step))
				return;

			if (IsValidStepIndex(index))
				stepList.Insert(index, step);
			else
				stepList.Add(step);
			step.SetChain(this);
		}

		#endregion


		// ------------------------------------------------------
		#region Step Management

		/// <summary>
		/// Notifica a este manager que un Step ha terminado. </summary>
		public void StepEnded(Step step)
		{
			// Activar el siguiente si el que ha terminado es el currentStep.
			if (!IsValidStepIndex(currentStep) || stepList[currentStep] != step)
				return;

			// Pasar al siguiente paso que no sea null. (Para que no de error si se destruye un Step).
			do
			{
				currentStep++;
				RedrawHierarchy();
			} while (IsValidStepIndex(currentStep) && stepList[currentStep] == null);

			// Activar el siguiente paso.
			if (!IsValidStepIndex(currentStep))
				return;
			stepList[currentStep].Activate();
		}

		/// <summary> Inicia o reanuda la ejecuccion de la cadena de Steps. </summary>
		public void Play()
		{
			if (stepList.Count == 0)
				return;

			/// Si indice del paso actual esta fuera de los limites de la lista por lo que sea,
			/// reiniciarla la cadena desde el prinicpio.
			if (IsValidStepIndex(currentStep))
				stepList[currentStep].Activate();
			else
				Restart();

			RedrawHierarchy();
		}

		/// <summary>
		/// Para la cadena de Steps, devolviendola a su estado inicial
		/// lista para activarla otra vez desde el inicio. </summary>
		public void Cancel()
		{
			int i = currentStep;        /// Restear de atras alante desde currentStep.
			if (i >= stepList.Count) i = stepList.Count - 1;

			currentStep = -1;
			while (i >= 0)
			{
				stepList[i].Restart();
				i--;
			}
			currentStep = 0;

			RedrawHierarchy();
		}

		/// <summary> Reinicia la cadena de Steps desde el principio. </summary>
		public void Restart()
		{
			Cancel();
			Play();
		}

		/// <summary>
		/// Ir al Step objetivo. Asegurando de dejar terminados los pasos anteriores y reseteados los posteriores. </summary>
		public void GoToStep(Step step)
		{
			// Error si este manager no tiene el paso indicado.
			if (!stepList.Contains(step))
			{
				Debug.LogError("El step manager (" + transform.name + ") no contiene el step (" + step.transform.name + ").");
				return;
			}

			// Si el paso indicado es el actual, dejarlo como esta.
			if (step == stepList[currentStep])
				return;

			int i = currentStep;
			currentStep = stepList.IndexOf(step);
			// Si el objetivo esta por encima del actual, ir activando y terminando todos hasta llegar al nuestro.
			if (i < currentStep)
			{
				stepList[i].End();
				i++;

				while (i < currentStep)
				{
					stepList[i].Activate();
					stepList[i].End();
					i++;
				}
			}
			// Si esta por debajo, ir reiniciandolos.
			else
			{
				while (i > currentStep)
				{
					stepList[i].Restart();
					i--;
				}
			}

			// Activar el nuevo paso actual.
			stepList[currentStep].Activate();
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

			/// GameObject del elemento de la jerarquia por el que se ha llamado esta función.
			GameObject hierarchyItem = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			if (hierarchyItem == null || !IsValidStepIndex(currentStep))
				return;
			/// GameObject del currentStep.
			GameObject activeStep = stepList[currentStep].gameObject;
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
