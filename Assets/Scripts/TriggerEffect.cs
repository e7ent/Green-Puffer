using UnityEngine;
using System.Collections;

public class TriggerEffect : MonoBehaviour {

	public float lifeTime = 1;
	public float upSpeed = 0.3f;

	private float elapsed = 0;
	private Renderer[] renderers;

	void Start()
	{
		renderers = GetComponentsInChildren<Renderer>();
		Destroy(gameObject, lifeTime);
	}

	void Update()
	{
		float rate = (lifeTime - elapsed) / lifeTime;
		for (int i = 0; i < renderers.Length; i++ )
		{
			var color = renderers[i].material.color;
			color.a = rate;
			renderers[i].material.color = color;
		}
		transform.Translate(transform.up * upSpeed * Time.deltaTime);
		elapsed += Time.deltaTime;
	}
}
