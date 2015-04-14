using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using DG.Tweening;
using E7;

public class GameManager : MonoSingleton<GameManager>
{
	public enum FinishType
	{
		Dead,
		Rebirth,
		RankUp,
	}

	[SerializeField]
	private int currency;
    public int Currency
	{
		get { return currency; }
		set
		{
			currency = value;
			PlayerPrefs.SetInt("currency", currency);
		}
	}
    public int generation = 1;

    public GameObject createEffect;
    public PlayerController[] playerPrefabs;
    public GameObject[] backgroundPrefabs;
	public GameObject[] finishPrefabs;

    private bool isFinished = false;
    private bool isPaused = false;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        DOTween.Init();
		UM_AdManager.instance.Init();

		currency = PlayerPrefs.GetInt("currency", 10000);

		var id = PlayerPrefs.GetString("player_id", string.Empty);
		if (string.IsNullOrEmpty(id))
		{
			CreatePlayer(PlayerController.RankType.Baby);
		}
		else
		{
			CreatePlayer(id);
		}

		var player = PlayerController.instance;
		player.Hp = PlayerPrefs.GetFloat("player_hp", player.MaxHp);
		player.Exp = PlayerPrefs.GetFloat("player_exp", 0);
		player.Satiety = PlayerPrefs.GetFloat("player_satiety", 0);
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

    public void CreatePlayer(PlayerController.RankType rank)
    {
        List<PlayerController> prefabs = new List<PlayerController>();
        foreach (var item in playerPrefabs)
        {
            if (item.Rank == rank)
                prefabs.Add(item);
        }
        CreatePlayer(prefabs[Random.Range(0, prefabs.Count)].Id);
    }

    public void CreatePlayer(string id)
    {
        PlayerController prefab = null;
        foreach (var item in playerPrefabs)
        {
            if (item.Id == id)
            {
                prefab = item;
                break;
            }
        }

		if (PlayerController.instance != null)
		{
			PlayerController.instance.Destroy();
			PlayerLibraryManager.instance.Unlock(prefab.Id);
		}

        var newPlayer = GameObject.Instantiate(prefab) as PlayerController;
        GameObject.Instantiate(createEffect, newPlayer.transform.position, Quaternion.identity);

		PlayerPrefs.SetString("player_id", id);
    }

    public void Finish(FinishType type = FinishType.Dead)
    {
        if (isFinished) return;
        isFinished = true;

        Pause();

		PlayerController player = PlayerController.instance;
		GameObject finish = GameObject.Instantiate(finishPrefabs[(int)type]);
		string[] messages = null;

		switch (type)
		{
			case FinishType.Dead:
				if (player.Hp <= 0)
					EndingLibraryManager.instance.Unlock(EndingLibraryManager.Type.Hurt);
				messages = Localization.GetStringArray("death");
				MessageManager.instance.onConfirm += OnDeadFinish;
				break;


			case FinishType.Rebirth:
				EndingLibraryManager.instance.Unlock(EndingLibraryManager.Type.Rebirth);
				messages = Localization.GetStringArray("rebirth");
				MessageManager.instance.onConfirm += OnRebirthFinish;
				break;


			case FinishType.RankUp:
				messages = Localization.GetStringArray("rankup");
				MessageManager.instance.onConfirm += OnRankUpFinish;
				break;


			default:
				break;
		}

		MessageManager.instance.PushMessage(messages);
		MessageManager.instance.onConfirm += () =>
		{
			if (finish)
				GameObject.Destroy(finish);
		};

		FadeManager.instance.Fade(Color.clear, new Color(0, 0, 0, .7f), 1, () =>
		{
			MessageManager.instance.Open();
        });

		UM_AdManager.instance.StartInterstitialAd();
    }

	private void OnDeadFinish()
	{
		PlayerController player = PlayerController.instance;

		Currency += ((int)player.Rank * 100) + (int)player.Exp;
		isFinished = false;

		CreatePlayer(PlayerController.RankType.Baby);

		FadeManager.FadeIn();
		Resume();
	}

	private void OnRebirthFinish()
	{
		OnDeadFinish();
	}

	private void OnRankUpFinish()
	{
		CreatePlayer(PlayerController.instance.Rank + 1);
		PlayerController.instance.transform.position = Vector3.zero;
		FadeManager.FadeIn();
		isFinished = false;
		Resume();
	}
}
