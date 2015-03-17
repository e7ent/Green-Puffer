using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set; }

	public int generation = 0;

	public PlayerController puffer;
	public JoystickSystem joystick;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		Application.targetFrameRate = 60;
		DOTween.Init();

		SetPuffer(FindObjectOfType<PlayerController>());
	}

	public void SetPuffer(PlayerController puffer)
	{
		if (puffer == null)
			Debug.LogError("Arg is null");
		this.puffer = puffer;
		joystick.valueChange.RemoveAllListeners();
		joystick.valueChange.AddListener(puffer.Move);
	}
}
