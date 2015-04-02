using UnityEngine;
using System.Collections;
using DG.Tweening;
using Soomla.Profile;

public class GameManager : MonoSingleton<GameManager>
{
	public int currency = 0;
	public int generation = 1;
	public UpgradeData[] upgrades;

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

	public bool Upgrade(string name)
	{
		UpgradeData upgrade = null;
		foreach (var item in upgrades)
		{
			if (item.name != name)
				continue;
			upgrade = item;
			break;
		}

		if ((currency - upgrade.NextRequiredCurrency()) < 0)
			return false;

		currency -= upgrade.NextRequiredCurrency();
		upgrade.Upgrade();

		return true;
	}

	public int GetUpgradeLevel(string name)
	{
		UpgradeData upgrade = null;
		foreach (var item in upgrades)
		{
			if (item.name != name)
				continue;
			upgrade = item;
			break;
		}
		
		if (upgrade == null)
			return 0;

		return upgrade.currentLevel;
	}
}
