using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimator : MonoBehaviour
{
	public Animator eye;
	public Animator nose;
	public Animator mouth;
	public Animator body;
	public Animator fin;

	private new Rigidbody2D rigidbody;

	void Start()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		fin.speed = Mathf.Clamp(rigidbody.velocity.sqrMagnitude * 5.0f, 0.5f, 2);
	}

	#region Util

	public void ChangeState(StateAnimationType state)
	{
		eye.Play("Init");
		int value = (int)state;

		eye.SetInteger("State", value);
		nose.SetInteger("State", value);
		mouth.SetInteger("State", value);
		body.SetInteger("State", value);
		fin.SetInteger("State", value);
	}

	#endregion
}