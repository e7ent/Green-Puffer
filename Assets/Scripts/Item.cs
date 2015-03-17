using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Item : MonoBehaviour
{
	public int exp;
	public int fat;
	public float existTime = 10.0f;
	public GameObject feedFx;

	IEnumerator Start()
	{
		Destroy(gameObject, existTime);
		yield return new WaitForSeconds(existTime - 1);
		foreach (var renderer in GetComponentsInChildren<Renderer>())
			renderer.material.DOFade(0, 1);
	}

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
