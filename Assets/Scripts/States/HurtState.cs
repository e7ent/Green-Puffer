using UnityEngine;
using System.Collections;

public class HurtState : IState
{
	private SpeechBubble talkbox;
	private float elapsed = 0;
	private string[] talkBoxMessages = { "으아아" };

	public void Begin(PlayerController owner)
	{
		talkbox = SpeechBubbleManager.instance.CreateBubble(0, talkBoxMessages[Random.Range(0, talkBoxMessages.Length)]).Attach(owner.transform);
		elapsed = 0;
	}

	public void Update()
	{
		elapsed += Time.deltaTime;
	}

	public void End()
	{
		talkbox.Destory();
	}

	public bool IsEnd()
	{
		return elapsed > 2.0f;
	}

	public StateAnimationType GetAnimationType()
	{
		return StateAnimationType.Fear;
	}

	public string Description()
	{
		return "공격받는 상태이다.";
	}


	public StateType GetStateType()
	{
		return StateType.Action;
	}
}
