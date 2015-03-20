using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set; }

	public int generation = 0;

	public PlayerController player;
	public JoystickSystem joystick;

	public Transform endingPrefab;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		Application.targetFrameRate = 60;
		DOTween.Init();

		SetPlayer(FindObjectOfType<PlayerController>());
		FadeManager.FadeIn();
	}

	public void SetPlayer(PlayerController player)
	{
		if (player == null)
			Debug.LogError("Arg is null");
		this.player = player;
		joystick.valueChange.RemoveAllListeners();
		joystick.valueChange.AddListener(player.Move);
	}

	public void EndGame()
	{
		Instantiate(endingPrefab);
	}
}
