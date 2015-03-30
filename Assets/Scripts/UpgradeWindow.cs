using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeWindow : MonoBehaviour
{
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
			var newCell = reuse.Pop();
			if (newCell == null)
				newCell = Instantiate(defaultCell) as Transform;

			newCell.gameObject.SetActive(true);
			newCell.GetComponent<UpgradeCell>().SetData(item);
			newCell.SetParent(content, false);
		}
	}

}
