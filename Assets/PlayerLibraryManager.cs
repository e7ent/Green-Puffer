using UnityEngine;
using System.Collections;

public class PlayerLibraryManager : MonoSingleton<PlayerLibraryManager>
{

    public void Unlock(string id)
    {
        PlayerPrefs.SetInt("Ending" + id, 1);
    }

    public bool IsUnlocked(string id)
    {
        return PlayerPrefs.GetInt("Ending" + id, 0) == 1;
    }
}
