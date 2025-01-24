using UnityEngine;

namespace Isostopy.StepSystem.Types
{
	/// <summary>
	/// Step que activa o desactiva un GameObject. </summary>
	[AddComponentMenu("Isostopy/Step System/General/Game Object Step")]
	public class GameObjectStep : Step
	{
		/// <summary> GameObject que se activa o desactiva. </summary>
		[Space] public GameObject gameObj = null;

		/// <summary> ¿Se va a activar o desactivar el GameObject? </summary>
		[Space][SerializeField] Behaviour behaviour = Behaviour.Activate;
		/// <summary> ¿Forzar que el GameObject empieze en el estado contrario a Behaviour? </summary>
		public bool forceInitialState = false;

		/// <summary> Enum definida para que elegir si se activa o desactiva el objeto sea bonito en el inspector. </summary>
		enum Behaviour { Activate, Deactivate }
		/// <summary> Estado del GameObject antes de que este Step lo modificase. </summary>
		bool prevState;


		// ------------------------------------------------------

		void Start()
		{
			// Iniciar en el estado contrario si esta indicado.
			if (forceInitialState)
				gameObj.SetActive(!targetState);
			prevState = gameObj.activeSelf;
		}

		protected override void OnActivate()
		{
			if (gameObj != null)
			{
				prevState = gameObj.activeSelf;
				gameObj.SetActive(targetState);
			}

			End();
		}

		protected override void OnRestart()
		{
			if (gameObj != null)
				gameObj.SetActive(prevState);
		}


		// ------------------------------------------------------

		/// <summary> Estado al que vamos a poner el GameObject. </summary>
		public bool targetState
		{
			get
			{
				if (behaviour == Behaviour.Activate)
					return true;
				return false;
			}

			set
			{
				if (value == true)
					behaviour = Behaviour.Activate;
				else
					behaviour = Behaviour.Deactivate;
			}
		}
	}
}
