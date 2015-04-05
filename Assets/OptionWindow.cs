using UnityEngine;
using System.Collections;

public class OptionWindow : MonoBehaviour
{
    public void DataReset()
    {
        PlayerPrefs.DeleteAll();
        UpgradeSystem.instance.Reset();
    }
}
