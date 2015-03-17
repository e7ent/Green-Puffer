using UnityEngine;
using System.Collections;

public class MoveState : IState
{
	private Rigidbody2D rigidbody;

	public void Begin(PlayerController owner)
	{
		rigidbody = owner.GetComponent<Rigidbody2D>();
	}

	public void Update()
	{
	}

	public void End()
	{
	}

	public bool IsEnd()
	{
		return rigidbody.velocity.sqrMagnitude < .01;
	}

	public StateAnimationType GetAnimationType()
	{
		return StateAnimationType.Move;
	}

	public string Description()
	{
		return "움직이는 상태이다.";
	}

	public StateType GetStateType()
	{
		return StateType.Behavior;
	}
}
