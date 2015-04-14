using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MessageManager : MonoSingleton<MessageManager>, UnityEngine.EventSystems.IPointerClickHandler
{
    public System.Action onConfirm;

    [SerializeField]
    private RectTransform messgaeBox;
    [SerializeField]
    private Text textControl;

    private string currentMessage;
    private Queue<string> messageQueue = new Queue<string>();

    private bool isWriting = false;
    private bool isVisible = false;

    protected override void Awake()
    {
        base.Awake();
        Close(true);
    }

    private void OnEnable()
    {
        if (!isVisible)
            gameObject.SetActive(false);
    }

    public MessageManager Open(bool withEffect = true)
    {
        if (isVisible) return this;
        isVisible = true;

        gameObject.SetActive(true);
        GetComponent<Canvas>().enabled = true;
        messgaeBox.transform.localScale = Vector3.zero;
        textControl.text = "";

        if (withEffect)
            messgaeBox.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);

		DisplayNextMessage();

        return this;
    }

    public MessageManager Close(bool withEffect = true)
    {
        if (!isVisible) return this;
        isVisible = false;

        if (withEffect)
        {
            messgaeBox.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    GetComponent<Canvas>().enabled = false;

                    if (onConfirm != null)
                        onConfirm();
                });
        }
        else
        {
            gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = false;

            if (onConfirm != null)
                onConfirm();
        }

        return this;
    }

    public MessageManager PushMessage(string message)
    {
        this.messageQueue.Enqueue(message);
        if (!isWriting && string.IsNullOrEmpty(textControl.text))
            DisplayNextMessage();
        return this;
    }

    public MessageManager PushMessage(string[] message)
    {
        for (int i = 0; i < message.Length; i++)
            PushMessage(message[i]);
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
            yield return StartCoroutine(WaitForRealSeconds(.15f));
        }
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
            return;

        if (HasNextMessage())
            DisplayNextMessage();
        else
            Close();
    }
}
