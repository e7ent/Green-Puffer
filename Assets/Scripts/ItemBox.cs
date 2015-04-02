using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ItemBox : MonoBehaviour
{
	public Transform acquireFx;
	public float attackThreshold = 2;

	private bool isOpend = false;
	private bool isUsed = false;

	public void Touch(GameObject target)
	{
		if (isOpend == false)
		{
			var rigidbody = target.GetComponent<Rigidbody2D>();
			if (rigidbody != null)
				rigidbody.velocity = (target.transform.position - transform.position).normalized * 2;

			transform.DOPunchScale(Vector3.one * .2f, .5f).SetEase(Ease.Linear);
			transform.DOPunchRotation(new Vector3(0, 0, 5), .5f).SetEase(Ease.Linear);
			GetComponent<Animator>().SetTrigger("Open");
			foreach (var collider in GetComponentsInChildren<Collider2D>())
				collider.isTrigger = true;
			isOpend = true;
		}
		else if (isOpend == true && isUsed == false)
		{
			transform.DOPunchScale(Vector3.one * .2f, .5f).SetEase(Ease.Linear);
			transform.DOPunchRotation(new Vector3(0, 0, 5), .5f).SetEase(Ease.Linear);

			if (acquireFx != null)
				Instantiate(acquireFx, transform.position, Quaternion.identity);
			target.GetComponent<PlayerController>().Eat(10, 0);
			GetComponent<Animator>().SetBool("Used", true);
			GetComponent<Destroyer>().Destroy();
			isUsed = true;
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Player") == false) return;
		Touch(col.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Player") == false) return;
		Touch(col.gameObject);
	}
}
