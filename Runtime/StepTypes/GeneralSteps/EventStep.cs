using UnityEngine;
using UnityEngine.Events;

namespace Isostopy.StepSystem.Types
{
	/// <summary> Paso que invoca un evento de unity al activar, terminar o reiniciarse. </summary>
	[AddComponentMenu("Isostopy/Step System/General/Event Step")]
	public class EventStep : Step
	{
		/// <summary> Si se tiene o no que iniciar el siguiente paso inmediatamente despues de activar este. </summary>
		/// Si no, tendra que llamarse a la funcion End() desde fuera de este componente.
		[Space] public bool endImmediately = true;

		/// <summary> Evento que se invoca al activar este paso. </summary>
		[Space] public UnityEvent onActivate = new UnityEvent();
		/// <summary> Evento que se incova al terminar este paso. </summary>
		public UnityEvent onEnd = new UnityEvent();
		/// <summary> Evento que se invoca al resetear este paso. </summary>
		public UnityEvent onRestart = new UnityEvent();


		// ---------------------------------------------

		protected override void OnActivate()
		{
			onActivate.Invoke();
			if (endImmediately)
				End();
		}

		protected override void OnEnd()
		{
			onEnd.Invoke();
		}

		protected override void OnRestart()
		{
			onRestart.Invoke();
		}
	}
}
