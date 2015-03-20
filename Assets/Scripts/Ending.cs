using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Ending : UIBehaviour, IPointerClickHandler
{
	public string message;

	[SerializeField]
	private Text messageText;

	private bool effectDone = false;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(TypeWriterEffect());
	}

	IEnumerator TypeWriterEffect()
	{
		for (int i = 0; i <= message.Length; i++)
		{
			messageText.text = message.Substring(0, i);
			yield return StartCoroutine(WaitForRealSeconds(.25f));
		}
		effectDone = true;
	}

	IEnumerator WaitForRealSeconds(float seconds)
	{
		var start = Time.unscaledTime + seconds;
		while (Time.unscaledTime < start)
			yield return null;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!effectDone) return;
		FadeManager.FadeOut(1, () =>
		{
			Application.LoadLevel("Game");
			Time.timeScale = 1;
		});
	}
}
