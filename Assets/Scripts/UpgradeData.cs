using System;
using UnityEngine;

[Serializable]
public class UpgradeData
{
	public string name;
	public string title;
	public string description;
	public Sprite icon;
	public int[] requiredMoney;
	public int currentLevel;

	public void Upgrade()
	{
		currentLevel = Math.Min(currentLevel + 1, 5);
	}

	public int NextRequiredMoney()
	{
		return requiredMoney[currentLevel + 1];
	}
}
