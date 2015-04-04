using UnityEngine;
using System.Collections;

public class QuestTrigger : MonoBehaviour
{
	public string questName;

	private QuestData quest;

	private void Awake()
	{
		quest = QuestSystem.instance.GetQuest(questName);
	}

	private void OnUse()
	{
		quest.AddCount();
	}
}
