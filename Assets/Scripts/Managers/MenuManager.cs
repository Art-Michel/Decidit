using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance = null;
    [SerializeField] int _gameIndex;
    [SerializeField] int _optionIndex;
    [SerializeField] List<Button> Buttons;

    [SerializeField] GameObject firstSelected;

    [SerializeField] EventSystem eventsys;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
        List<Sprite> sprites = new List<Sprite>();
    }

    void Start()
    {

    }

    void Update()
    {
        SetSelectedGameObjectToSettings();
        Debug.Log(eventsys.currentSelectedGameObject);
    }
    #region Butons fonctions
    //toutes les fonctions pour les boutons 
    public void Quit()
    {
        //fait quitter le jeu
        Application.Quit();
    }

    public void LoadGame()
    {
        //fait entrer le joueur dans la scene de jeu
        SceneManager.LoadScene(_gameIndex);
    }

    public void Options()
    {
        //fait entrer le joueur dans les options
        SceneManager.LoadScene(_optionIndex);
    }

    public void SetSelectedGameObjectToSettings()
    {
        //switch manette/clavier souris
        if(eventsys.currentSelectedGameObject == null)
        {
            eventsys.SetSelectedGameObject(firstSelected);
        }
    }

    public void HideMousse()
    {
        Cursor.visible = false;
    }

    #endregion
}
