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
    FMOD.Studio.Bus Master;
    [SerializeField] float MasterVolumeNumber = 1f;
    FMOD.Studio.Bus SFX;
    [SerializeField] float SFXVolumeNumber = 1f;
    FMOD.Studio.Bus Music;
    [SerializeField] float MusicVolumeNumber = 1f;
    #endregion


    private void Awake()
    {
        //ça va chercher la magie que j'ai fait sur FMod
        LoadBus();

        //Debug qui affiche la current value du son (Tu peux l'enlever quand tu aura fini)
        Master.getVolume(out MasterVolumeNumber);
        SFX.getVolume(out SFXVolumeNumber);
        Music.getVolume(out MusicVolumeNumber);

        //chargement des settings
        LoadPrefs();
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
        Master.setVolume(MasterVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        Master.getVolume(out MasterVolumeNumber);//Debug qui affiche la current value du son
        SavePrefs();

    }
    public void SFXVolumeUpdate()//ToDoArt un slider qui appele la fonction connecté a la valeur SFXVolumeNumber
    {
        SFX.setVolume(SFXVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        SFX.getVolume(out SFXVolumeNumber);//Debug qui affiche la current value du son
    }
    public void MusicVolumeUpdate()//ToDoArt un slider qui appele la fonction connecté a la valeur MusicVolumeNumber
    {
        Music.setVolume(MusicVolumeNumber);//1 = 0Db donc la valeur normale donc 0 = plus de son
        Music.getVolume(out MusicVolumeNumber);//Debug qui affiche la current value du son
    }
    #endregion
    public void SavePrefs()
    {
        //TMTC ça save
        PlayerPrefs.SetFloat("MasterBaseVolume", MasterVolumeNumber);
        PlayerPrefs.SetFloat("SFXBaseVolume", SFXVolumeNumber);
        PlayerPrefs.SetFloat("MusicBaseVolume", MusicVolumeNumber);
        PlayerPrefs.Save();
    }
    public void LoadPrefs()
    {
        MasterVolumeNumber = PlayerPrefs.GetFloat("MasterBaseVolume",MasterVolumeNumber);
        SFXVolumeNumber = PlayerPrefs.GetFloat("SFXBaseVolume", SFXVolumeNumber);
        MusicVolumeNumber = PlayerPrefs.GetFloat("MusicBaseVolume", MusicVolumeNumber);
    }

    [Button]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void LoadBus()
    {
        Master = FMODUnity.RuntimeManager.GetBus("bus:/");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Music");
    }

    private void OnApplicationQuit()
    {
        SavePrefs();//babysitting anti con du Alt+F4
    }
}
