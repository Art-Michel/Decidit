using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using NaughtyAttributes;

public class MenuManager : LocalManager<MenuManager>
{
    //Scenes
    [Foldout("References")]
    const int _mainMenuIndex = 0;
    [Foldout("References")]
    const int _gameIndex = 1;
    [Foldout("References")]
    const int _gameOverIndex = 2;
    [Foldout("References")]
    const int _winScreenIndex = 3;
    [Foldout("References")]
    const int _dioramaGreen = 4;
    [Foldout("References")]
    const int _dioramaRed = 5;
    [Foldout("References")]
    const int _dioramaYellow = 6;

    [Foldout("References")]
    [SerializeField] EventSystem _eventSys;
    PlayerInputMap _inputs;

    //Submenus
    [SerializeField] Submenu _mainMenu;
    [SerializeField] Submenu _videoSettings;
    [SerializeField] Submenu _inputsSettings;
    [SerializeField] Submenu _audioSettings;
    [SerializeField] Submenu _playSelect;
    [SerializeField] Submenu _credits;
    [SerializeField] Submenu _quitConfirm;
    public enum Menus
    {
        Main,
        Videosettings,
        Inputsettings,
        AudioSettings,
        Playselect,
        Credits,
        Quit
    }

    private Dictionary<Menus, Submenu> _submenus;
    [SerializeField] Submenu _currentMenu;

    //Fade to black when loading a scene
    [Foldout("Fading")]
    [SerializeField] Image _fade;
    [Foldout("Fading")]
    [SerializeField]
    float _sceneFadingDuration = 1f;
    private float _fadingT;
    private bool _isFading = false;

    //Devices 
    public enum Devices { Controller, Keyboard, Mouse }
    Devices _currentDevice;

    protected override void Awake()
    {
        base.Awake();
        _inputs = new PlayerInputMap();
        _inputs.MenuNavigation.anyKey.started += _ => SwitchToKeyboard();
        _inputs.MenuNavigation.anyButton.started += _ => SwitchToController();
        _inputs.MenuNavigation.moveMouse.started += _ => SwitchToMouse();
        _inputs.MenuNavigation.Cancel.started += _ => PreviousMenu();

        _submenus = new Dictionary<Menus, Submenu>()
        {
            { Menus.Main, _mainMenu },
            { Menus.Videosettings, _videoSettings },
            { Menus.Inputsettings, _inputsSettings },
            { Menus.AudioSettings, _audioSettings },
            { Menus.Playselect, _playSelect },
            { Menus.Credits, _credits },
            { Menus.Quit, _quitConfirm },
        };
        //Initialize Dictionnary
    }

    void Start()
    {
        _currentDevice = Devices.Controller;
        _eventSys.SetSelectedGameObject(null);
    }

    #region Submenu navigation
    private void OpenSubmenu(Menus menu)
    {
        Submenu previousMenu = _currentMenu;
        _currentMenu = _submenus[menu];

        _currentMenu.gameObject.SetActive(true);
        previousMenu.gameObject.SetActive(false);

        if (_currentDevice == Devices.Controller || _currentDevice == Devices.Keyboard)
            _eventSys.SetSelectedGameObject(_currentMenu.FirstButton);
    }

    public void PreviousMenu()
    {
        if (_currentMenu.PreviousMenu != null)
            OpenSubmenu(_currentMenu.PreviousMenu.Id);
    }

    public void PlaySelect()
    {
        //fait entrer le joueur dans les options
        OpenSubmenu(Menus.Playselect);
    }

    public void VideoSettings()
    {
        //fait entrer le joueur dans les options
        OpenSubmenu(Menus.Videosettings);
    }

    public void AudioSettings()
    {
        //fait entrer le joueur dans les options
        OpenSubmenu(Menus.AudioSettings);
    }

    public void InputSettings()
    {
        //fait entrer le joueur dans les options
        OpenSubmenu(Menus.Inputsettings);
    }

    public void Credits()
    {
        //fait entrer le joueur dans les options
        OpenSubmenu(Menus.Credits);
    }

    public void Quit()
    {
        //fait entrer le joueur dans les options
        OpenSubmenu(Menus.Quit);
    }

    #endregion

    #region Scene Loading
    //toutes les fonctions pour les boutons 
    public void ReallyQuit()
    {
        //fait quitter le jeu
        StartExiting();
    }

    public void LoadGame()
    {
        StartLoadingScene(_gameIndex);
    }

    public void LoadMainMenu()
    {
        StartLoadingScene(_mainMenuIndex);
    }

    public void LoadGameOver()
    {
        StartLoadingScene(_gameOverIndex);
    }

    public void LoadWinScreen()
    {
        StartLoadingScene(_winScreenIndex);
    }

    public void LoadDioramaRed()
    {
        StartLoadingScene(_dioramaRed);
    }

    public void LoadDioramaGreen()
    {
        StartLoadingScene(_dioramaGreen);
    }

    public void LoadDioramaYellow()
    {
        StartLoadingScene(_dioramaYellow);
    }

    #endregion

    #region Transitions

    void StartLoadingScene(int scene)
    {
        _isFading = true;
        _fadingT = 0f;
        _fade.gameObject.SetActive(true);

        SceneManager.LoadSceneAsync(scene);
    }

    void StartExiting()
    {
        _isFading = true;
        _fadingT = 0f;
        _fade.gameObject.SetActive(true);
        Application.Quit();
    }

    void Update()
    {
        if (_isFading)
        {
            _fadingT += Time.deltaTime;
            _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, Mathf.InverseLerp(0, _sceneFadingDuration, _fadingT));
        }
    }
    #endregion

    #region Controller / Keyboard / Mouse Switching
    private void SwitchToController()
    {
        _eventSys.sendNavigationEvents = true;

        if (_currentDevice == Devices.Controller)
            return;
        _currentDevice = Devices.Controller;

        //get button under mouse if there is any
        GameObject buttonUnderMouse = CheckUnderMouse();

        //remove cursor
        Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        if (buttonUnderMouse == null)
            _eventSys.SetSelectedGameObject(_currentMenu.FirstButton);
        else
            _eventSys.SetSelectedGameObject(buttonUnderMouse);
        _eventSys.sendNavigationEvents = false;
    }

    private void SwitchToKeyboard()
    {
        _eventSys.sendNavigationEvents = true;

        if (_currentDevice == Devices.Keyboard)
            return;
        _currentDevice = Devices.Keyboard;

        //get button under mouse if there is any
        GameObject buttonUnderMouse = CheckUnderMouse();

        //remove cursor
        Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        if (buttonUnderMouse == null)
            _eventSys.SetSelectedGameObject(_currentMenu.FirstButton);
        else
            _eventSys.SetSelectedGameObject(buttonUnderMouse);
        _eventSys.sendNavigationEvents = false;

    }

    private GameObject CheckUnderMouse()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult res in raycastResults)
            {
                if ((res.gameObject.TryGetComponent<Button>(out Button mario)))
                {
                    return mario.gameObject;
                }
            }
            return null;
        }
        else return null;
    }

    private void SwitchToMouse()
    {
        if (_currentDevice == Devices.Mouse)
            return;

        _currentDevice = Devices.Mouse;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _eventSys.SetSelectedGameObject(null);
    }
    #endregion

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
