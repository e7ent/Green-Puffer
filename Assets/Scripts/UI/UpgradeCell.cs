using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using E7;

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

        title.text = Localization.GetString(data.title);
		description.text = Localization.GetString(data.description);
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
		{
			AlertManager.instance.ShowAlert(Localization.GetString("not_enough_currency"));
			return;
		}
		
		AlertManager.instance.ShowAlert(Localization.GetString("upgrade_success"));

		UpgradeSystem.instance.Upgrade(upgradeName);
        Load(this.upgradeName);
	}

}
