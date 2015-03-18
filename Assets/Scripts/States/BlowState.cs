using UnityEngine;


public class BlowState : IState
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
		if ((elapsed += Time.deltaTime) >= 3)
			owner.SetActionState(new LazeState());
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
		return StateAnimationType.Blow;
	}

	public string Description()
	{
		return "아무런 행동도 하지 않는 기본 상태이다.";
	}

	public StateType GetStateType()
	{
		return StateType.Body;
	}
}
