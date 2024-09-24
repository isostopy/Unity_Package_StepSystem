using System.Collections;
using UnityEngine;

/// <summary>
/// Step que espera un tiempo antes de terminar y pasar el siguiente. </summary>
[AddComponentMenu("Isostopy/Step System/Wait/Wait Step")]
public class WaitStep : Step
{
	/// <summary> Tiempo a esperar antes de pasar al siguiente Step. </summary>
	[Space][Min(0)] public float time = 1;
	/// <summary> Corrutina actualmente activa. </summary>
	Coroutine routine = null;


	// ------------------------------------------------------

	protected override void OnActivate()
	{
		StopWaitingRoutine();
		routine = StartCoroutine(WaitingRouting());
	}

	protected override void OnEnd()
	{
		StopWaitingRoutine();
	}

	protected override void OnRestart()
	{
		StopWaitingRoutine();
	}


	// ------------------------------------------------------

	IEnumerator WaitingRouting()
	{
		yield return new WaitForSeconds(time);
		End();
	}

	void StopWaitingRoutine()
	{
		if (routine != null) StopCoroutine(routine);
	}
}
