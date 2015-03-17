using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System.Collections;

[CustomEditor(typeof(LayerSorter))]
public class LayerSorterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var renderers = (target as MonoBehaviour).GetComponents<Renderer>();
		if (renderers == null) return;

		var sortingLayer = renderers[0].sortingLayerName;
		var sortingOrder = renderers[0].sortingOrder;
		var sortingLayersProperty = typeof(InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		var layers = (string[])sortingLayersProperty.GetValue(null, new object[0]);

		if (string.IsNullOrEmpty(sortingLayer))
			sortingLayer = layers[0];
		
		sortingLayer = layers[EditorGUILayout.Popup("Layer Name", System.Array.IndexOf(layers, sortingLayer), layers)];
		sortingOrder = EditorGUILayout.IntField("Order", sortingOrder);

		foreach (var renderer in renderers)
		{
			renderer.sortingLayerName = sortingLayer;
			renderer.sortingOrder = sortingOrder;
		}
	}
}
