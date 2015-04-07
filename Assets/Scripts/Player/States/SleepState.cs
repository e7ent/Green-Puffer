using UnityEngine;
using E7;

class SleepState : IState
{

	SpeechBubble bubble;

	public void Begin(PlayerStateMachine owner)
	{
		bubble = SpeechBubbleManager.instance.CreateBubble(0, Localization.GetString("sleep"));
		bubble.Attach(owner.transform);
		bubble.SetEffect(SpeechBubble.Effect.None);
		owner.animator.Change(PlayerAnimator.Type.Sleep);
	}

	public void Update(PlayerStateMachine owner)
	{
		if (owner.rigidbody.velocity.sqrMagnitude >= 0.1f)
		{
			owner.Change(new NormalState());
			return;
		}
	}

	public void End(PlayerStateMachine owner)
	{
		bubble.Destory();
	}
}