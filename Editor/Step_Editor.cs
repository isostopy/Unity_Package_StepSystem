using UnityEngine;
using UnityEditor;

namespace Isostopy.StepSystem.Editor
{
	/// Si el Step esta activo, dibuja un rectangulo amarillo en la parte superior del componente para indicarlo.

	[CustomEditor(typeof(Step), true)]
	[CanEditMultipleObjects]
	class Step_Editor : UnityEditor.Editor
	{
		Step thisStep;

		static GUIStyle labelStyle = null;


		// -------------------------------

		void OnEnable()
		{
			thisStep = target as Step;

			labelStyle = new GUIStyle()
			{
				normal = new GUIStyleState() { textColor = Color.black },
				fontStyle = FontStyle.Bold
			};
		}

		public override void OnInspectorGUI()
		{
			if (thisStep.active)
			{
				Rect rect = EditorGUILayout.GetControlRect();
				EditorGUI.DrawRect(rect, Color.yellow);
				EditorGUI.LabelField(rect, "   Active Step!", labelStyle);

				/// Asegura que Unity redibuje la ventana del inspector.
				EditorUtility.SetDirty(target);
			}

			DrawDefaultInspector();
		}
	}
}
