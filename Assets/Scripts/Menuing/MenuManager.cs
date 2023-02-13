using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using NaughtyAttributes;
using UnityEngine.Rendering;

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
    GameObject _lastSelectedObject;
    PlayerInputMap _inputs;

    //Submenus
    [SerializeField] Submenu _mainMenu;
    [SerializeField] Submenu _videoSettings;
    [SerializeField] Submenu _inputsSettings;
    [SerializeField] Submenu _audioSettings;
    [SerializeField] Submenu _playSelect;
    [SerializeField] Submenu _credits;
    [SerializeField] Submenu _quitConfirm;
    [SerializeField] Submenu _cheats;
    [SerializeField] Submenu _restart;
    [SerializeField] Submenu _win;
    [SerializeField] Submenu _death;
    public enum Menus
    {
        Main,
        Videosettings,
        Inputsettings,
        AudioSettings,
        Playselect,
        Credits,
        Quit,
        Cheats,
        Restart,
        Death,
        Win
    }

    private Dictionary<Menus, Submenu> _submenus;
    [SerializeField] private Submenu _firstMenu;
    public Submenu CurrentMenu;

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
        _inputs.MenuNavigation.Cancel.started += _ => OpenPreviousMenu();

        _submenus = new Dictionary<Menus, Submenu>()
        {
            { Menus.Main, _mainMenu },
            { Menus.Videosettings, _videoSettings },
            { Menus.Inputsettings, _inputsSettings },
            { Menus.AudioSettings, _audioSettings },
            { Menus.Playselect, _playSelect },
            { Menus.Credits, _credits },
            { Menus.Quit, _quitConfirm },
            { Menus.Cheats, _cheats },
            { Menus.Restart, _restart },
            { Menus.Death, _death },
            { Menus.Win, _win },
        };
        //Initialize Dictionnary
    }

    void Start()
    {
        DebugManager.instance.enableRuntimeUI = false;
    }

    void OnEnable()
    {
        _currentDevice = Devices.Controller;
        _eventSys.SetSelectedGameObject(null);
        EnableMenuInputs();
        CurrentMenu.gameObject.SetActive(false);
        CurrentMenu = _firstMenu;
        CurrentMenu.gameObject.SetActive(true);
        _eventSys.SetSelectedGameObject(CurrentMenu.FirstButton);
    }

    void OnDisable()
    {
        DisableMenuInputs();
    }

    public void DisableMenuInputs()
    {
        _inputs.Disable();

    }

    public void EnableMenuInputs()
    {
        _inputs.Enable();

    }

    #region Submenu navigation
    private void OpenSubmenu(Menus menu)
    {
        if (_submenus[menu] == CurrentMenu)
            return;

        Submenu previousMenu = CurrentMenu;
        CurrentMenu = _submenus[menu];

        _lastSelectedObject = null;
        previousMenu.FirstButton = _eventSys.currentSelectedGameObject;
        CurrentMenu.gameObject.SetActive(true);
        previousMenu.gameObject.SetActive(false);

        if (_currentDevice == Devices.Controller || _currentDevice == Devices.Keyboard)
            _eventSys.SetSelectedGameObject(CurrentMenu.FirstButton);
    }

    public void OpenPreviousMenu()
    {
        if (CurrentMenu.PreviousMenu != null)
            OpenSubmenu(CurrentMenu.PreviousMenu.Id);
        else if (PlayerManager.Instance != null)
            PlayerManager.Instance.Unpause();
    }

    public void OpenPlaySelect()
    {
        OpenSubmenu(Menus.Playselect);
    }

    public void OpenVideoSettings()
    {
        OpenSubmenu(Menus.Videosettings);
    }

    public void OpenAudioSettings()
    {
        OpenSubmenu(Menus.AudioSettings);
    }

    public void OpenInputSettings()
    {
        OpenSubmenu(Menus.Inputsettings);
    }

    public void OpenCredits()
    {
        OpenSubmenu(Menus.Credits);
    }

    public void OpenQuit()
    {
        OpenSubmenu(Menus.Quit);
    }

    public void OpenCheats()
    {
        OpenSubmenu(Menus.Cheats);
    }

    public void OpenRestart()
    {
        OpenSubmenu(Menus.Restart);
    }

    public void OpenDeath()
    {
        OpenSubmenu(Menus.Death);
    }

    public void OpenWin()
    {
        OpenSubmenu(Menus.Win);
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
        Time.timeScale = 1;
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
            _fadingT += Time.unscaledDeltaTime;
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
        bool wasUsingMouse = _currentDevice == Devices.Mouse;
        _currentDevice = Devices.Controller;


        if (wasUsingMouse)
            TransitionFromMouse();

        _eventSys.sendNavigationEvents = false;
    }

    private void SwitchToKeyboard()
    {
        _eventSys.sendNavigationEvents = true;

        if (_currentDevice == Devices.Keyboard)
            return;
        bool wasUsingMouse = _currentDevice == Devices.Mouse;
        _currentDevice = Devices.Keyboard;


        if (wasUsingMouse)
            TransitionFromMouse();

        _eventSys.sendNavigationEvents = false;
    }

    private void TransitionFromMouse()
    {
        //get button under mouse if there is any
        GameObject buttonUnderMouse = CheckUnderMouse();

        //remove cursor
        Cursor.visible = false;

        if (buttonUnderMouse == null)
        {
            if (_lastSelectedObject != null)
                _eventSys.SetSelectedGameObject(_lastSelectedObject);
            else
                _eventSys.SetSelectedGameObject(CurrentMenu.FirstButton);
        }
        else
            _eventSys.SetSelectedGameObject(buttonUnderMouse);
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
        _lastSelectedObject = _eventSys.currentSelectedGameObject;
        _eventSys.SetSelectedGameObject(null);
    }
    #endregion
}
