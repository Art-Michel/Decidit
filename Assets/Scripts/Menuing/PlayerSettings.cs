using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class PlayerSettings : MonoBehaviour
{
    #region VolumeSettingsVariablesFmod
    FMOD.Studio.Bus master;
    [SerializeField] float masterVolumeNumber = 1f;
    FMOD.Studio.Bus SFX;
    [SerializeField] float SFXVolumeNumber = 1f;
    FMOD.Studio.Bus music;
    [SerializeField] float musicVolumeNumber = 1f;
    #endregion

    #region ResolutionsVariables
    Resolution[] DisponibleResolutions;
    [SerializeField] Dropdown resolutionDropdown;
    #endregion

    #region SensivityVariables
    float xSensivity = Mathf.Clamp(25f, 0.1f, 100f);
    float ySensivity = Mathf.Clamp(25f, 0.1f, 100f);

    float xControllerSensivity = Mathf.Clamp(25f, 0.1f, 100f);
    float yControllerSensivity = Mathf.Clamp(25f, 0.1f, 100f);

    #endregion

    Resolution resolution;

    private void Awake()
    {
        //ça va chercher la magie que j'ai fait sur FMod
        LoadBus();

        //Debug qui affiche la current value du son (Tu peux l'enlever quand tu aura fini)
        master.getVolume(out masterVolumeNumber);
        SFX.getVolume(out SFXVolumeNumber);
        music.getVolume(out musicVolumeNumber);

        //chargement des settings
        LoadPrefs();

        //Ca fait en sorte que le résolution soit automatiquement celle de l'écran
        DisponibleResolutions = Screen.resolutions;
        if (resolutionDropdown != null)
            resolutionDropdown.ClearOptions();

        //Set up des résolutions dispo
        ResolutionsSetUp();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MasterVolumeUpdate();//Debug que tu pourra aussi elever
    }
    #region VolumeFonctions
    public void MasterVolumeUpdate()//ToDoArt un slider qui appele la fonction connecté a la valeur MasterVolumeNumber
    {
        master.setVolume(masterVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        master.getVolume(out masterVolumeNumber);//Debug qui affiche la current value du son
        SavePrefs();

    }
    public void SFXVolumeUpdate()//ToDoArt un slider qui appele la fonction connecté a la valeur SFXVolumeNumber
    {
        SFX.setVolume(SFXVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        SFX.getVolume(out SFXVolumeNumber);//Debug qui affiche la current value du son
        SavePrefs();
    }
    public void MusicVolumeUpdate()//ToDoArt un slider qui appele la fonction connecté a la valeur MusicVolumeNumber
    {
        music.setVolume(musicVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        music.getVolume(out musicVolumeNumber);//Debug qui affiche la current value du son
        SavePrefs();
    }
    #endregion

    #region SavingProcess
    public void SavePrefs()
    {
        //TMTC ça save
        PlayerPrefs.GetFloat("ResolutionWidith", resolution.width);
        PlayerPrefs.GetFloat("ResolutionHeight", resolution.height);
        PlayerPrefs.SetFloat("XSensivity", xSensivity);
        PlayerPrefs.SetFloat("YSensivity", ySensivity);
        PlayerPrefs.SetFloat("XControllerSensivity", xControllerSensivity);
        PlayerPrefs.SetFloat("YControllerSensivity", yControllerSensivity);
        PlayerPrefs.SetFloat("MasterBaseVolume", masterVolumeNumber);
        PlayerPrefs.SetFloat("SFXBaseVolume", SFXVolumeNumber);
        PlayerPrefs.SetFloat("MusicBaseVolume", musicVolumeNumber);
        PlayerPrefs.Save();
        UnityEngine.Debug.Log("Save");
    }
    public void LoadPrefs()
    {
        masterVolumeNumber = PlayerPrefs.GetFloat("MasterBaseVolume", masterVolumeNumber);
        SFXVolumeNumber = PlayerPrefs.GetFloat("SFXBaseVolume", SFXVolumeNumber);
        musicVolumeNumber = PlayerPrefs.GetFloat("MusicBaseVolume", musicVolumeNumber);
    }

    [Button]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

    #region ResolutionFonctions
    public void ScreenResolution(int ResolutionIndex) //Foction à appeller pour set la resolution
    {
        resolution = DisponibleResolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SavePrefs();
    }

    public void ResolutionsSetUp()
    {
        //A renseigner dans l'editor à la main (les differentes résolutions) JE NE PEUX PAS FAIRE PLUS PRORE UNITY DE MERDE
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < DisponibleResolutions.Length; i++)
        {
            //Rentre les valleurs dans DisponibleResolutions
            string option = DisponibleResolutions[i].width + "x" + DisponibleResolutions[i].height;
            options.Add(option);

            //Sécurité
            if (DisponibleResolutions[i].width == Screen.currentResolution.width && DisponibleResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        //Aplique la resolution en temps réel 
        //Uncomment// resolutionDropdown.AddOptions(options);
        //Uncomment// resolutionDropdown.value = currentResolutionIndex;
        //Uncomment// resolutionDropdown.RefreshShownValue();
    }
    public void fullScreenFonction(bool isFullScreen) //fonction pour faire en sorte que le FS soit activable ou non
    {
        Screen.fullScreen = isFullScreen;
    }
    #endregion

    #region SensivityFonctions
    public void ChangeMouseSensivity(float x, float y)
    {
        xSensivity = x;
        ySensivity = y;
        //ta fonction avec xSensivity pour x et ySensivity pour y
        SavePrefs();//Sauvegarde
    }

    public void ChangeControllerSensivity(float x, float y)
    {
        xControllerSensivity = x;
        yControllerSensivity = y;
        //ta fonction avec xSensivity pour x et ySensivity pour y
        SavePrefs();//Sauvegarde
    }
    #endregion

    public void AimAssistChange()
    {

    }
    public void LoadBus()
    {
        master = FMODUnity.RuntimeManager.GetBus("bus:/");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        music = FMODUnity.RuntimeManager.GetBus("bus:/Music");
    }
    private void OnApplicationQuit()
    {
        SavePrefs();//babysitting anti con du Alt+F4
    }
}
