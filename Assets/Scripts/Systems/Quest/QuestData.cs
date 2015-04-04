using System;
using UnityEngine;

[Serializable]
public class QuestData
{
	public enum State
	{
		InActive,
		Active,
		Complete,
		End,
	}

	public string name;
	public string title;
	public string description;
	public string iconName;

	public int count;
	public int conditionCount;
	public int rewardCurrency;
	public State state = State.Active;

	private Sprite iconSprite;

	public void AddCount()
	{
		if (++count >= conditionCount)
		{
			state = State.Complete;
		}
	}

	public State GetState()
	{
		return state;
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
