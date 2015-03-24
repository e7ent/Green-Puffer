using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoSingleton<UIManager>
{
	private GraphicRaycaster raycaster;

	void Awake()
	{
		raycaster = GetComponent<GraphicRaycaster>();
	}
}
