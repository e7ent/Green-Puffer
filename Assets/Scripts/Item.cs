﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Item : MonoBehaviour
{
	public float exp;
	public float satiety;
	public int currency;
	public GameObject feedFx;

	public void Use(PlayerController player)
	{
		if (player == null)
			return;
		
		if (feedFx)
			Instantiate(feedFx, transform.position, Quaternion.identity);

		GameManager.instance.currency += currency;
		player.Eat(exp, satiety);
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Use(other.GetComponent<PlayerController>());
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		Use(other.gameObject.GetComponent<PlayerController>());
	}
}
