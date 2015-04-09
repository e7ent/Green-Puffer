using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class SpeechBubble : UIBehaviour
{
	public System.Action<SpeechBubble> onEffectFinish;

	public enum Effect
	{
		None = 0,
		Typewriter,
		RepeatTypewriter,
	}
	[SerializeField]
	private RectTransform bubbleTransform;
	[SerializeField]
	private float maxWidthSize = 300;
	[SerializeField]
	private Text messageText;

	private string message;
	private Effect effects = Effect.None;
	private Vector2 originSize;
	private Vector2 followOffset;
	private Transform followTarget = null;

	protected override void Awake()
	{
		base.Awake();
		originSize = bubbleTransform.sizeDelta;
	}


	protected override void Start()
	{
		base.Start();

		transform.DOPunchScale(Vector3.one * 0.2f, 0.4f);
	}

	void Update()
	{
		if (followTarget)
		{
			followOffset.x = Mathf.Abs(followOffset.x) * Mathf.Sign(followTarget.localScale.x);
			bubbleTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, followTarget.position) + followOffset;
		}
	}

	public SpeechBubble SetMessage(string text)
	{
		var col = Mathf.Min(text.Length, maxWidthSize / messageText.fontSize);
		var row = Mathf.Max(Mathf.CeilToInt(text.Length / col), 1);
		var newSize = new Vector2(col, row) * messageText.fontSize;
		newSize.x += originSize.x - messageText.fontSize;
		bubbleTransform.sizeDelta = Vector2.Max(originSize, newSize);
		messageText.text = text.ToUpper();
		message = text.ToUpper();
		StartEffect();
		return this;
	}

	public SpeechBubble SetEffect(Effect effects)
	{
		this.effects = effects;
		StartEffect();
		return this;
	}

	public SpeechBubble Attach(Transform target, Vector2? offset = null)
	{
		followTarget = target;
		if (offset == null)
		{
			Bounds? bounds = null;
			foreach (var collider in target.GetComponents<Collider2D>())
			{
				if (bounds == null)
					bounds = collider.bounds;
				else
					bounds.Value.Encapsulate(collider.bounds);
			}
			var expectPos = followTarget.position + new Vector3(bounds.Value.extents.x, bounds.Value.extents.y, 0);
			followOffset = RectTransformUtility.WorldToScreenPoint(Camera.main, expectPos) - RectTransformUtility.WorldToScreenPoint(Camera.main, followTarget.position);
		}
		else
		{
			followOffset = offset.Value;
		}
		return this;
	}

	public SpeechBubble Detach()
	{
		followTarget = null;
		return this;
	}

	public void Destory()
	{
//		transform.DOKill(false);
		GameObject.Destroy(gameObject);
	}

	private void StartEffect()
	{
		switch (effects)
		{
			case Effect.None:
				break;
			case Effect.Typewriter:
			case Effect.RepeatTypewriter:
				StopCoroutine("TypeWriterEffect");
				StartCoroutine("TypeWriterEffect");
				break;
			default:
				break;
		}
	}

	private IEnumerator TypeWriterEffect()
	{
		do
		{
			for (int i = 0; i <= message.Length; i++)
			{
				messageText.text = message.Substring(0, i);
				yield return StartCoroutine(WaitForRealSeconds(.1f));
			}
			if (effects != Effect.RepeatTypewriter) break;
		} while (true);
		onEffectFinish(this);
	}
	IEnumerator WaitForRealSeconds(float seconds)
	{
		var start = Time.unscaledTime + seconds;
		while (Time.unscaledTime < start)
			yield return null;
	}
}
