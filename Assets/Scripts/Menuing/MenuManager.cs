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
    const int _mainMenuIndex = 0;
    const int _gameIndex = 1;
    const int _dioramaGreen = 2;
    const int _dioramaRed = 3;
    const int _dioramaYellow = 4;

    [Foldout("References")]
    [SerializeField] EventSystem _eventSys;
    [Foldout("References")]
    [SerializeField] GameObject _menuParent;
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
    [Foldout("Loading")]
    [SerializeField] Image _loading;
    [Foldout("Fading")]
    [SerializeField] Image _fade;
    [Foldout("Fading")]
    [SerializeField]
    const float _sceneFadingDuration = 1f;
    private bool _isFading = false;
    private float _currentFadeDuration;
    private float _fadingT;
    [Foldout("Fading")]
    [SerializeField]
    const float _sceneUnfadingDuration = .5f;
    private bool _isUnfading = false;
    private float _currentUnfadeDuration;
    private float _unfadingT;
    [Foldout("Fading")]
    [SerializeField]
    AnimationCurve _unfadingCurve;

    [Foldout("Things to disable on pause")]
    [SerializeField] Volume _postProcessVolume;
    [Foldout("Things to disable on pause")]
    [SerializeField] GameObject _healthBar;

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
        StartUnfading(_sceneUnfadingDuration);
    }

    public void StartMenuing()
    {
        EnableMenuInputs();
        _menuParent.SetActive(true);
        _eventSys.SetSelectedGameObject(null);

        //blur
        _postProcessVolume.enabled = true;

        if (Player.Instance != null)
        {
            //Disable everything
            Player.Instance.enabled = false;
            foreach (GameObject gun in PlayerManager.Instance.Guns)
                gun.GetComponent<Revolver>().enabled = false;
            foreach (GameObject arm in PlayerManager.Instance.Arms)
                arm.GetComponent<Arm>().enabled = false;
            _healthBar.SetActive(false);

            //Stop rumble
            PlayerManager.Instance.StopRumbling();
        }
    }

    public void StopMenuing()
    {
        DisableMenuInputs();
        if (_menuParent)
            _menuParent.SetActive(false);

        //blur
        _postProcessVolume.enabled = false;

        //re enable everything
        if (Player.Instance != null)
        {
            Player.Instance.enabled = true;
            foreach (GameObject gun in PlayerManager.Instance.Guns)
                gun.GetComponent<Revolver>().enabled = true;
            foreach (GameObject arm in PlayerManager.Instance.Arms)
                arm.GetComponent<Arm>().enabled = true;
            _healthBar.SetActive(true);
        }
    }

    public void DisableMenuInputs()
    {
        _inputs.Disable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnableMenuInputs()
    {
        _inputs.Enable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #region Submenu navigation
    private void OpenSubmenu(Menus menu)
    {
        if (_submenus[menu] == CurrentMenu)
            return;

        Submenu previousMenu = CurrentMenu;
        CurrentMenu = _submenus[menu];

        _lastSelectedObject = null;
        if (previousMenu != null)
        {
            previousMenu.FirstButton = _eventSys.currentSelectedGameObject;
            previousMenu.gameObject.SetActive(false);
        }
        CurrentMenu.gameObject.SetActive(true);

        if (_currentDevice == Devices.Controller || _currentDevice == Devices.Keyboard)
            _eventSys.SetSelectedGameObject(CurrentMenu.FirstButton);
    }

    public void OpenPreviousMenu()
    {
        if (CurrentMenu.PreviousMenu != null)
            OpenSubmenu(CurrentMenu.PreviousMenu.Id);
        else if (PlayerManager.Instance != null && CurrentMenu.Id == Menus.Main)
            PlayerManager.Instance.Unpause();
    }

    public void OpenMain()
    {
        OpenSubmenu(Menus.Main);
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
        StartFading(_sceneFadingDuration);
        _loading.gameObject.SetActive(true);

        DisableMenuInputs();
        SceneManager.LoadSceneAsync(scene);
        Time.timeScale = 1;
    }

    void StartExiting()
    {
        StartFading(_sceneFadingDuration);
        _loading.gameObject.SetActive(true);

        DisableMenuInputs();
        Application.Quit();
    }

    public void StartFading(float length)
    {
        _isFading = true;
        _fadingT = 0f;
        _currentFadeDuration = length;
        _fade.gameObject.SetActive(true);
    }

    void Update()
    {
        if (_isFading)
        {
            _fadingT += Time.unscaledDeltaTime;
            _fade.color = new Color(0, 0, 0, Mathf.InverseLerp(0, _currentFadeDuration, _fadingT));
            if (_fadingT >= _currentFadeDuration)
            {
                _isFading = false;
                _fade.color = Color.black;
            }
        }
        if (_isUnfading)
        {
            _unfadingT += Time.unscaledDeltaTime;
            _fade.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, _unfadingCurve.Evaluate(Mathf.InverseLerp(0, _currentUnfadeDuration, _unfadingT))));
            if (_unfadingT >= _currentUnfadeDuration)
            {
                _isUnfading = false;
                _fade.color = new Color(0, 0, 0, 0);
                _fade.gameObject.SetActive(false);
            }
        }
    }

    public void StartUnfading(float length)
    {
        _fade.gameObject.SetActive(true);
        _isUnfading = true;
        _unfadingT = 0f;
        _currentUnfadeDuration = length;
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

        Cursor.visible = true;
        _currentDevice = Devices.Mouse;
        _lastSelectedObject = _eventSys.currentSelectedGameObject;
        _eventSys.SetSelectedGameObject(null);
    }
    #endregion
}
