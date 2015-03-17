using UnityEngine;
using System.Collections;

public class EatState : IState
{
	private float elapsed = 0;

	public void Begin(PlayerController owner)
	{
		elapsed = 0;
	}

	public void Update()
	{
		elapsed += Time.deltaTime;
	}

	public void End()
	{
	}

	public bool IsEnd()
	{
		return elapsed > 0.5f;
	}

	public StateAnimationType GetAnimationType()
	{
		return StateAnimationType.Eat;
	}

	public string Description()
	{
		return "무언가 먹는 상태이다.";
	}

	public StateType GetStateType()
	{
		return StateType.Action;
	}
}
