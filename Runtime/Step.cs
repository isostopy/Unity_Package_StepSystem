using UnityEngine;

namespace Isostopy.StepSystem
{
	/// <summary>
	/// Clase basica para que hereden todos los Steps. </summary>
	public abstract class Step : MonoBehaviour
	{
		/// <summary> Referencia al manager que controla este Step. </summary>
		protected StepChain chain { get; set; } = null;
		/// <summary> TRUE mientras este step esta activo. </summary>
		private bool _active = false;


		// ------------------------------------------------------------------------------------------------------------
		#region Step Functions

		/*	Cada funcion tiene 2 partes, una public que es llamada desde otras clases y componentes.		*
		 *	Y una "protected virtual" para que los diferentes steps sobreesicriban haciendo lo que quieran.	*/

		// ------------------------------------------------------
		#region Activate

		/// <summary>
		/// Activa este Step. </summary>
		public void Activate()
		{
			// Saltarse este paso si el GameObject o el componente esta desactivado.
			if (!enabled || !gameObject.activeSelf || !gameObject.activeInHierarchy)
			{
				if (chain != null) chain.StepEnded(this);
				return;
			}

			_active = true;
			OnActivate();
		}

		/// <summary> ActivateStep() es llamada al iniciar este paso. </summary>
		protected virtual void OnActivate() { }

		#endregion

		// ------------------------------------------------------
		#region End

		/// <summary>
		/// Termina este Step. Informando al manager de que ha terminado. </summary>
		public void End()
		{
			if (this._active == false)
				return;

			OnEnd();

			_active = false;
			if (chain != null)
				chain.StepEnded(this);
		}

		/// <summary> EndStep() es llamada al terminar este paso. </summary>
		protected virtual void OnEnd() { }

		#endregion

		// ------------------------------------------------------
		#region Restart
		/// La funcion no se puede llamar Reset porque ya hay una que se llama asi en MonoBehaviour.

		/// <summary>
		/// Devuelve el Step a su estado inicial, listo para volver a llamar al Activate. </summary>
		public void Restart()
		{
			if (_active == true)
				End();
			_active = false;

			OnRestart();
		}

		/// <summary> RestartStep() es llamada para devolver este paso a su estado inicial. </summary>
		protected virtual void OnRestart() { }

		#endregion

		#endregion


		// ------------------------------------------------------------------------------------------------------------
		#region Public

		/// <summary> Establece el manager de este Step. </summary>
		public void SetChain(StepChain chain)
		{
			if (this.chain != null)
				return;
			this.chain = chain;
		}

		/// <summary> ¿Esta este Step activo? </summary>
		public bool active
		{
			get { return _active; }
		}

		#endregion


		// ------------------------------------------------------
		#region On Disable

		// Esto hace falta porque Unity llama OnDisable al salir del Play en el editor, y eso da error en algunos pasos.
		bool _quiting = false;
		private void OnApplicationQuit() => _quiting = true;

		void OnDisable()
		{
			if (_quiting)
				return;
			// Al desactivar este componente o GameObject, si este paso estaba activado, terminarlo.
			if (chain != null && _active) End();
		}

		#endregion
	}
}
