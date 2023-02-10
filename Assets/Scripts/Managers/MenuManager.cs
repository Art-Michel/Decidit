using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class MenuManager : LocalManager<MenuManager>
{
    [SerializeField] int _gameIndex;
    [SerializeField] int _optionIndex;
    [SerializeField] int _mainIndex;

    [SerializeField] Submenu _currentMenu;

    [SerializeField] EventSystem _eventsys;

    PlayerInputMap _inputs;
    bool _currentControlSchemeIsMouse;

    protected override void Awake()
    {
        base.Awake();
        _inputs = new PlayerInputMap();
        _inputs.MenuNavigation.anyKey.performed += _ => SwitchToMK();
        _inputs.MenuNavigation.anyButton.performed += _ => SwitchToMK();
        _inputs.MenuNavigation.moveMouse.performed += _ => SwitchToController();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {

    }

    #region Buttons fonctions
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
    }
    public void MainMenu()
    {
        //fait entrer le joueur dans le menu principal
        SceneManager.LoadScene(_mainIndex);
    }

    #endregion

    private void SwitchToController()
    {
        _currentControlSchemeIsMouse = false;
    }

    private void SwitchToMK()
    {
        _currentControlSchemeIsMouse = true;
    }

    #region Enable Disable Inputs
    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
    #endregion
}
