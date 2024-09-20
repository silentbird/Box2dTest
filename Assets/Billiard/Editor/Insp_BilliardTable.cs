using Billiard.Tables;
using UnityEditor;
using UnityEngine;

namespace Billiard.Editor {
	[CustomEditor(typeof(BilliardTable))]
	public class Insp_BilliardTable : UnityEditor.Editor {
		private TableType tableType;

		public override void OnInspectorGUI() {
			var intVal = serializedObject.FindProperty("tableType").intValue;
			// if (intVal == TableType.RectangleTable) {
			// 	GUILayout.Label("");
			// }
		}
	}
}