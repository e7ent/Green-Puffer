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

	private Sprite iconSprite;

	public int GetRequiredCurrency(int level)
	{
		if (requiredCurrency == null)
			return 0;
		if (requiredCurrency.Length <= 0)
			return 0;
        if (level >= requiredCurrency.Length)
            return 0;
		return requiredCurrency[level];
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
