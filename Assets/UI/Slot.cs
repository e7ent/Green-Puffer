using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Slot : UIBehaviour
{
	public GameObject[] slots;

	public int Value
	{
		get { return this.value; }
		set
		{
			this.value = value;

			for (int i = 0; i < slots.Length; i++)
				slots[i].SetActive(i < this.value);
		}
	}

	private int value;
}
