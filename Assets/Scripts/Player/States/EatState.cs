using UnityEngine;
using E7;


class EatState : IState
{
	private SpeechBubble bubble;
	private float elapsed = 0;

	public void Begin(PlayerStateMachine owner)
	{
		bubble = SpeechBubbleManager.instance.CreateBubble(0, Localization.GetString("eat"));
		bubble.Attach(owner.transform);
		bubble.SetEffect(SpeechBubble.Effect.None);
		owner.animator.Change(PlayerAnimator.Type.Eat);
	}

	public void Update(PlayerStateMachine owner)
	{
		if ((elapsed += Time.deltaTime) > 3)
		{
			owner.Change(new NormalState());
		}
	}

	public void End(PlayerStateMachine owner)
	{
		bubble.Destory();
	}
}