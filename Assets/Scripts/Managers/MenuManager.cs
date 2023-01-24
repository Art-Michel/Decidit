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

    [SerializeField] GameObject firstSelected;

    [SerializeField] EventSystem eventsys;
    Gamepad gamepad = Gamepad.current;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        Switch();
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
        if (eventsys.currentSelectedGameObject == null)
        {
            eventsys.SetSelectedGameObject(firstSelected);
        }
    }

    public void Switch()
    {
        if (gamepad.wasUpdatedThisFrame)
        {
            Debug.Log("mhidfnbjmnsbknksnb");
            Cursor.visible = false;
            SetSelectedGameObjectToSettings();
        }
        if (Input.anyKeyDown)
        {
            Debug.Log("Mouse");
            Cursor.visible = true;
        }
    }

    #endregion
}
