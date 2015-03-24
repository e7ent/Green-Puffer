using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Window : UIBehaviour
{
	private bool isVisible = false;
	protected override void OnEnable()
	{
		if (!IsVisible())
			gameObject.SetActive(false);
	}

	public void Show()
	{
		if (isVisible) return;
		isVisible = true;

		transform.localScale = Vector3.zero;
		gameObject.SetActive(true);
		GameManager.instance.Pause();
		transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
	}

	public void Close()
	{
		if (!isVisible) return;
		isVisible = false;

		transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack).OnComplete(() =>
		{
			gameObject.SetActive(false);
			Time.timeScale = 1;
		});
	}

	public bool IsVisible()
	{
		return isVisible;
	}
}
