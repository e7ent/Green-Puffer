using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class UpgradeData
{
	[XmlElement("Name")]
	public string name;
	[XmlElement("Title")]
	public string title;
	[XmlElement("Description")]
	public string description;
	[XmlElement("IconName")]
	public string iconName;

	[XmlArray("RequiredCurrency")]
	[XmlArrayItem("Currency")]
	public int[] requiredCurrency;
	[XmlElement("CurrentLevel")]
	public int currentLevel;

	private Sprite iconSprite;

	public void Upgrade()
	{
		currentLevel = Math.Min(currentLevel + 1, 5);
	}

	public int GetRequiredCurrency()
	{
		return 0;/*
		if (requiredCurrency == null)
			return 0;
		if (requiredCurrency.Length <= 0)
			return 0;
		if (currentLevel >= 5)
			return 0;
		return requiredCurrency[currentLevel];*/
	}

	public bool IsMaxLevel()
	{
		return currentLevel >= 5;
	}

	public Sprite GetIcon()
	{
		if (iconSprite != null)
			return iconSprite;
		foreach (var sprite in Resources.LoadAll<Sprite>("UI"))
		{
			if (sprite.name == iconName)
			{
				iconSprite = sprite;
				break;
			}
		}
		return iconSprite;
	}
}
