using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using E7;

public class TextLocalization : MonoBehaviour {
	public string key;

	public void OnEnable()
	{
		GetComponent<Text>().text = Localization.GetString(key);
	}
}
