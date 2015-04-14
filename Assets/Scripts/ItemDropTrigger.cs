using UnityEngine;
using System.Collections;

public class ItemDropTrigger : MonoBehaviour
{
	[SerializeField]
	private GameObject[] coinPrefabs;
	[SerializeField]
	private float coinDropRate = 0;		// 0 ~ 1
	[SerializeField]
	private GameObject[] itemPrefabs;
	[SerializeField]
	private int dropCount;

	public int DropCount
	{
		get { return dropCount; }
		set { dropCount = value; }
	}

	public void Drop(PlayerController player = null)
	{
		do
		{
			if (itemPrefabs.Length <= 0)
				break;

			for (int i = 0; i < this.DropCount; i++)
			{
				var randomForce = Random.insideUnitCircle;
				var createdItem = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)],
					transform.position, Quaternion.identity) as GameObject;

				randomForce.y = Mathf.Abs(randomForce.y);
				createdItem.GetComponent<Rigidbody2D>().AddForce(randomForce, ForceMode2D.Impulse);
				createdItem.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1, 1), ForceMode2D.Impulse);
			}
		} while (false);

		do
		{
			if (coinPrefabs.Length <= 0)
				break;

			float dropRate = coinDropRate;
			if (player != null)
				dropRate += player.Luck;
			else
				dropRate += FindObjectOfType<PlayerController>().Luck;

			int rand = Random.Range(0, 100);
			if (rand > (dropRate * 100))
				break;

			for (int i = 0; i < rand; i++)
			{
				var randomForce = Random.insideUnitCircle;
				var createdItem = Instantiate(coinPrefabs[Random.Range(0, coinPrefabs.Length)],
					transform.position, Quaternion.identity) as GameObject;

				randomForce.y = Mathf.Abs(randomForce.y);
				createdItem.GetComponent<Rigidbody2D>().AddForce(randomForce, ForceMode2D.Impulse);
				createdItem.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1, 1), ForceMode2D.Impulse);
			}
		} while (false);
	}
}
