using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Profile;

public class SceneLoader : MonoBehaviour
{
	public bool autoLoad = false;
	public string sceneName;

	public Slider progressControl;

	[SerializeField]
	private float progressValue = 0;

	void Start()
	{
		if (autoLoad)
			LoadScene();
	}

	void Update()
	{
		progressControl.value = progressValue;
	}

	public void LoadScene()
	{
		if (progressControl != null)
			StartCoroutine(LoadSceneWithProgressBar());
		else
			Application.LoadLevel(sceneName);
	}

	private IEnumerator LoadSceneWithProgressBar()
	{
		bool profileInit = false;
		ProfileEvents.OnSoomlaProfileInitialized = () => profileInit = true;
		SoomlaProfile.Initialize();
		while (profileInit != true)
			yield return null;

		progressValue = .3f;

		SoomlaProfile.Login(Provider.FACEBOOK);
		SoomlaProfile.Login(Provider.TWITTER);
		while (SoomlaProfile.IsLoggedIn(Provider.FACEBOOK) != true)
			yield return null;

		while (SoomlaProfile.IsLoggedIn(Provider.TWITTER) != true)
			yield return null;

		progressValue = .6f;

		var op = Application.LoadLevelAsync(sceneName);
		while (!op.isDone)
		{
			progressValue = .6f + (op.progress * 0.3f);
			yield return null;
		}
	}
}
