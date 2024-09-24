using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Step que espera a que se pulse un boton de la UI para pasar al siguiente.</summary>
[AddComponentMenu("Isostopy/Step System/Wait/Wait For Buttom Step")]
public class WaitForButtomStep : Step
{
	/// <summary> Lista de botones de la UI que terminaran la espera. </summary>
	[Space] public List<Button> buttons = new List<Button>() { null };


	// ------------------------------------------------------

	protected override void OnActivate()
	{
		SetAllButtons(true);
	}

	protected override void OnEnd()
	{
		SetAllButtons(false);
	}

	protected override void OnRestart()
	{
		SetAllButtons(false);
	}


	// ------------------------------------------------------

	/// Añade o quita todos los listener de los botones.
	void SetAllButtons(bool value)
	{
		if (value)
		{
			foreach (Button but in buttons)
				but.onClick.AddListener(OnButtonClick);
		}
		else
		{
			foreach (Button but in buttons)
				but.onClick.RemoveListener(OnButtonClick);
		}
	}

	void OnButtonClick()
	{
		End();
	}
}
