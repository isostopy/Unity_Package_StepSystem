using UnityEngine;

/// <summary>
/// Step que, simplemente, escribe un mensaje por consola. </summary>
[AddComponentMenu("Isostopy/Step System/General/Debug Step")]
public class DebugStep : Step
{
	/// <summary> Mensaje que se mostrara por consola. </summary>
	[Space][TextArea] public string logMessage = "Hello World!";

	protected override void OnActivate()
	{
		Debug.Log("[" + gameObject.name + "] " + logMessage, this);
		End();
	}
}
