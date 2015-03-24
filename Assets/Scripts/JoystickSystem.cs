using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class JoystickSystem : MonoSingleton<JoystickSystem>
{
	[System.Serializable]
	public class JoystickEvent : UnityEvent<Vector2> { }

	public float dragThreshold = 50;
	public float radius = 100;
	public RectTransform joystick;
	public RectTransform defaultImage;
	public RectTransform dragImage;
	public RectTransform elasticImage;
	public JoystickEvent valueChange;

	private Vector2 startPosition;
	private bool wasTouched = false;
	private bool isDrag = false;

	void Start()
	{
		Input.simulateMouseWithTouches = true;
		CleanUp();
	}

	void Update()
	{
		if (Time.timeScale <= 0.001f) return;
		bool isTouched = Input.GetMouseButton(0) && !(!wasTouched && EventSystem.current.IsPointerOverGameObject(Input.touchCount > 0 ? 0 : -1));
		if (wasTouched != isTouched)
		{
			if (isTouched)
				Setup();
			else
				CleanUp();
		}
		else
		{
			if (!isTouched) return;
			UpdateDrag();
		}
		wasTouched = isTouched;
	}

	private void Setup()
	{
		startPosition = GetTouchPosition();
		joystick.position = GetTouchPosition();
		joystick.DOKill(true);
		SetVisible(true);
		joystick.localScale = Vector3.one;
		joystick.DOPunchScale(Vector3.one * .3f, 0.1f);
	}

	private void CleanUp()
	{
		defaultImage.gameObject.SetActive(true);
		dragImage.gameObject.SetActive(false);
		isDrag = false;
		joystick.DOPunchScale(Vector3.one * -.3f, .1f).OnComplete(() =>
		{
			SetVisible(false);
		});
	}

	private void SetVisible(bool visible)
	{
		joystick.gameObject.SetActive(visible);
	}

	private void SetDragMode(bool mode)
	{
		defaultImage.gameObject.SetActive(!mode);
		dragImage.gameObject.SetActive(mode);
		joystick.DOKill(true);
		joystick.DOPunchScale(Vector3.one * .3f, 0.1f);
		isDrag = mode;
	}

	private void UpdateDrag()
	{
		Vector2 newPos = GetTouchPosition();
		Vector2 diff = newPos - startPosition;
		float distance = diff.magnitude;

		if (distance <= dragThreshold)
		{
			if (isDrag)
				SetDragMode(false);
			//return;
		}
		if (distance > dragThreshold && !isDrag)
			SetDragMode(true);

		distance = Mathf.Clamp(distance, 0, radius);

		Vector2 dir = diff.normalized;

		if (valueChange != null)
			valueChange.Invoke(dir * (distance / radius));

		dragImage.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
		var size = elasticImage.sizeDelta;
		size.x = distance;
		elasticImage.sizeDelta = size;
	}

	private static Vector2 GetTouchPosition()
	{
		return Input.mousePosition;
	}
}
