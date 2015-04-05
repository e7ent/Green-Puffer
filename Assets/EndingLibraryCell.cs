using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingLibraryCell : MonoBehaviour
{

    public EndingLibraryManager.Type type;

    public GameObject unlocked;

    void Start()
    {
        unlocked.SetActive(!EndingLibraryManager.instance.IsUnlocked(type));
    }
}
