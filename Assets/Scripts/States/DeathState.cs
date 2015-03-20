using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DeathState : IState
{
	private SpeechBubble speechBubble;
	private string[] speechMessages = { "으앙 주금..", "으아아 힘이 빠진다.." };

	public void Begin(PlayerController owner)
	{
		Time.timeScale = 0.001f;
		speechBubble =
			SpeechBubbleManager.instance.CreateBubble(0, speechMessages[Random.Range(0, speechMessages.Length)]).
			Attach(owner.transform);
		speechBubble.SetEffect(SpeechBubble.SpeechBubbleEffect.Typewriter);
		speechBubble.onEffectFinish = (SpeechBubble bubble) =>
		{
			speechBubble.Destory();
			var seq = DOTween.Sequence();
			seq.Append(owner.transform.DORotate(new Vector3(0, 0, 180), 1));
			seq.Append(owner.transform.DOMoveY(2.9f, 2).SetEase(Ease.Linear));
			seq.OnComplete(() =>
			{
				GameManager.instance.EndGame();
			});
		};
	}

	public void Update()
	{

	}

	public void End()
	{
	}

	public bool IsEnd()
	{
		return false;
	}

	public StateAnimationType GetAnimationType()
	{
		return StateAnimationType.Die;
	}

	public string Description()
	{
		return "죽은 상태이다.";
	}

	public StateType GetStateType()
	{
		return StateType.Behavior;
	}
}
