using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeWindow : MonoBehaviour
{
	public float cellHeight;
	public Transform content;
	public Transform defaultCell;

	void OnShow()
	{
		ReloadData();
	}

	public void ReloadData()
	{
		var upgrades = UpgradeSystem.instance.GetDataAll();
		Stack<Transform> reuse = new Stack<Transform>();

		for (int i = 0; i < upgrades.Count; i++)
		{
			Transform child = null;
			if (i < content.childCount)
			{
				child = content.GetChild(i);
			}
			else
			{
				child = Instantiate(defaultCell) as Transform;
				child.SetParent(content, false);
			}
			child.gameObject.SetActive(false);
			reuse.Push(child);
		}

		foreach (var item in upgrades)
		{
			Transform newCell = null;
			
			newCell = reuse.Pop();

			newCell.gameObject.SetActive(true);
			newCell.GetComponent<UpgradeCell>().Load(item.name);
		}
		
		var size = content.GetComponent<RectTransform>().sizeDelta;
		size.y = cellHeight * upgrades.Count;
		content.GetComponent<RectTransform>().sizeDelta = size;
	}

}
