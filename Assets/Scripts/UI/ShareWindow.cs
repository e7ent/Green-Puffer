using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShareWindow : MonoBehaviour
{
	public InputField messageInputField;
	public RawImage rawImageControl;

	private RenderTexture renderTexture;
	private Texture2D texture;

	void OnEnable()
	{
		var cam = Camera.main;

		float aspectRatio = 9.0f / 16.0f;
		int width = Screen.width;
		int height = (int)(width * aspectRatio);

		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(width, height, 0);
			renderTexture.useMipMap = false;
		}

		if (texture == null)
		{
			texture = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
		}

		cam.targetTexture = renderTexture;
		cam.Render();

		RenderTexture.active = renderTexture;

		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.width * aspectRatio), 0, 0);
		texture.Apply();

		cam.targetTexture = null;

		if (rawImageControl.texture != texture)
			rawImageControl.texture = texture;
	}

	public void Share(string providerName)
	{
		var message = E7.Localization.GetString("share_message") + messageInputField.text;

		switch (providerName)
		{
			default:
			case "Facebook":
				UM_ShareUtility.FacebookShare(message, texture);
				break;
			case "Twitter":
				UM_ShareUtility.TwitterShare(message, texture);
				break;
		}
	}
}
