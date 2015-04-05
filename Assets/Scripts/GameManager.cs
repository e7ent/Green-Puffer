﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using DG.Tweening;
using Soomla.Profile;

public class GameManager : MonoSingleton<GameManager>
{
    public int currency = 0;
    public int generation = 1;

    public PlayerController[] playerPrefabs;
    public GameObject[] backgroundPrefabs;
    public GameObject endingPrefab;

    private PlayerController player;
    private GameObject endingObject;

    private bool isFinished = false;
    private bool isPaused = false;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        DOTween.Init();

        // Load Currency
        currency = PlayerPrefs.GetInt("currency", 1000);

        LoadPlayer();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SavePlayer();
        PlayerPrefs.SetInt("currency", currency);
    }

    private void Start()
    {
        FadeManager.FadeIn();
    }

    public void LoadPlayer()
    {
        string savedPlayerData = PlayerPrefs.GetString("player", string.Empty);

        if (string.IsNullOrEmpty(savedPlayerData))
        {
            CreatePlayer(PlayerController.RankType.Baby);
            return;
        }

        using (XmlReader reader = XmlTextReader.Create(new StringReader(savedPlayerData)))
        {
                reader.ReadStartElement("player");

                var id = reader.ReadElementString("id");
                PlayerController findPrefab = null;
                foreach (var prefab in playerPrefabs)
                {
                    if (prefab.Id == id)
                    {
                        findPrefab = prefab;
                        break;
                    }
                }

                CreatePlayer(findPrefab.Id);

                player.Hp = float.Parse(reader.ReadElementString("hp"));
                player.Exp = float.Parse(reader.ReadElementString("exp"));
                player.Satiety = float.Parse(reader.ReadElementString("satiety"));

                reader.ReadEndElement();
        }
    }

    public void SavePlayer()
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "\t";
        settings.NewLineOnAttributes = true;

        StringBuilder sb = new StringBuilder();
        using (XmlWriter writer = XmlTextWriter.Create(sb, settings))
        {
            writer.WriteStartElement("player");
            writer.WriteElementString("id", player.Id);
            writer.WriteElementString("hp", player.Hp.ToString());
            writer.WriteElementString("exp", player.Exp.ToString());
            writer.WriteElementString("satiety", player.Satiety.ToString());
            writer.WriteEndElement();
        }
        PlayerPrefs.SetString("player", sb.ToString());
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
        var newPlayer = GameObject.Instantiate(prefab) as PlayerController;
        if (player != null)
        {
            newPlayer.transform.position = player.transform.position;
            newPlayer.transform.rotation = player.transform.rotation;
            PlayerLibraryManager.instance.Unlock(newPlayer.Id);
            player.Destroy();
        }
        player = newPlayer;
    }

    public void Finish(bool isRebirth = false)
    {
        if (isFinished) return;
        isFinished = true;

        Pause();

        if (player.Hp <= 0)
        {
            EndingLibraryManager.instance.Unlock(EndingLibraryManager.Type.Hurt);
        }
        else if (isRebirth)
        {
            EndingLibraryManager.instance.Unlock(EndingLibraryManager.Type.Rebirth);
        }

        endingObject = GameObject.Instantiate(endingPrefab) as GameObject;
        FadeManager.instance.Fade(Color.clear, new Color(0, 0, 0, .7f), 1, () =>
        {
            MessageManager.instance.Open();
            MessageManager.instance.PushMessage(LocalizationString.GetStrings("death"));
            MessageManager.instance.onConfirm += () =>
            {
                GameManager.instance.Retry();
            };
        });
    }

    public void Retry()
    {
        GameObject.Destroy(endingObject);
        CreatePlayer(PlayerController.RankType.Baby);
        FadeManager.FadeIn();
        isFinished = false;
        Resume();
    }
}
