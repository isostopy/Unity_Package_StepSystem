using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Step que espera a que se pulse una tecla para pasar el siguiente. </summary>
[AddComponentMenu("Isostopy/Step System/Wait/Wait For Input Step")]
public class WaitForInputStep : Step
{
	/// <summary> Tecla que permite pasar al siguiente Step. </summary>
	[Space] public List<KeyCode> keys = new List<KeyCode>() { KeyCode.Space };


	// ------------------------------------------------------

	protected override void OnActivate()
	{
		StopWaitingRoutine();
		routine = StartCoroutine(WaitingRoutine());
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

	Coroutine routine = null;
	IEnumerator WaitingRoutine()
	{
		while (true)
		{
			foreach (KeyCode key in keys)
			{
				if (Input.GetKeyDown(key))  /// Si se pulsa alguna de la teclas indicadas, parar los dos bucles.
				{                           /// See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/jump-statements
					yield return new WaitForEndOfFrame();
					goto End;               
				}
			}
			yield return null;
		}

		End: End();
	}

	void StopWaitingRoutine()
	{
		if (routine != null) StopCoroutine(routine);
	}
}
