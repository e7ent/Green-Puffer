using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace E7
{
	public static class Localization
	{
		private const string prefsRegionKey = "E7.Localization.Code";
		private const string defaultRegion = "KR";

		private static bool isDirty = true;
		private static string region;
		private static Dictionary<string, string> dictionary = null;
		private static Dictionary<string, string[]> arrayDictionary = null;

		public static string Region
		{
			get
			{
				if (string.IsNullOrEmpty(region))
					region = PlayerPrefs.GetString(prefsRegionKey, defaultRegion);
				return region;
			}
			set
			{
				region = value;
				isDirty = true;
				PlayerPrefs.SetString(prefsRegionKey, region);
			}
		}

		public static string GetString(string key)
		{
			if (isDirty)
				Load();

			if (dictionary.ContainsKey(key))
				return dictionary[key];

			return string.Empty;
		}

		public static string[] GetStringArray(string key)
		{
			if (isDirty)
				Load();

			if (arrayDictionary.ContainsKey(key))
				return arrayDictionary[key];

			return new string[] { string.Empty };
		}

		private static bool Load()
		{
			var textAsset = Resources.Load<TextAsset>("Languages/" + Region);
			if (textAsset == null)
				return false;

			dictionary = new Dictionary<string, string>();
			arrayDictionary = new Dictionary<string, string[]>();

			var doc = new XmlDocument();
			doc.LoadXml(textAsset.text);

			var root = doc.DocumentElement;
			foreach (XmlNode node in root)
			{
				switch (node.Name)
				{
					case "string":
						dictionary.Add(node.Attributes["name"].Value, node.InnerText);
						break;
					case "string-array":
						var childNode = node.ChildNodes;
						string[] array = new string[childNode.Count];
						for (int i = 0; i < childNode.Count; i++)
							array[i] = childNode[i].InnerText;
						arrayDictionary.Add(node.Attributes["name"].Value, array);
						break;
					default:
						break;
				}
			}
			isDirty = false;
			Debug.LogFormat("Language Load Success : {0}", Region);
			return true;
		}
	}
}