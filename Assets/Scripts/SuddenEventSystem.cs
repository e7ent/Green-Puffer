using UnityEngine;
using System.Collections;

public class SuddenEventSystem : MonoBehaviour
{
	public GameObject[] survivalPrefabs;
	public GameObject[] feverPrefabs;

	public float min, max;

	IEnumerator Start()
	{
		while (true)
		{
			float randomTime = Random.Range(min, max);
			yield return new WaitForSeconds(randomTime);

			bool flag = Random.Range(0, 2) == 0;
			GameObject prefab = null;
			if (flag)
			{
				prefab = survivalPrefabs[Random.Range(0, survivalPrefabs.Length)];
			}
			else
			{
				prefab = feverPrefabs[Random.Range(0, feverPrefabs.Length)];
			}

			GameObject.Instantiate(prefab);
		}
	}
}
