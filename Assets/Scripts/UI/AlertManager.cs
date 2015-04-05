using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertManager : MonoSingleton<AlertManager>
{
	public Window window;
	public Text message;

	System.Action onFinish = null;

    void OnEnable()
    {
        if (!window.IsVisible())
            gameObject.SetActive(false);
    }


	public void ShowAlert(string message, System.Action onFinish = null)
	{
		this.onFinish = onFinish;
		this.message.text = message;
		window.Show();
		gameObject.SetActive(true);
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
