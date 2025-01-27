using UnityEngine;

namespace Isostopy.StepSystem.Types
{
	/// <summary>
	/// Step que reinicia la cadena de pasos. </summary>
	[AddComponentMenu("Isostopy/Step System/General/Restart Step")]
	public class RestartStep : Step
	{
		protected override void OnActivate()
		{
			End();
			chain.Play();
		}
	}
}
