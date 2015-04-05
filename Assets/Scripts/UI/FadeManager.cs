using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class FadeManager : MonoSingleton<FadeManager>
{
	public delegate void FadeCallback();

	private Canvas canvas;
	private Image image;

    private Color color;

	protected override void Awake()
	{
        base.Awake();

		canvas = GetComponent<Canvas>();
		image = GetComponent<Image>();

		canvas.enabled = false;
	}

	/// <summary>
	/// 투명으로 페이드인 된다.
	/// </summary>
	public static void FadeIn(float duration = 1, FadeCallback onComplate = null)
	{
		instance.Fade(instance.color, Color.clear, duration, onComplate);
	}

	/// <summary>
	/// 투명에서 검은색으로 페이드아웃 된다.
	/// </summary>
	public static void FadeOut(float duration = 1, FadeCallback onComplate = null)
	{
		instance.Fade(Color.clear, Color.black, duration, onComplate);
	}

	/// <summary>
	/// 페이드 한다.
	/// </summary>
	/// <param name="from">시작 색상</param>
	/// <param name="to">끝 색상</param>
	/// <param name="duration">페이드에 걸리는 시간</param>
	/// <param name="onComplate">페이드 완료 콜백</param>
	public void Fade(Color from, Color to, float duration = 1, FadeCallback onComplate = null)
	{
		image.DOKill(false);
		if (canvas.enabled != true)
			canvas.enabled = true;
		image.color = from;
		image.DOColor(to, duration).OnComplete(() =>
		{
			if (onComplate != null)
				onComplate();

			if (to.a <= 0)
				canvas.enabled = false;
		});
        color = to;
	}

	/// <summary>
	/// Set Canvas SortOrder
	/// </summary>
	/// <param name="order">Sort Order</param>
	public void SetSortOrder(int order)
	{
		canvas.sortingOrder = order;
	}
}
