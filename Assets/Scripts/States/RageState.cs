using UnityEngine;
using System.Collections;

public class RageState : IState
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
		return StateAnimationType.Rage;
	}

	public string Description()
	{
		return "화난 상태이다.";
	}

	public StateType GetStateType()
	{
		return StateType.Feel;
	}
}
