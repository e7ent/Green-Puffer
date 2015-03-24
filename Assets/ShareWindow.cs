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
	public GameObject loadingControl;

	private Window window;
	private RenderTexture renderTexture;
	private Texture2D texture;

	void Awake()
	{
		window = GetComponent<Window>();
	}

	void OnEnable()
	{
		var cam = Camera.main;

		if (renderTexture == null)
			renderTexture = new RenderTexture((int)captureImageRect.width, (int)captureImageRect.height, 0);

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

	public void Share(string providerName)
	{
		string sharePayload = "uploadImage";
		Provider shareProvider = null;

		switch (providerName)
		{
			default:
			case "Facebook":
				shareProvider = Provider.FACEBOOK;
				break;
			case "Twitter":
				shareProvider = Provider.TWITTER;
				break;
		}

		ProfileEvents.OnSocialActionStarted = (Provider provider, SocialActionType action, string payload) =>
		{
			if (payload != sharePayload)
				return;

			loadingControl.SetActive(true);
		};

		ProfileEvents.OnSocialActionFinished = (Provider provider, SocialActionType action, string payload) =>
		{
			if (payload != sharePayload)
				return;

			loadingControl.SetActive(false);
		};

		SoomlaProfile.UploadImage(shareProvider, messageInputField.text, "image.png", texture, sharePayload);
	}
}
