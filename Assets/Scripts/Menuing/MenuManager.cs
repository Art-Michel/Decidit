using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using NaughtyAttributes;
using UnityEngine.Rendering;
using TMPro;

public class MenuManager : LocalManager<MenuManager>
{
    //Scenes
    const int _mainMenuIndex = 0;
    const int _gameIndex = 1;
    const int _dioramaGreen = 2;
    const int _dioramaRed = 3;
    const int _dioramaYellow = 4;
    const int _props = 11;

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
    [SerializeField] Submenu _fugueSelect;
    [SerializeField] Submenu _museSelect;
    [SerializeField] Submenu _cimetiereSelect;
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
        Win,
        FugueSelect,
        MuseSelect,
        CimetiereSelect
    }

    private Dictionary<Menus, Submenu> _submenus;
    [SerializeField] private Submenu _firstMenu;
    public Submenu CurrentMenu;
    public GameObject _currObj;

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
    const float _sceneUnfadingDuration = 1f;
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
    public Devices CurrentDevice;

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
            { Menus.FugueSelect, _fugueSelect },
            { Menus.MuseSelect, _museSelect },
            { Menus.CimetiereSelect, _cimetiereSelect },
        };
        //Initialize Dictionnary
    }

    void Start()
    {
        DebugManager.instance.enableRuntimeUI = false;
        //SoundManager.Instance.PlayMenu();
        //Start the main menu Theme
    }

    public void StartMenuing()
    {
        EnableMenuInputs();
        _menuParent.SetActive(true);
        _eventSys.SetSelectedGameObject(null);
        MenuManager.Instance.ResetFirstButton();

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
            if (_healthBar != null)
                _healthBar.SetActive(false);
            InteractionRay.Instance.enabled = false;


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

        _eventSys.SetSelectedGameObject(null);
        if (CurrentMenu != null)
        {
            if (CurrentMenu.Id == Menus.Inputsettings || CurrentMenu.Id == Menus.AudioSettings || CurrentMenu.Id == Menus.Videosettings)
                PlayerSettings.Instance.SavePrefs();
            CurrentMenu.gameObject.SetActive(false);
        }
        CurrentMenu = null;


        //re enable everything
        if (Player.Instance != null)
        {
            Player.Instance.enabled = true;
            foreach (GameObject gun in PlayerManager.Instance.Guns)
                gun.GetComponent<Revolver>().enabled = true;
            foreach (GameObject arm in PlayerManager.Instance.Arms)
                arm.GetComponent<Arm>().enabled = true;
            if (_healthBar != null)
                _healthBar.SetActive(true);
            InteractionRay.Instance.enabled = true;
        }
    }

    public void GreyOutItem(Menus menu, int i)
    {
        if (_submenus[menu].GreyOuts == null)
        {
            Debug.LogError("No greyouts array initialized");
            return;
        }
        if (_submenus[menu].GreyOuts[i] == null)
        {
            Debug.LogError("No greyout object to enable");
            return;
        }
        _submenus[menu].GreyOuts[i].SetActive(true);
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
        CurrentDevice = Devices.Mouse;
    }

    public void ResetFirstButton()
    {
        foreach (var (key, value) in _submenus)
        {
            _submenus[key].FirstButton = _submenus[key].FirstFirstButton;
        }
    }

    #region Submenu navigation
    public void OpenSubmenu(Menus menu, bool backwards)
    {
        if (_submenus[menu] == CurrentMenu)
            return;

        Submenu previousMenu = CurrentMenu;
        CurrentMenu = _submenus[menu];

        _lastSelectedObject = null;
        if (previousMenu != null)
        {
            previousMenu.gameObject.SetActive(false);

            //Remember button only if we are going forward in th hierarchy
            if (!backwards)
                previousMenu.FirstButton = _eventSys.currentSelectedGameObject;
        }
        CurrentMenu.gameObject.SetActive(true);

        if (CurrentDevice == Devices.Controller || CurrentDevice == Devices.Keyboard)
            _eventSys.SetSelectedGameObject(CurrentMenu.FirstButton);
    }

    public void OpenPreviousMenu()
    {
        if (CurrentMenu.Id == Menus.Inputsettings || CurrentMenu.Id == Menus.AudioSettings || CurrentMenu.Id == Menus.Videosettings)
            PlayerSettings.Instance.SavePrefs();
        if (CurrentMenu.PreviousMenu != null)
        {
            OpenSubmenu(CurrentMenu.PreviousMenu.Id, true);
        }
        else if (PlayerManager.Instance != null && CurrentMenu.Id == Menus.Main)
            PlayerManager.Instance.Unpause();
    }

    public void OpenMain()
    {
        OpenSubmenu(Menus.Main, false);
    }

    public void OpenPlaySelect()
    {
        OpenSubmenu(Menus.Playselect, false);
    }

    public void OpenVideoSettings()
    {
        SetSlidersToPlayerPrefs();
        OpenSubmenu(Menus.Videosettings, false);
    }

    public void OpenAudioSettings()
    {
        SetSlidersToPlayerPrefs();
        OpenSubmenu(Menus.AudioSettings, false);
    }

    public void OpenInputSettings()
    {
        SetSlidersToPlayerPrefs();
        OpenSubmenu(Menus.Inputsettings, false);
    }

    [SerializeField] Slider _mouseXSlider;
    [SerializeField] Slider _mouseYSlider;
    [SerializeField] Slider _controllerXSlider;
    [SerializeField] Slider _controllerYSlider;

    public void SetSlidersToPlayerPrefs()
    {
        Debug.Log("Loaded PlayerPrefs");
        _mouseXSlider.value = PlayerPrefs.GetFloat("XSensivity");
        _mouseYSlider.value = PlayerPrefs.GetFloat("YSensivity");
        _controllerXSlider.value = PlayerPrefs.GetFloat("XControllerSensivity");
        _controllerYSlider.value = PlayerPrefs.GetFloat("YControllerSensivity");
    }

    public void OpenCredits()
    {
        OpenSubmenu(Menus.Credits, false);
    }

    public void OpenQuit()
    {
        OpenSubmenu(Menus.Quit, false);
    }

    public void OpenCheats()
    {
        OpenSubmenu(Menus.Cheats, false);
    }

    public void OpenRestart()
    {
        OpenSubmenu(Menus.Restart, false);
    }

    public void OpenDeath()
    {
        OpenSubmenu(Menus.Death, false);
    }

    public void OpenWin()
    {
        OpenSubmenu(Menus.Win, false);
    }

    public void OpenFugue()
    {
        OpenSubmenu(Menus.FugueSelect, false);
    }

    public void OpenMuse()
    {
        OpenSubmenu(Menus.MuseSelect, false);
    }

    public void OpenCimetiere()
    {
        OpenSubmenu(Menus.CimetiereSelect, false);
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

    public void LoadProps()
    {
        StartLoadingScene(_props);
    }

    #endregion

    #region Transitions

    void StartLoadingScene(int scene)
    {
        if (_isFading)
            return;

        StartFading(_sceneFadingDuration);
        if (_loading != null)
            _loading.gameObject.SetActive(true);

        DisableMenuInputs();
        SceneManager.LoadSceneAsync(scene);
        Time.timeScale = 0;
    }

    void StartExiting()
    {
        if (_isFading)
            return;

        StartFading(_sceneFadingDuration);
        if (_loading != null)
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

        _currObj = _eventSys.currentSelectedGameObject;
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
    public void SwitchToController()
    {
        _eventSys.sendNavigationEvents = true;

        if (CurrentDevice == Devices.Controller)
            return;
        bool wasUsingMouse = CurrentDevice == Devices.Mouse;
        CurrentDevice = Devices.Controller;

        if (wasUsingMouse)
            TransitionFromMouse();

        _eventSys.sendNavigationEvents = false;
    }

    public void SwitchToKeyboard()
    {
        _eventSys.sendNavigationEvents = true;

        if (CurrentDevice == Devices.Keyboard)
            return;
        bool wasUsingMouse = CurrentDevice == Devices.Mouse;
        CurrentDevice = Devices.Keyboard;

        if (wasUsingMouse)
            TransitionFromMouse();

        _eventSys.sendNavigationEvents = false;
    }

    private void TransitionFromMouse()
    {
        //get button under mouse if there is any
        GameObject buttonUnderMouse = CheckUnderMouse();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (buttonUnderMouse == null)
        {
            if (_lastSelectedObject != null)
            {
                Debug.Log("Set to Last selected button: " + _lastSelectedObject.gameObject.name);
                _eventSys.SetSelectedGameObject(_lastSelectedObject);
                return;
            }
            else
            {
                Debug.Log("Set to first button: " + CurrentMenu.FirstButton.gameObject.name);
                _eventSys.SetSelectedGameObject(CurrentMenu.FirstButton);
                return;
            }
        }
        else
        {
            // Debug.Log("Set to button under mouse: " + buttonUnderMouse.gameObject.name);
            _eventSys.SetSelectedGameObject(buttonUnderMouse);
            return;
        }
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
                if ((res.gameObject.TryGetComponent<Button>(out Button button)))
                {
                    return button.gameObject;
                }
            }
            return null;
        }
        else return null;
    }

    public void SwitchToMouse()
    {
        if (!Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (CurrentDevice == Devices.Mouse)
            return;

        CurrentDevice = Devices.Mouse;
        _lastSelectedObject = _eventSys.currentSelectedGameObject;
        _eventSys.SetSelectedGameObject(null);
    }
    #endregion
}
