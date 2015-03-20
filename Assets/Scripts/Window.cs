using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Window : UIBehaviour
{
	protected override void OnEnable()
	{
		Show();
	}

	public void Show()
	{
		Time.timeScale = 0.00001f;
		transform.localScale = Vector3.zero;
		transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
	}

	public void Close()
	{
		transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack).OnComplete(() =>
		{
			gameObject.SetActive(false);
			Time.timeScale = 1;
		});
	}
}
