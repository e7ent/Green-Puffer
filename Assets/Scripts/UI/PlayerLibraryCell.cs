using UnityEngine;
using System.Collections;

public class PlayerLibraryCell : MonoBehaviour
{
    public string id;

    public GameObject unlocked;

    void Start()
    {
        unlocked.SetActive(!PlayerLibraryManager.instance.IsUnlocked(id));
    }
}
