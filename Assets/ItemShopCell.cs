using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using E7;

public class ItemShopCell : MonoBehaviour
{
	public enum Type
	{
		HpMax,
		ExpMax,
		Treatment,
	}


	[SerializeField]
	private Image iconImage;
	[SerializeField]
	private Text titleText, descriptionText, effectText, coinText;

	[SerializeField]
	private Type type;
	[SerializeField]
	private Sprite icon;
	[SerializeField]
	private string title;
	[SerializeField]
	private string description;
	[SerializeField]
	private string effect;
	[SerializeField]
	private int requiredCurrency;

	public void OnEnable()
	{
		titleText.text = Localization.GetString(title);
		descriptionText.text = Localization.GetString(description);
		effectText.text = Localization.GetString(effect);
		iconImage.sprite = icon;

		coinText.text = string.Format("{0:#,##0}", requiredCurrency);
	}


	public void Buy()
	{
		if ((GameManager.instance.Currency - requiredCurrency) < 0)
		{
			AlertManager.instance.ShowAlert(Localization.GetString("not_enough_currency"));
			return;
		}
		else
		{
			AlertManager.instance.ShowAlert(Localization.GetString("item_success"));
			GameManager.instance.Currency -= requiredCurrency;
			var player = FindObjectOfType<PlayerController>();
			switch (type)
			{
				case Type.HpMax:
					player.Hp = player.MaxHp;
					break;
				case Type.ExpMax:
					player.Exp = player.MaxExp;
					break;
				case Type.Treatment:
					player.IsSick = false;
					break;
				default:
					break;
			}
			return;
		}
	}
}
