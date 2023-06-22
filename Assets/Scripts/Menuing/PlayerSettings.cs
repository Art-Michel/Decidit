using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class PlayerSettings : LocalManager<PlayerSettings>
{
    #region VolumeSettingsVariablesFmod
    FMOD.Studio.Bus master;
    [SerializeField] public float masterVolumeNumber = 1f;

    FMOD.Studio.Bus SFX;
    [SerializeField] public float SFXVolumeNumber = 1f;

    FMOD.Studio.Bus music;
    [SerializeField] public float musicVolumeNumber = 1f;

    [SerializeField] Slider sliderMasterVolumeNumber;
    [SerializeField] Slider sliderSFXVolumeNumber;
    [SerializeField] Slider slidermusicVolumeNumber;

    static bool once;
    #endregion

    #region ResolutionsVariables
    Resolution[] DisponibleResolutions;
    [SerializeField] Dropdown resolutionDropdown;
    #endregion

    #region SensivityVariables
    float xSensivity;
    float ySensivity;

    float xControllerSensivity;
    float yControllerSensivity;

    #endregion

    Resolution resolution;

    protected override void Awake()
    {
        base.Awake();
        //ça va chercher la magie que j'ai fait sur FMod
        LoadBus();

        /*//Debug qui affiche la current value du son (Tu peux l'enlever quand tu aura fini)
        master.getVolume(out masterVolumeNumber);
        SFX.getVolume(out SFXVolumeNumber);
        music.getVolume(out musicVolumeNumber);
*/
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
        //LoadSoundSettings();
    }
    private void OnEnable()
    {
        LoadSoundSettings();
    }
    // Update is called once per frame
    void Update()
    {
        // MasterVolumeUpdate();//Debug que tu pourra aussi elever
    }
    #region VolumeFonctions

    [Button]
    public void LoadSoundSettings()
    {
        // UnityEngine.Debug.Log(SaveLoadManager.LoadSoundSet().masterVolumeNumber);

        sliderMasterVolumeNumber.value = SaveLoadManager.LoadSoundSet().masterVolumeNumber * 100;
        sliderSFXVolumeNumber.value = SaveLoadManager.LoadSoundSet().SFXVolumeNumber * 100;
        slidermusicVolumeNumber.value = SaveLoadManager.LoadSoundSet().musicVolumeNumber * 100;

        MasterVolumeUpdate(sliderMasterVolumeNumber);
        SFXVolumeUpdate(sliderSFXVolumeNumber);
        MusicVolumeUpdate(slidermusicVolumeNumber);
    }

    public void MasterVolumeUpdate(Slider slider)//ToDoArt un slider qui appele la fonction connecté a la valeur MasterVolumeNumber
    {
        masterVolumeNumber = slider.value / 100;

        // UnityEngine.Debug.Log(slider.value);
        // UnityEngine.Debug.Log(masterVolumeNumber);

        master.setVolume(masterVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        master.getVolume(out masterVolumeNumber);//Debug qui affiche la current value du son
    }
    public void SFXVolumeUpdate(Slider slider)//ToDoArt un slider qui appele la fonction connecté a la valeur SFXVolumeNumber
    {
        SFXVolumeNumber = slider.value / 100;

        SFX.setVolume(SFXVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        SFX.getVolume(out SFXVolumeNumber);//Debug qui affiche la current value du son
    }
    public void MusicVolumeUpdate(Slider slider)//ToDoArt un slider qui appele la fonction connecté a la valeur MusicVolumeNumber
    {
        musicVolumeNumber = slider.value / 100;

        music.setVolume(musicVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        music.getVolume(out musicVolumeNumber);//Debug qui affiche la current value du son
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
        /*PlayerPrefs.SetFloat("MasterBaseVolume", masterVolumeNumber);
        PlayerPrefs.SetFloat("SFXBaseVolume", SFXVolumeNumber);
        PlayerPrefs.SetFloat("MusicBaseVolume", musicVolumeNumber);*/
        PlayerPrefs.Save();
        // UnityEngine.Debug.Log("Save");
    }
    public void LoadPrefs()
    {
        /*  masterVolumeNumber = PlayerPrefs.GetFloat("MasterBaseVolume", masterVolumeNumber);
          SFXVolumeNumber = PlayerPrefs.GetFloat("SFXBaseVolume", SFXVolumeNumber);
          musicVolumeNumber = PlayerPrefs.GetFloat("MusicBaseVolume", musicVolumeNumber);*/
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
    public void ChangeMouseSensivityX(float x)
    {
        xSensivity = Mathf.Round(x * 10) / 10;
    }

    public void ChangeMouseSensivityY(float y)
    {
        ySensivity = Mathf.Round(y * 10) / 10;
    }

    public void ChangeControllerSensivityX(float x)
    {
        xControllerSensivity = Mathf.Round(x * 10) / 10;
    }

    public void ChangeControllerSensivityY(float y)
    {
        yControllerSensivity = Mathf.Round(y * 10) / 10;
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SaveLoadManager.SaveSoundSet(this);

    }
    private void OnApplicationQuit()
    {
        // SavePrefs();//babysitting anti con du Alt+F4
        SaveLoadManager.SaveSoundSet(this);
    }
}
