using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StateWindow : MonoBehaviour
{
	public Text ageText, genText, sizeText;
	public Image hpBar;
	public Image expBar;

	void OnEnable()
	{
		var player = FindObjectOfType<PlayerController>();
		if (player == null) return;

		ageText.text = player.stat.name;
		genText.text = GameManager.instance.generation + " Gen";
		sizeText.text = player.stat.GetSize() + " Cm";

		expBar.fillAmount = (float)player.stat.exp / player.stat.maxExp;
		hpBar.fillAmount = (float)player.stat.hp / player.stat.maxHp;
	}
}
