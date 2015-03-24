using UnityEngine;
using System.Collections;
using DG.Tweening;
using Soomla.Profile;

public class GameManager : MonoSingleton<GameManager>
{
	public int generation = 0;

	private bool isPaused = false;

	protected override void Awake()
	{
		base.Awake();
		Application.targetFrameRate = 60;
		DOTween.Init();
		DontDestroyOnLoad(this);
	}

	private void Start()
	{
		FadeManager.FadeIn();
	}

	public void Pause()
	{
		if (isPaused)
			return;

		isPaused = true;
		Time.timeScale = 0.00001f;
	}

	public void Resume()
	{
		if (!isPaused)
			return;

		isPaused = false;
		Time.timeScale = 1;
	}

	public bool IsPaused()
	{
		return isPaused;
	}
}
