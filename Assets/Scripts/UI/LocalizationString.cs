using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LocalizationString
{
    public static Dictionary<string, string> strings = new Dictionary<string, string>
	{
		{"eat", "냠냠..."},
		{"attack", "공격 메세지"},
		{"death", "죽었을때 메세지"},
        {"baby", "아기"},
        {"kid", "어린이"},
        {"adult", "성인"},
	};
    public static Dictionary<string, string[]> stringArrays = new Dictionary<string, string[]>
	{
		{
            "death", new string[] {
                "죽음 메세지 1",
                "죽음 메세지 2",
                "죽음 메세지 3",
            }
        },
	};

    public static string GetString(string id)
    {
        if (strings.ContainsKey(id) == false)
            return string.Empty;
        return strings[id];
    }
    public static string[] GetStrings(string id)
    {
        if (stringArrays.ContainsKey(id) == false)
            return null;
        return stringArrays[id];
    }
}
