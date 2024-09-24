using UnityEngine;

using UnityEditor;

/// <summary> Step que activa una animacion. </summary>
[AddComponentMenu("Isostopy/Step System/General/Animation Step")]
public class AnimationStep : Step
{
	/// <summary> Animator del que vamos a activar una animacion. </summary>
    [Space][SerializeField] public Animator animator = null;
	/// <summary> El nombre de parametro que vamos a cambiar del animator. </summary>
	[Space][SerializeField] public string parameter = "";
	/// <summary> El tipo de parametro que vamos a activar del animator. </summary>
	[SerializeField] public AnimatorControllerParameterType type = AnimatorControllerParameterType.Trigger;

	/// Valores objetivo a los que poner el parametro indicado arriba.
	[SerializeField] public float floatValue = 0;
	[SerializeField] public int intValue = 0;
	[SerializeField] public bool boolValue = true;
	/// Valores del paremetro antes de que los modifiquemos.
	float prevFloat;
	int prevInt;
	bool prevBool;


	// ------------------------------------------------------

	private void Start()
	{
		// Guardar valor inicial del parametro.
		switch (type)
		{
			case AnimatorControllerParameterType.Float:
				prevFloat = animator.GetFloat(parameter);
				break;

			case AnimatorControllerParameterType.Int:
				prevInt = animator.GetInteger(parameter);
				break;

			case AnimatorControllerParameterType.Bool:
				prevBool = animator.GetBool(parameter);
				break;
		}
	}

	protected override void OnActivate()
	{
		// Guardar el valor previo del parametro. Y cambiar el del animator.
		switch (type)
		{
			case AnimatorControllerParameterType.Float:
				prevFloat = animator.GetFloat(parameter);
				animator.SetFloat(parameter, floatValue);
				break;

			case AnimatorControllerParameterType.Int:
				prevInt = animator.GetInteger(parameter);
				animator.SetInteger(parameter, intValue);
				break;

			case AnimatorControllerParameterType.Bool:
				prevBool = animator.GetBool(parameter);
				animator.SetBool(parameter, boolValue);
				break;

			case AnimatorControllerParameterType.Trigger:
				animator.SetTrigger(parameter);
				break;
		}

		End();
	}

	protected override void OnRestart()
	{
		// Devolver los valores del parametro a los anteriores al cambio.
		switch (type)
		{
			case AnimatorControllerParameterType.Float:
				animator.SetFloat(parameter, prevFloat);
				break;

			case AnimatorControllerParameterType.Int:
				animator.SetInteger(parameter, prevInt);
				break;

			case AnimatorControllerParameterType.Bool:
				animator.SetBool(parameter, prevBool);
				break;
		}
	}
}


// ------------------------------------------------------------------------------------------------------------
#region CustomInspector
#if UNITY_EDITOR

/* Custom inspoctor que dibuja solo el valor del parametro elegido en la enum. */

[CustomEditor(typeof(AnimationStep))]
[CanEditMultipleObjects]
class AnimationStep_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DrawProperty("animator");
		DrawProperty("parameter");

		int type = DrawProperty("type").enumValueIndex;
		switch (type)
		{
			case 0:
				DrawProperty("floatValue");
				break;
			case 1:
				DrawProperty("intValue");
				break;
			case 2:
				DrawProperty("boolValue");
				break;
			default:
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}

	SerializedProperty DrawProperty(string name)
	{
		SerializedProperty property = serializedObject.FindProperty(name);
		EditorGUILayout.PropertyField(property);

		return property;
	}
}

#endif
#endregion
