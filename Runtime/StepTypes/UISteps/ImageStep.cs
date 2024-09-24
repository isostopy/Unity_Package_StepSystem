using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Step que cambia el sprite de una imagen de la UI.</summary>
[AddComponentMenu("Isostopy/Step System/UI/Image Step")]
public class ImageStep : Step
{
	/// <summary> Imagen a la que vamos cambiar el sprite. </summary>
	[Space] public Image targetImage = null;

	/// <summary> Sprite que vamos a poner. </summary>
	public Sprite newSprite = null;
	/// <summary> Sprite que tenia antes de modificarlo. </summary>
	Sprite prevSprite = null;


	// ------------------------------------------------------

	void Start()
	{
		prevSprite = targetImage.sprite;
	}

	protected override void OnActivate()
	{
		prevSprite = targetImage.sprite;
		targetImage.sprite = newSprite;

		End();
	}

	protected override void OnRestart()
	{
		targetImage.sprite = prevSprite;
	}
}
