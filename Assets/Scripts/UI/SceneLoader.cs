using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
	public bool autoLoad = false;
	public string sceneName;

	public Slider progressControl;

	void Start()
	{
		if (autoLoad)
			LoadScene();
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
		var op = Application.LoadLevelAsync(sceneName);
		while (!op.isDone)
		{
			progressControl.value = op.progress;
			yield return null;
		}
	}
}
