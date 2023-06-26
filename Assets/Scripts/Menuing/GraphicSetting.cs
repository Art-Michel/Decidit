using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicSetting : MonoBehaviour
{
    [SerializeField] Vector2[] resolution;
    [SerializeField] Dropdown dropdownResolution;

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
        int index = dropdownResolution.value;

        switch (index)
        {
            case 0:
                QualitySettings.vSyncCount = 1;
                break;
            case 1:
                QualitySettings.vSyncCount = 0;
                break;
        }
    }

    void Start()
    {
        Screen.SetResolution((int)resolution[0].x, (int)resolution[0].y,
                    FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);

        QualitySettings.vSyncCount = 1;
        //Screen.SetResolution((int)quatreK.x, (int)quatreK.y, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
