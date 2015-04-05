using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class UpgradeUserData
{
    [XmlElement("UpgradeDataName")]
    public string upgradeDataName;
    [XmlElement("CurrentLevel")]
    public int currentLevel;
}
