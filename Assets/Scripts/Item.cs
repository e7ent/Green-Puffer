using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Item : MonoBehaviour
{
	public int exp;
	public int fat;
	public GameObject feedFx;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player") == false)
			return;

		other.GetComponent<PlayerController>().Feed(exp, fat);
		if (feedFx)
			Instantiate(feedFx, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
