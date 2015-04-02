using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeechBubbleManager : MonoSingleton<SpeechBubbleManager>
{
	public List<SpeechBubble> prefabs;

	public SpeechBubble CreateBubble(int prefabIndex, string message)
	{
		var bubble = Instantiate(prefabs[prefabIndex]) as SpeechBubble;
		bubble.SetMessage(message);
		return bubble;
	}
}
