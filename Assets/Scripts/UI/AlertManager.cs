using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertManager : MonoSingleton<AlertManager>
{
	public Window window;
	public Text message;

	System.Action onFinish = null;

	public void ShowAlert(string message, System.Action onFinish = null)
	{
		this.onFinish = onFinish;
		gameObject.SetActive(true);
		this.message.text = message;
		window.Show();
	}

	public void Ok()
	{
		window.Close(() =>
		{
			gameObject.SetActive(false);
			if (onFinish != null)
				onFinish();
		});
	}
}
