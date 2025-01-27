using UnityEngine;
using UnityEditor;

namespace Isostopy.StepSystem.Editor
{
	/// Resalta en la jerarquia el game object del objeto que esta activo en una cadena de pasos.

	[InitializeOnLoad]
	public static class StepChain_CustomHierarchy
	{
		static Color backgroundColor = Color.yellow;
		static GUIStyle labelStyle = new GUIStyle()
		{
			normal = new GUIStyleState() { textColor = Color.black },
			fontStyle = FontStyle.BoldAndItalic
		};


		// -------------------------------

		static StepChain_CustomHierarchy()
		{
			// Añadimos un listner al evento de redibujado de la ventana Hierarchy.
			EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchyItem;
		}

		/// A esta funcion se la llama por cada GameObject visible en la ventana de Hierarchy.
		static void OnDrawHierarchyItem(int instanceId, Rect selectionRect)
		{
			/// InstanceId nos dice por que objeto ha sido llamada la funcion. Pero hay que convertirlo a GameObject.
			/// SelectoinRect es el especio en la ventana de la jerarquia que ocupa la label de este objeto.

			GameObject hierarchyItem = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			if (hierarchyItem == null)
				return;
			Step[] steps = hierarchyItem.GetComponents<Step>();
			if (steps.Length == 0)
				return;

			// Pintar si hay un step activo.
			foreach(Step step in steps)
			{
				if (step.active == true)
				{
					DrawLabel(hierarchyItem.name, selectionRect);
					return;
				}
			}
		}

		static void DrawLabel(string name, Rect rect)
		{ 
			// Dibujar el rectangulo de fondo con el color indicado.
			EditorGUI.DrawRect(rect, backgroundColor);
			// Y el nombre en negrita del color indicado.
			EditorGUI.LabelField(rect, name, labelStyle);
		}
	}
}
