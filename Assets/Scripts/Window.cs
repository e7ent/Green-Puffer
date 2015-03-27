using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Window : UIBehaviour
{
	private static Window focusedWindow = null;
	private bool isVisible = false;

	protected override void OnEnable()
	{
		if (!IsVisible())
			gameObject.SetActive(false);
	}

	public void Show()
	{
		if (focusedWindow != null) return;
		if (isVisible) return;
		focusedWindow = this;
		isVisible = true;

		transform.localScale = Vector3.zero;
		gameObject.SetActive(true);
		GameManager.instance.Pause();
		FadeManager.instance.Fade(new Color(0, 0, 0, 0), new Color(0, 0, 0, .5f), 0.25f);
		transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
	}

	public void Close()
	{
		if (!isVisible) return;
		isVisible = false;
		if (focusedWindow == this)
			focusedWindow = null;
		transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack).OnComplete(() =>
		{
			gameObject.SetActive(false);
			GameManager.instance.Resume();
			FadeManager.instance.Fade(new Color(0, 0, 0, .5f), new Color(0, 0, 0, 0), 0.25f);
			Time.timeScale = 1;
		});
	}

	public bool IsVisible()
	{
		return isVisible;
	}
}
