using UnityEngine;
using System.Collections;

public class Killer : MonoBehaviour
{
	[SerializeField]
	private int strength;


	private void OnCollisionEnter2D(Collision2D other)
	{
		var player = other.gameObject.GetComponent<PlayerController>();
		if (player != null)
			player.Hurt(strength);
	}
}
