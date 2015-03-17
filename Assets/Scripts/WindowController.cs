using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class WindowController : MonoBehaviour
{
	[SerializeField]
	private Image titleImage;
	[SerializeField]
	private Button closeButton;
	[SerializeField]
	private RectTransform content;

	void OnEnable()
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
