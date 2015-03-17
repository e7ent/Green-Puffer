using UnityEngine;


public class LazeState : IState
{
	private PlayerController owner;
	private float elapsed = 0;

	public void Begin(PlayerController owner)
	{
		this.owner = owner;
		elapsed = 0;
	}

	public void Update()
	{
		if ((elapsed += Time.deltaTime) >= 5)
			owner.SetActionState(new SleepState());
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
		return StateAnimationType.Laze;
	}

	public string Description()
	{
		return "나른한 상태, 몇초간 아무런 행동도 하지 않을때";
	}

	public StateType GetStateType()
	{
		return StateType.Feel;
	}
}
