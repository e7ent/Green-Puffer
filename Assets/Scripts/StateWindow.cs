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

		ageText.text = player.Name;
		genText.text = GameManager.instance.generation + " Gen";
		sizeText.text = player.Size + " Cm";

		expBar.fillAmount = player.Exp / player.MaxExp;
		hpBar.fillAmount = player.Hp / player.MaxHp;
	}
}
