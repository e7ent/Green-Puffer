using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertManager : MonoSingleton<AlertManager>
{
	public Window window;
	public Text message;

	public Button yesButton, noButton;

	System.Action<bool> onFinish = null;

    void OnEnable()
    {
        if (!window.IsVisible())
            gameObject.SetActive(false);
    }


	public void ShowAlert(string message, bool activeNo = false, System.Action<bool> onFinish = null)
	{
		this.onFinish = onFinish;
		this.message.text = message;
		window.Show();
		gameObject.SetActive(true);

		noButton.gameObject.SetActive(activeNo);
	}

	public void Yes()
	{
		window.Close(() =>
		{
			gameObject.SetActive(false);
			if (onFinish != null)
				onFinish(true);
		});
	}

	public void No()
	{
		window.Close(() =>
		{
			gameObject.SetActive(false);
			if (onFinish != null)
				onFinish(false);
		});
	}
}
