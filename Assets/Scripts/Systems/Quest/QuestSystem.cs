using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class QuestSystem : MonoSingleton<QuestSystem>
{
	private static string assetPath = Application.dataPath + "/Resources/quest.xml";

	private Dictionary<string, QuestData> quests = new Dictionary<string,QuestData>();

	public void Load()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(QuestData[]));
		using (XmlReader reader = XmlReader.Create(assetPath))
		{
			QuestData[] quests = serializer.Deserialize(reader) as QuestData[];
			foreach (QuestData quest in quests)
				this.quests[quest.name] = quest;
		}
	}

	public void Save()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(QuestData[]));
		using (XmlWriter writer = XmlWriter.Create(assetPath))
		{
			serializer.Serialize(writer, quests.Values);
		}
	}

	public QuestData GetQuest(string questName)
	{
		return quests[questName];
	}
}
