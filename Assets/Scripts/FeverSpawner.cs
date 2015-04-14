using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FeverSpawner : MonoBehaviour
{
	public GameObject spawnFx;
	public List<GameObject> prefabs;
	public float createTime;
	public Rect spawnRect;

	private float elapsed = 0;

	private void Update()
	{
		if ((elapsed += Time.deltaTime) < createTime) return;
		elapsed = 0;

		var newObject = Create(Random.Range(0, prefabs.Count));
		//newObject.transform.parent = transform;
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
		obj.transform.DOPunchScale(Vector3.one * 0.35f, 0.3f);
		return obj;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(spawnRect.center, spawnRect.size);
	}
}
