using UnityEngine;
using System.Collections;

[System.Serializable]
public class Quest
{
	public string name;
	public string title;
	public string description;
	public int count;
	public int conditionCount;

	public void AddCount()
	{
		count++;
	}

	public bool IsComplete()
	{
		return count >= conditionCount;
	}
}
