using UnityEngine;
using System.Collections;

public class ItemDropTrigger : MonoBehaviour
{
	[SerializeField]
	private GameObject[] itemPrefabs;
	[SerializeField]
	private int dropCount;

	public int DropCount
	{
		get { return dropCount; }
		set { dropCount = value; }
	}

	public void Drop()
	{
		if (itemPrefabs.Length <= 0)
			return;

		for (int i = 0; i < this.DropCount; i++)
		{
			var randomForce = Random.insideUnitCircle;
			var createdItem = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)],
				transform.position, Quaternion.identity) as GameObject;

			randomForce.y = Mathf.Abs(randomForce.y);
			createdItem.GetComponent<Rigidbody2D>().AddForce(randomForce, ForceMode2D.Impulse);
			createdItem.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1, 1), ForceMode2D.Impulse);
		}
	}
}
