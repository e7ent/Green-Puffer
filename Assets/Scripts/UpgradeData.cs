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

	public int NextRequiredCurrency()
	{
		if (requiredMoney == null)
			return 0;
		if (requiredMoney.Length <= 0)
			return 0;
		if (currentLevel >= 5)
			return 0;
		return requiredMoney[currentLevel];
	}

	public bool IsMaxLevel()
	{
		return currentLevel >= 5;
	}
}
