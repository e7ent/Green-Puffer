using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusView : MonoBehaviour
{
	public RectTransform expBar;
	public Image rankImage;
	public Text generationText;
	public Image injuryImage;

	private PlayerController puffer;
	private float originExpBarWidth;

	void Start()
	{
		originExpBarWidth = expBar.sizeDelta.x;
	}

	void Update()
	{
		if (puffer == null)
		{
			puffer = FindObjectOfType<PlayerController>();
			if (puffer == null)
				return;
		}

		var expBarSize = expBar.sizeDelta;
		expBarSize.x = Mathf.Clamp((float)puffer.stat.exp / puffer.stat.maxExp * originExpBarWidth, 0, originExpBarWidth);
		expBar.sizeDelta = expBarSize;
	}
}
