using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MessageManager : MonoSingleton<MessageManager>, UnityEngine.EventSystems.IPointerClickHandler
{
	public delegate void MessageCallback();

	public MessageCallback onConfirm;

	[SerializeField]
	private Text textControl;

	private string currentMessage;
	private Queue<string> messageQueue = new Queue<string>();

	private bool isWriting = false;

	protected override void Awake()
	{
		base.Awake();
		Close(true);
	}

	public MessageManager Open(bool withEffect = true)
	{
		transform.localScale = Vector3.zero;
		textControl.text = "";

		if (withEffect)
			transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
		return this;
	}

	public MessageManager Close(bool withEffect = true)
	{
		if (withEffect)
			transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack);
		return this;
	}

	public MessageManager PushMessage(string message)
	{
		this.messageQueue.Enqueue(message);
		return this;
	}

	public MessageManager PushMessage(string[] message)
	{
		for (int i = 0; i < message.Length; i++)
			this.messageQueue.Enqueue(message[i]);
		return this;
	}

	public int GetMessagesCount()
	{
		return messageQueue.Count;
	}

	public bool DisplayNextMessage()
	{
		if (messageQueue.Count <= 0) return false;
		currentMessage = messageQueue.Dequeue();
		StartCoroutine(TypeWriterEffect());
		return true;
	}

	public bool HasNextMessage()
	{
		if (GetMessagesCount() <= 0) return false;
		return true;
	}

	private IEnumerator TypeWriterEffect()
	{
		isWriting = true;
		for (int i = 0; i <= currentMessage.Length; i++)
		{
			textControl.text = currentMessage.Substring(0, i);
			yield return StartCoroutine(WaitForRealSeconds(.25f));
		}
	}

	private void StopTypeWriterEffect()
	{
		StopCoroutine("TypeWriterEffect");
		textControl.text = currentMessage;
		isWriting = false;
	}

	private IEnumerator WaitForRealSeconds(float seconds)
	{
		var cur = Time.unscaledTime + seconds;
		while (Time.unscaledTime < cur)
			yield return null;
	}

	public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (isWriting)
			StopTypeWriterEffect();
		else if (HasNextMessage())
			DisplayNextMessage();
		else
			Close();
	}
}
