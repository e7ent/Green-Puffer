using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class UpgradeSystem : MonoSingleton<UpgradeSystem>
{
    private static string assetPath = "upgrade";
    private static string userDataPath = "upgradeUserData";

    [SerializeField]
    private Dictionary<string, UpgradeData> datas = new Dictionary<string, UpgradeData>();
    [SerializeField]
    private Dictionary<string, UpgradeUserData> userDatas = new Dictionary<string, UpgradeUserData>();

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

    private void OnApplicationQuit()
    {
        Save();
    }

	public void Load()
    {
        this.datas.Clear();
        var asset = Resources.Load<TextAsset>(assetPath);
        using (XmlReader reader = XmlTextReader.Create(new StringReader(asset.text)))
		{
    		XmlSerializer serializer = new XmlSerializer(typeof(UpgradeData[]));
			UpgradeData[] upgrades = serializer.Deserialize(reader) as UpgradeData[];
			foreach (UpgradeData item in upgrades)
				this.datas[item.name] = item;
        }

        this.userDatas.Clear();

        string savedUserData = PlayerPrefs.GetString(userDataPath, string.Empty);
        if (string.IsNullOrEmpty(savedUserData))
            return;
        
        using (XmlReader reader = XmlTextReader.Create(new StringReader(savedUserData)))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UpgradeUserData[]));
            UpgradeUserData[] upgrades = serializer.Deserialize(reader) as UpgradeUserData[];
            foreach (UpgradeUserData item in upgrades)
                this.userDatas[item.upgradeDataName] = item;
        }
    }

    public void Reset()
    {
        this.userDatas.Clear();
    }

	public void Save()
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.IndentChars = "\t";
		settings.NewLineOnAttributes = true;

        StringBuilder sb = new StringBuilder();
        using (XmlWriter writer = XmlTextWriter.Create(sb, settings))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UpgradeUserData[]));
            var array = new UpgradeUserData[userDatas.Count];
            userDatas.Values.CopyTo(array, 0);
            serializer.Serialize(writer, array);
        }
        PlayerPrefs.SetString(userDataPath, sb.ToString());
	}

    public int GetLevel(string name)
    {
        return GetUserData(name).currentLevel;
    }

	public UpgradeData GetData(string name)
	{
		return datas[name];
	}

    public ICollection<UpgradeData> GetDataAll()
    {
        return datas.Values;
    }

    public UpgradeUserData GetUserData(string name)
    {
        if (userDatas.ContainsKey(name))
            return userDatas[name];
        
        var userData = userDatas[name] = new UpgradeUserData();
        userData.upgradeDataName = name;
        userData.currentLevel = 0;
        return userData;
    }

	public bool CanUpgrade(string name)
	{
		var currentCurrency = GameManager.instance.Currency;
        var currentLevel = GetUserData(name).currentLevel;
		var requiredCurrency = GetData(name).GetRequiredCurrency(currentLevel);
		return currentCurrency - requiredCurrency >= 0;
	}

    public void Upgrade(string name)
    {
        var data = GetData(name);
        var userData = GetUserData(name);

        GameManager.instance.Currency -= data.GetRequiredCurrency(userData.currentLevel);
        GetUserData(name).currentLevel++;
    }
}
