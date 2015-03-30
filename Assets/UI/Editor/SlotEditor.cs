using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Slot))]
public class SlotEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var slot = target as Slot;
		slot.Value = EditorGUILayout.IntSlider("Value", slot.Value, 0, slot.slots.Length);
	}
}
