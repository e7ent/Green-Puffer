using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour
{
	public float lifeTime;
	public bool withFade = true;

	private float elapsed = 0;
	private Renderer[] renderers;

	void Start()
	{
		renderers = GetComponents<Renderer>();
	}

	void Update()
	{
		if ((elapsed += Time.deltaTime) < lifeTime)
			return;

		if (withFade)
		{
			if (elapsed >= lifeTime + 1)
				Destroy(gameObject);

			for (int i = 0; i < renderers.Length; i++)
			{
				var color = renderers[i].material.color;
				color.a = 1.0f - (elapsed - lifeTime);
				renderers[i].material.color = color;
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
