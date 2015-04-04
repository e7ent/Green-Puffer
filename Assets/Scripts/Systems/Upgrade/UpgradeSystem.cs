using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class UpgradeSystem : MonoSingleton<UpgradeSystem>
{
	private static string assetPath = Application.persistentDataPath + "/Resources/upgrade.xml";

	[SerializeField]
	private Dictionary<string, UpgradeData> upgrades;

	protected override void Awake()
	{
		base.Awake();
		Load();
	}

	protected override void OnDestroy()
	{
		Save();
		base.OnDestroy();
	}

	public void Load()
	{
		this.upgrades = new Dictionary<string, UpgradeData>();

		XmlSerializer serializer = new XmlSerializer(typeof(UpgradeData[]));
		using (XmlReader reader = XmlReader.Create(assetPath))
		{
			UpgradeData[] upgrades = serializer.Deserialize(reader) as UpgradeData[];
			foreach (UpgradeData item in upgrades)
				this.upgrades[item.name] = item;
		}
	}

	public void Save()
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.IndentChars = "\t";
		settings.NewLineOnAttributes = true;
		XmlSerializer serializer = new XmlSerializer(typeof(UpgradeData[]));
		using (XmlWriter writer = XmlWriter.Create(assetPath, settings))
		{
			var array = new UpgradeData[upgrades.Count];
			upgrades.Values.CopyTo(array, 0);
			serializer.Serialize(writer, array);
		}
	}

	public UpgradeData Get(string name)
	{
		return upgrades[name];
	}

	public bool CanUpgrade(string name)
	{
		var currentCurrency = GameManager.instance.currency;
		var requiredCurrency = upgrades[name].GetRequiredCurrency();
		return currentCurrency - requiredCurrency >= 0;
	}

	public ICollection<UpgradeData> GetAll()
	{
		return upgrades.Values;
	}
}
