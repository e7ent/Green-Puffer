using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeCell : MonoBehaviour
{
	public Image icon;
	public Text title;
	public Slot slot;
	public Text description;
	public Button upgradeButton;
	public Text coin;

	private UpgradeData upgradeData;

	public void Load(UpgradeData data)
	{
		upgradeData = data;
		Bind(upgradeData);
	}

	private void Bind(UpgradeData data)
	{
		title.text = data.title;
		description.text = data.description;
		icon.sprite = data.GetIcon();
		slot.Value = data.currentLevel;

		coin.gameObject.SetActive(true);
		upgradeButton.interactable = true;
		coin.text = string.Format("{0:#,##0}", data.GetRequiredCurrency());

		if (data.IsMaxLevel())
		{
			coin.gameObject.SetActive(false);
			upgradeButton.interactable = false;
		}
	}

	public void Upgrade()
	{
		var res = UpgradeSystem.instance.CanUpgrade(upgradeData.name);
		if (res == false)
			AlertManager.instance.ShowAlert("돈이 부족합니다.");
		else
			AlertManager.instance.ShowAlert("업그레이드가 완료 되었습니다.");

		upgradeData.Upgrade();
		Bind(upgradeData);
	}

}
