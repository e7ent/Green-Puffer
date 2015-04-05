using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Spawner))]
public class FoodSpawner : MonoBehaviour
{
	[SerializeField]
	private int increaseCount = 2;
	[SerializeField]
	private string upgradeName;

	private Spawner spawner;

	private void Awake()
	{
		spawner = GetComponent<Spawner>();
	}

	private void Update()
	{
		spawner.MaxCount = UpgradeSystem.instance.GetLevel(upgradeName) * increaseCount;
	}
}
