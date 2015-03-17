using UnityEngine;


public class SleepState : IState
{
	private SpeechBubble speechBubble;
	private string[] speechMessages = { "ZZZ...", "음냐..음냐.." };

	public void Begin(PlayerController owner)
	{
		speechBubble = 
			SpeechBubbleManager.instance.CreateBubble(0, speechMessages[Random.Range(0, speechMessages.Length)]).
			Attach(owner.transform);
		speechBubble.SetEffect(SpeechBubble.SpeechBubbleEffect.RepeatTypewriter);
	}

	public void Update()
	{
	}

	public void End()
	{
		speechBubble.Destory();
	}

	public bool IsEnd()
	{
		return false;
	}

	public StateAnimationType GetAnimationType()
	{
		return StateAnimationType.Sleep;
	}

	public string Description()
	{
		return "오랫동안 아무것도 하지 않는 상태 이다.";
	}

	public StateType GetStateType()
	{
		return StateType.Feel;
	}
}
