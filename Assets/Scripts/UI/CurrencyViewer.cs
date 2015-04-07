using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrencyViewer : MonoBehaviour
{
	public Text text;
 
	void Update()
	{
		text.text = string.Format("{0:#,##0}", GameManager.instance.Currency);
	}
}
