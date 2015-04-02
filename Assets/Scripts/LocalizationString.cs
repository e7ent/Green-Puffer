using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LocalizationString
{
	public static Dictionary<string, string> strings = new Dictionary<string, string>
	{
		{"eat", "냠냠..."},
		{"attack", "공격 메세지"},
	};

	public static string GetString(string id)
	{
		return strings[id];
	}
}
