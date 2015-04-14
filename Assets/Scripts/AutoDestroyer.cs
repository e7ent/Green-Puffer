using UnityEngine;
using System.Collections;

public class AutoDestroyer : MonoBehaviour
{
	public float delayTime;

	public IEnumerator Start()
	{
		yield return new WaitForSeconds(delayTime);
		Destroy(gameObject);
	}
}
