using UnityEngine;

/// <summary> Teletrasporta un GameObject de una posicion a otra. </summary>
[AddComponentMenu("Isostopy/Step System/General/Teleport Step")]
public class TeleportStep : Step
{
    /// <summary> Objeto que va a teletransportarse. </summary>
    [Space] public Transform targetObject = null;
    /// <summary> Transform que determina la nueva posicion del objeto. </summary>
    public Transform newPosition = null;

	/// <summary> Posicion del objeto anterior al teletrasporte. </summary>
	Vector3 prevPos = Vector3.positiveInfinity;


	// ---------------------------------------------

	protected override void OnActivate()
	{
		prevPos = targetObject.position;
		targetObject.position = newPosition.position;

		End();
	}

	protected override void OnRestart()
	{
		if (prevPos != Vector3.positiveInfinity)
		{
			targetObject.position = prevPos;
		}
	}
}
