using UnityEngine;
using E7;

public class HungryState : IState
{
	private SpeechBubble bubble;
	private float elapsed = 0;

	public void Begin(PlayerStateMachine owner)
	{
		bubble = SpeechBubbleManager.instance.CreateBubble(0, Localization.GetString("hurt"));
		bubble.Attach(owner.transform);
		bubble.SetEffect(SpeechBubble.Effect.None);
		owner.animator.Change(PlayerAnimator.Type.Hungry);
	}

	public void Update(PlayerStateMachine owner)
	{
		if ((elapsed += Time.deltaTime) > 2)
		{
			owner.Change(new NormalState());
		}
		else if (owner.rigidbody.velocity.sqrMagnitude >= 0.1f)
		{
			owner.Change(new NormalState());
		}
	}

	public void End(PlayerStateMachine owner)
	{
		bubble.Destory();
	}
}