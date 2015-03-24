using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Soomla.Profile;

public class ShareWindow : MonoBehaviour
{
	public InputField messageInputField;
	public RawImage rawImageControl;
	public Rect captureImageRect;

	private RenderTexture renderTexture;
	private Texture2D texture;

	void OnEnable()
	{
		var cam = Camera.main;

		if (renderTexture == null)
			renderTexture = new RenderTexture((int)captureImageRect.width, (int)captureImageRect.height, 24);

		if (texture == null)
		{
			texture = new Texture2D((int)captureImageRect.width, (int)captureImageRect.height);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
		}

		cam.targetTexture = renderTexture;
		cam.Render();

		RenderTexture.active = renderTexture;
		texture.ReadPixels(captureImageRect, 0, 0);
		texture.Apply();

		cam.targetTexture = null;

		if (rawImageControl.texture != texture)
			rawImageControl.texture = texture;
	}

	public void Share()
	{
		var window = GetComponent<Window>();
		SoomlaProfile.UploadImage(Provider.FACEBOOK, messageInputField.text, "image.png", texture, "TESTasdf");
		ProfileEvents.OnSocialActionFinished = (Provider provider, SocialActionType action, string payload) =>
		{
			window.Close();
		};
	}
}
