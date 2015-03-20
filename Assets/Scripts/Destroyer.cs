using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour
{
	public float lifeTime;
	public bool withFade = true;
	public float fadeDuration = 1;

	private bool destroyInProgress = false;

	IEnumerator Start()
	{
		if (lifeTime <= 0)
			yield break;
		
		yield return new WaitForSeconds(lifeTime);
		Destroy();
	}

	IEnumerator DisappearEffect()
	{

		if (withFade == false)
			yield break;
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		float elapsed = 0;

		while (true)
		{
			if ((elapsed += Time.deltaTime) > fadeDuration)
				break;

			for (int i = 0; i < renderers.Length; i++)
			{
				var color = renderers[i].material.color;
				color.a = 1.0f - (elapsed / fadeDuration);
				renderers[i].material.color = color;
			}
			yield return null;
		}

		Destroy(gameObject);
	}

	public void Destroy()
	{
		if (destroyInProgress) return;

		if (withFade)
			StartCoroutine(DisappearEffect());
		else
			Destroy(gameObject);

		destroyInProgress = true;
	}
}
