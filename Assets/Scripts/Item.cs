using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Item : MonoBehaviour
{
	public int exp;
	public int fat;
	public GameObject feedFx;

	public void Use(PlayerController player)
	{
		if (player == null)
			return;

		player.Feed(exp, fat);
		if (feedFx)
			Instantiate(feedFx, transform.position, Quaternion.identity);
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
