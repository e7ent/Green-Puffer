using UnityEngine;
using E7;

public class HurtState : IState
{
	private SpeechBubble bubble;
	private float elapsed = 0;

	public void Begin(PlayerStateMachine owner)
	{
		bubble = SpeechBubbleManager.instance.CreateBubble(0, Localization.GetString("hurt"));
		bubble.Attach(owner.transform);
		bubble.SetEffect(SpeechBubble.Effect.None);
		owner.animator.Change(PlayerAnimator.Type.Fear);
	}

	public void Update(PlayerStateMachine owner)
	{
		if ((elapsed += Time.deltaTime) > 2)
		{
			owner.Change(new NormalState());
		}
	}

	public void End(PlayerStateMachine owner)
	{
		bubble.Destory();
	}
}