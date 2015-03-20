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
		var puffer = GameManager.instance.player;
		if (puffer == null) return;

		ageText.text = puffer.stat.name;
		genText.text = GameManager.instance.generation + " Gen";
		sizeText.text = puffer.stat.GetSize() + " Cm";

		expBar.fillAmount = (float)puffer.stat.exp / puffer.stat.maxExp;
		hpBar.fillAmount = (float)puffer.stat.hp / puffer.stat.maxHp;
	}
}
