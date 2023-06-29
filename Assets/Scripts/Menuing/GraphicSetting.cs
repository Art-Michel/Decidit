using UnityEngine;
using UnityEngine.UI;

public class GraphicSetting : MonoBehaviour
{
    [SerializeField] Vector2[] resolution;
    [SerializeField] Dropdown dropdownResolution;
    [SerializeField] Dropdown dropdownVSync;

    public void SetQualityLevelDropdown()
    {
        int index = dropdownResolution.value;

        switch (index)
        {
            case 0:
                Screen.SetResolution((int)resolution[index].x, (int)resolution[index].y, 
                    FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
                break;
            case 1:
                Screen.SetResolution((int)resolution[index].x, (int)resolution[index].y, 
                    FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
                break;
            case 2:
                Screen.SetResolution((int)resolution[index].x, (int)resolution[index].y, 
                    FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
                break;
        }
    }

    public void EnableVSync()
    {
        int index = dropdownVSync.value;

        switch (index)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                break;
            case 1:
                QualitySettings.vSyncCount = 1;
                break;
        }
    }

    void Start()
    {
        Screen.SetResolution((int)resolution[0].x, (int)resolution[0].y,
                    FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);

        QualitySettings.vSyncCount = 0;
    }
}