using UnityEngine;

public class NormalState : IState
{
	private float sleepElapsed = 0;

	public void Begin(PlayerStateMachine owner)
	{
	}

	public void Update(PlayerStateMachine owner)
	{
		var hp = owner.controller.Hp;
		var satiety = owner.controller.Satiety;

		if (owner.rigidbody.velocity.sqrMagnitude <= 0.1f)
		{
			if ((sleepElapsed += Time.deltaTime) >= 1)
			{
				owner.Change(new SleepState());
				return;
			}
		}
		else
		{
			sleepElapsed = 0;
		}

		if (hp <= (owner.controller.MaxHp * 0.1f))
		{
			owner.animator.Change(PlayerAnimator.Type.Blow);
		}
		else if (Mathf.Abs(satiety) <= 0.5f)
		{
			owner.animator.Change(PlayerAnimator.Type.Normal);
		}
		else if (satiety < 0)
		{
			owner.animator.Change(PlayerAnimator.Type.Thin);
		}
		else
		{
			owner.animator.Change(PlayerAnimator.Type.Fat);
		}
	}

	public void End(PlayerStateMachine owner)
	{

	}
}
