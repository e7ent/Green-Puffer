using UnityEngine;
using System.Collections;

public class ItemSlot : MonoBehaviour
{
    public void Use()
    {
        var player = FindObjectOfType<PlayerController>();
        player.Hp = player.MaxHp;
    }
}
