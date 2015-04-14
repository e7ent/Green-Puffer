using UnityEngine;
using System.Collections;

public class EndingLibraryManager : MonoSingleton<EndingLibraryManager>
{

    public enum Type
    {
        Rebirth,
        Hurt,
        Stress,
        Sick
    }

    public void Unlock(Type type)
    {
        PlayerPrefs.SetInt("Ending" + type.ToString(), 1);
    }

    public bool IsUnlocked(Type type)
    {
        return PlayerPrefs.GetInt("Ending" + type.ToString(), 0) == 1;
    }
}
