using UnityEngine;
using UnityEngine.UI;

namespace Isostopy.StepSystem.Types
{
	/// <summary>
	/// Step que modifica un texto de la UI.</summary>
	[AddComponentMenu("Isostopy/Step System/UI/Text Step")]
	public class TextStep : Step
	{
		/// <summary> El texto de la UI que vamos a modificar. </summary>
		[Space] public Text targetText = null;

		/// <summary> El string que va a escribirse en ese texto. </summary>
		[TextArea] public string newText = "";
		/// <summary> El string que habia escrito antes de modificarlo. </summary>
		string oldText = null;

		/// <summary> ¿Añadir el nuevo texto al que ya hay? </summary>
		[Space] public bool additive = false;


		// ------------------------------------------------------

		protected override void OnActivate()
		{
			if (targetText != null)
			{
				oldText = targetText.text;
				if (additive)
					targetText.text += newText;
				else
					targetText.text = newText;
			}

			End();
		}

		protected override void OnRestart()
		{
			if (targetText == null || oldText == null)
				return;

			targetText.text = oldText;
			oldText = null;
		}
	}
}
