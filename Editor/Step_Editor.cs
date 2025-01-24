using UnityEngine;
using UnityEditor;

namespace Isostopy.StepSystem.Editor
{
	/// Editor personalizado para los componentes Step.
	/// Si el Step esta activo, dibuja un rectangulo amarillo en la parte superior del componente para indicarlo.

	[CustomEditor(typeof(Step), true)]
	[CanEditMultipleObjects]
	class Step_Editor : UnityEditor.Editor
	{
		Step thisStep;

		void OnEnable()
		{
			thisStep = target as Step;
		}

		public override void OnInspectorGUI()
		{
			if (thisStep.active)
			{
				Rect rect = EditorGUILayout.GetControlRect();
				EditorGUI.DrawRect(rect, Color.yellow);
				EditorGUI.LabelField(rect, "   Active Step!", new GUIStyle()
				{
					normal = new GUIStyleState() { textColor = Color.black },
					fontStyle = FontStyle.Bold
				});

				/// Asegura que Unity redibuje la ventana del inspector.
				EditorUtility.SetDirty(target);
			}

			DrawDefaultInspector();
		}
	}
}
