using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusView : MonoBehaviour
{
	public Image hpBar;
	public Image expBar;
	public Image rankImage;
	public Text generationText;
	public Image injuryImage;

	private PlayerController puffer;

	void Start()
	{
	}

	void Update()
	{
		if (puffer == null)
		{
			puffer = FindObjectOfType<PlayerController>();
			if (puffer == null)
				return;
		}

		expBar.fillAmount = (float)puffer.stat.exp / puffer.stat.maxExp;
		hpBar.fillAmount = (float)puffer.stat.hp / puffer.stat.maxHp;
	}
}
