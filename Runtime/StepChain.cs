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
		}

		#endregion
	}
}
