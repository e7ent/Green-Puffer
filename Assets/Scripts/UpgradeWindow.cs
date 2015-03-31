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
		Stack<Transform> reuse = new Stack<Transform>();

		for (int i = 0; i < content.childCount; i++)
		{
			var child = content.GetChild(i);
			child.gameObject.SetActive(false);
			reuse.Push(child);
		}

		var upgrades = GameManager.instance.upgrades;
		foreach (var item in upgrades)
		{
			Transform newCell = null;
			if (reuse.Count <= 0)
				newCell = Instantiate(defaultCell) as Transform;
			else
				newCell = reuse.Pop();

			newCell.gameObject.SetActive(true);
			newCell.GetComponent<UpgradeCell>().SetData(item);
			newCell.SetParent(content, false);
		}
		
		var size = content.GetComponent<RectTransform>().sizeDelta;
		size.y = cellHeight * upgrades.Length;
		content.GetComponent<RectTransform>().sizeDelta = size;
	}

}
