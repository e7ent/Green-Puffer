using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerAnimator : MonoBehaviour
{
	public enum Type
	{
		None = -1,
		Normal = 0,
		Thin = 1,
		Fat = 2,
		Rage = 3,
		Eat = 4,
		Attack = 5,
		Fear = 6,
		Stress = 7,
		Sweat = 8,
		Laze = 9,
		Pinch = 10,
		Sleep = 11,
		Hungry = 12,
		SpaceOut = 13,
		Full = 14,
		Blow = 15,
		Move = 16,
		Die = 17,
		Happy = 18,
		Fun = 19,
	}

	public Animator eye;
	public Animator nose;
	public Animator mouth;
	public Animator body;
	public Animator fin;

	private new Rigidbody2D rigidbody;
	private Type currentType; 

	void Start()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		fin.speed = Mathf.Clamp(rigidbody.velocity.sqrMagnitude * 5.0f, 0.5f, 2);
	}

	#region Util

	public void ChangeBodyAnimation(Type type)
	{
		int value = (int)type;
		body.SetInteger("State", value);
	}

	public void Change(Type type)
	{
		if (currentType == type)
			return;

		currentType = type;

		eye.Play("Init");
		int value = (int)type;

		eye.SetInteger("State", value);
		nose.SetInteger("State", value);
		mouth.SetInteger("State", value);
		body.SetInteger("State", value);
		fin.SetInteger("State", value);
	}

	#endregion
}