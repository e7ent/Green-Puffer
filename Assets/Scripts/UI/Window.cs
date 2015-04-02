using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Window : UIBehaviour
{
	private static List<Window> opendWindows = new List<Window>();
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

		if (opendWindows.Count <= 0)
		{
			GameManager.instance.Pause();
			FadeManager.instance.Fade(new Color(0, 0, 0, 0), new Color(0, 0, 0, .5f), 0.25f);
		}
		opendWindows.Add(this);

		transform.localScale = Vector3.zero;
		gameObject.SetActive(true);
		transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);

		SendMessage("OnShow", SendMessageOptions.DontRequireReceiver);
	}

	public void Close()
	{
		Close(null);
	}

	public void Close(System.Action onClose)
	{
		if (!isVisible) return;
		isVisible = false;

		transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack).OnComplete(() =>
		{
			gameObject.SetActive(false);
			opendWindows.Remove(this);
			if (opendWindows.Count <= 0)
			{
				GameManager.instance.Resume();
				FadeManager.instance.Fade(new Color(0, 0, 0, .5f), new Color(0, 0, 0, 0), 0.25f);
			}
			if (onClose != null)
				onClose();
			SendMessage("OnClose", SendMessageOptions.DontRequireReceiver);
		});
	}

	public bool IsVisible()
	{
		return isVisible;
	}
}
