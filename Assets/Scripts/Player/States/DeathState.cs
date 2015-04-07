using UnityEngine;
using E7;

public class DeathState : IState
{
    private float elapsed = 0;
    private SpeechBubble bubble;

	public void Begin(PlayerStateMachine owner)
	{
        owner.animator.Change(PlayerAnimator.Type.Fear);
        
		bubble = SpeechBubbleManager.instance.CreateBubble(0, Localization.GetString("death"));
		bubble.Attach(owner.transform);
		bubble.SetEffect(SpeechBubble.Effect.None);
	}

	public void Update(PlayerStateMachine owner)
    {
        if ((elapsed += Time.deltaTime) >= 2)
        {
            GameManager.instance.Finish();
        }
	}

	public void End(PlayerStateMachine owner)
	{
        bubble.Destory();
	}
}