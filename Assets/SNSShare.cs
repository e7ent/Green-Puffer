using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Soomla.Profile;

public class SNSShare : MonoBehaviour, IPointerClickHandler
{
	private RenderTexture renderTexture;

	void Start()
	{
		SoomlaProfile.Initialize();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!SoomlaProfile.IsLoggedIn(Provider.FACEBOOK))
			SoomlaProfile.Login(Provider.FACEBOOK);

		var imageWidth = 1008;
		var imageHeight = 502;

		if (renderTexture == null)
			renderTexture = new RenderTexture(504, 251, 24);

		var cam = Camera.main;
		var texture = new Texture2D(504, 251);
		
		cam.targetTexture = renderTexture;
		cam.Render();
		
		RenderTexture.active = renderTexture;
		texture.ReadPixels(Rect.MinMaxRect(0, 0, 504, 251), 0, 0);
		texture.Apply();
		
		SoomlaProfile.UploadImage(Provider.FACEBOOK, "", "image.png", texture);
		cam.targetTexture = null;

	}
}
