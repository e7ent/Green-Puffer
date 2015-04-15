using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	[SerializeField]
	private string sceneName;
	[SerializeField]
	private bool autoStart = true;
	[SerializeField]
	private bool allowSceneActivation = true;
	[SerializeField]
	private Slider progressBar;
	[SerializeField]
	private Text progressText;

	private AsyncOperation asyncOperation = null;

	public bool AllowSceneActivation
	{
		get { return allowSceneActivation; }
		set
		{
			allowSceneActivation = value;
			if (asyncOperation != null)
				asyncOperation.allowSceneActivation = value;
		}
	}

	public float Progress
	{
		get
		{
			if (asyncOperation == null)
				return 0;

			return asyncOperation.progress + (allowSceneActivation ? 0.1f : 0);
		}
	}

	void Start()
	{
		if (autoStart)
			LoadLevel();
	}

	public void LoadLevel()
	{
		StartCoroutine(coLoadLevel());
	}

	private IEnumerator coLoadLevel()
	{
		asyncOperation = Application.LoadLevelAsync(sceneName);
		asyncOperation.allowSceneActivation = allowSceneActivation;

		while (!asyncOperation.isDone)
		{
			float progress = asyncOperation.progress;
			if (allowSceneActivation)
				progress += 0.1f;

			if (progressBar)
				progressBar.value = progress;

			if (progressText)
				progressText.text = string.Format("{0} %", (int)(progress * 100));

			yield return null;
		}
	}
}
