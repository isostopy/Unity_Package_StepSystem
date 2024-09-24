using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Step que cambia el color de un elemento de la UI.</summary>
[AddComponentMenu("Isostopy/Step System/UI/Color Step")]
public class ColorStep : Step
{
	/// <summary> Elemento de la UI que va a cambiar de color. </summary>
	[Space] public Graphic targetGraphic = null;

	/// <summary> Color que vamos a ponerle. </summary>
	public Color newColor = Color.yellow;
	/// <summary> Color que tenia antes de modificarlo. </summary>
	Color prevColor = Color.white;


	// ------------------------------------------------------

	protected virtual void Start()
	{
		prevColor = targetGraphic.color;
	}
	
	protected override void OnActivate()
	{
		prevColor = targetGraphic.color;
		targetGraphic.color = newColor;
		End();
	}

	protected override void OnRestart()
	{
		targetGraphic.color = prevColor;
	}
}
