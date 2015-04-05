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

    private string upgradeName;

	public void Load(string name)
	{
        upgradeName = name;

        var data = UpgradeSystem.instance.GetData(name);
        var userData = UpgradeSystem.instance.GetUserData(name);

        title.text = data.title;
        description.text = data.description;
        icon.sprite = data.GetIcon();
        slot.Value = userData.currentLevel;

        coin.gameObject.SetActive(true);
        upgradeButton.interactable = true;
        coin.text = string.Format("{0:#,##0}", data.GetRequiredCurrency(userData.currentLevel));

        if (userData.currentLevel >= 5)
        {
            coin.gameObject.SetActive(false);
            upgradeButton.interactable = false;
        }
	}

	public void Upgrade()
	{
		var res = UpgradeSystem.instance.CanUpgrade(this.upgradeName);
		if (res == false)
			AlertManager.instance.ShowAlert("돈이 부족합니다.");
		else
			AlertManager.instance.ShowAlert("업그레이드가 완료 되었습니다.");

		UpgradeSystem.instance.Upgrade(upgradeName);
        Load(this.upgradeName);
	}

}
