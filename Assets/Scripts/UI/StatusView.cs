using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using E7;

public class StatusView : MonoBehaviour
{
	public Image hpBar;
	public Image expBar;
	public Image rankImage;
	public Text generationText;
	public Button rebirthButton;
	public Image injuryImage;

	private PlayerController player;

	void Start()
	{
	}

	void Update()
	{
		if (player == null)
		{
			player = FindObjectOfType<PlayerController>();
			if (player == null)
				return;
		}

		expBar.fillAmount = player.Exp / player.MaxExp;
		hpBar.fillAmount = player.Hp / player.MaxHp;

		if (player.Rank >= PlayerController.RankType.Adult ^ rebirthButton.gameObject.activeSelf)
			rebirthButton.gameObject.SetActive(player.Rank >= PlayerController.RankType.Adult);
	}

	public void Rebirth()
	{
		AlertManager.instance.ShowAlert(
			Localization.GetString("ask_rebirth"),
			true,
			(bool ret) => {
				if (ret == false)
				{
					return;
				}

				GameManager.instance.Finish(GameManager.FinishType.Rebirth);
		});
	}
}
