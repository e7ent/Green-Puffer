using UnityEngine;
using System.Collections;
using DG.Tweening;
using Soomla.Profile;

public class GameManager : MonoSingleton<GameManager>
{
	public int generation = 0;
	public PlayerController player;
	public JoystickSystem joystick;
	public Transform endingPrefab;

	protected override void Awake()
	{
		Application.targetFrameRate = 60;
		DOTween.Init();
	}

	private void Start()
	{
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
