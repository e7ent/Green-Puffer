using UnityEngine;

class AttackState : IState
{

	SpeechBubble bubble;

	public void Begin(PlayerStateMachine owner)
	{
		bubble = SpeechBubbleManager.instance.CreateBubble(0, LocalizationString.GetString("attack"));
		bubble.Attach(owner.transform);
		bubble.SetEffect(SpeechBubble.Effect.None);
	}

	public void Update(PlayerStateMachine owner)
	{

	}

	public void End(PlayerStateMachine owner)
	{
		bubble.Destory();
	}
}