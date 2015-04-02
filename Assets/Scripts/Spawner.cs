using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Spawner : MonoBehaviour
{
	public class SpawnerMark : MonoBehaviour
	{
		[HideInInspector]
		public Spawner owner;

		internal void Mark(Spawner owner)
		{
			this.owner = owner;
			owner.spawned.Add(this);
		}

		void OnDestroy()
		{
			if (owner)
				owner.spawned.Remove(this);
		}
	}
	public GameObject spawnFx;
	public List<GameObject> prefabs;
	public float createTime;
	public Rect spawnRect;

	[SerializeField]
	private int maxCount = 10;

	public int MaxCount
	{
		get { return maxCount; }
		set { maxCount = value; }
	}

	private List<SpawnerMark> spawned = new List<SpawnerMark>();
	private float elapsed = 0;

	private void Update()
	{
		if (GetCreatedCount() >= maxCount) return;

		if ((elapsed += Time.deltaTime) < createTime) return;
		elapsed = 0;
		
		var newObject = Create(Random.Range(0, prefabs.Count));
		newObject.transform.parent = transform;
		newObject.transform.position = spawnRect.center +
			new Vector2(
				Random.Range(spawnRect.size.x * -.5f, spawnRect.size.x * .5f),
				Random.Range(spawnRect.size.y * -.5f, spawnRect.size.y * .5f)
				);

		if (spawnFx)
			Instantiate(spawnFx, newObject.transform.position, Quaternion.identity);
	}

	public GameObject Create(int index)
	{
		var obj = Instantiate(prefabs[index]) as GameObject;
		obj.AddComponent<SpawnerMark>().Mark(this);
		obj.transform.DOPunchScale(Vector3.one * 0.35f, 0.3f);
		return obj;
	}

	public int GetCreatedCount()
	{
		return spawned.Count;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(spawnRect.center, spawnRect.size);
	}
}
