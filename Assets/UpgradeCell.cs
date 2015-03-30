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

	private UpgradeData data;

	public void SetData(UpgradeData data)
	{
		this.data = data;
		title.text = data.title;
		description.text = data.description;
		icon.sprite = data.icon;
		slot.Value = data.currentLevel;
		coin.text = string.Format("{0:#,###}", data.NextRequiredMoney());
	}

	public void Upgrade()
	{
		GameManager.instance.Upgrade(data.name);
	}

}
