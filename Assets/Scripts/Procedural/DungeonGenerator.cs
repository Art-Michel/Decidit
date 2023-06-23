using System.Net.Mime;
using System.Collections.Specialized;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class DungeonGenerator : LocalManager<DungeonGenerator>
{
    [SerializeField] int _seed;
    [SerializeField] bool _randomizeSeed;
    ////[SerializeField] float _dungeonRotation;
    [SerializeField] int[] _difficultyPerRoom;
    private int _numberOfRooms;
    const int _firstPowerupAfterRoom = 1;
    const int _secondPowerupAfterRoom = 2;

    [SerializeField] RoomSetup _starterRoom;
    [SerializeField] RoomSetup _finalRoom;
    //// public List<RoomSetup> RoomSets;

    [SerializeField] List<int> _scenesEasy;
    [SerializeField] List<int> _scenesMedium;
    [SerializeField] List<int> _scenesHard;
    [SerializeField] int _altarIndex;
    [SerializeField] int _corridorIndex;
    [SerializeField] int _startIndex;
    [SerializeField] int _endIndex;
    [SerializeField] Image _loadingBar;
    public List<Altar> AltarList = new List<Altar>();

    [SerializeField] private List<List<Room>> _usableRooms;
    public List<RoomSetup> Corridors;
    public List<RoomSetup> CorridorsSpell;

    public int TotalRooms { get { return _actualRooms.Count - 1; } }

    [SerializeField] List<Room> _actualRooms;
    public int CurrentRoom;

    //Progression
    [System.NonSerialized] public bool ChoseASkill;
    [System.NonSerialized] public bool ChoseAGun;

    [SerializeField] List<Room> _roomsToBuild;

    protected override void Awake()
    {
        base.Awake();
        _actualRooms = new List<Room>();
    }

    void Update()
    {
        if (_isColorTransitionning)
            UpdateColorTransition();
    }

    void Start()
    {
        _numberOfRooms = _difficultyPerRoom.Length;
        SoundManager.Instance.ClearedSound();
        ResetDungeon();
        StartCoroutine(SetUsableRooms());

        ChoseAGun = false;
        ChoseASkill = false;

    }

    IEnumerator SetUsableRooms()
    {
        if (transform.childCount > 0)
        {
            ResetDungeon();
            ClearDungeon();
        }

        //* basically a new instance of the rooms setup so we can delete a room once it has been instanced in order to avoid repeats.
        int scenesToLoad = _scenesEasy.Count + _scenesMedium.Count + _scenesHard.Count + 6; // 2 altars, 2 corridors, start, and End
        scenesToLoad--;

        int scenesLoaded = 0;
        _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);

        _usableRooms = new List<List<Room>>();
        _usableRooms.Add(new List<Room>());
        for (int i = 0; i < _scenesEasy.Count; i++)
        {
            //Load a scene
            SceneManager.LoadScene(_scenesEasy[i], LoadSceneMode.Additive);
            scenesLoaded++;
            _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);
            // Debug.Log("Loaded scene " + _scenesEasy[i]);
            yield return null;

            //Add scenes level to _usableRooms
            GameObject thatScenesRoot = GameObject.Find((_scenesEasy[i].ToString()));
            Room thatScenesRoom = thatScenesRoot.GetComponent<Room>();
            _usableRooms[0].Add(thatScenesRoom);
            // thatScenesRoom.FindDoors();
            // thatScenesRoom.FindTriggers();
        }

        _usableRooms.Add(new List<Room>());
        for (int i = 0; i < _scenesMedium.Count; i++)
        {
            SceneManager.LoadScene(_scenesMedium[i], LoadSceneMode.Additive);
            scenesLoaded++;
            _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);
            // Debug.Log("Loaded scene " + _scenesMedium[i]);
            yield return null;

            GameObject thatScenesRoot = GameObject.Find((_scenesMedium[i].ToString()));
            Room thatScenesRoom = thatScenesRoot.GetComponent<Room>();
            _usableRooms[1].Add(thatScenesRoom);
            // thatScenesRoom.FindDoors();
            // thatScenesRoom.FindTriggers();
        }

        _usableRooms.Add(new List<Room>());
        for (int i = 0; i < _scenesHard.Count; i++)
        {
            SceneManager.LoadScene(_scenesHard[i], LoadSceneMode.Additive);
            scenesLoaded++;
            _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);
            // Debug.Log("Loaded scene " + _scenesHard[i]);
            yield return null;

            GameObject thatScenesRoot = GameObject.Find((_scenesHard[i].ToString()));
            Room thatScenesRoom = thatScenesRoot.GetComponent<Room>();
            _usableRooms[2].Add(thatScenesRoom);
            // thatScenesRoom.FindDoors();
            // thatScenesRoom.FindTriggers();
        }

        _usableRooms.Add(new List<Room>());
        for (int i = 0; i < 1; i++)
        {
            SceneManager.LoadScene(_altarIndex, LoadSceneMode.Additive);
            scenesLoaded++;
            _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);
            yield return null;

            GameObject thatScenesRoot = GameObject.Find((_altarIndex.ToString()));
            Room thatScenesRoom = thatScenesRoot.GetComponent<Room>();
            _usableRooms[3].Add(thatScenesRoom);
            _usableRooms[3].Add(thatScenesRoom);
        }

        _usableRooms.Add(new List<Room>());
        for (int i = 0; i < 1; i++)
        {
            SceneManager.LoadScene(_corridorIndex, LoadSceneMode.Additive);
            scenesLoaded++;
            _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);
            yield return null;

            GameObject thatScenesRoot = GameObject.Find((_corridorIndex.ToString()));
            Room thatScenesRoom = thatScenesRoot.GetComponent<Room>();
            _usableRooms[4].Add(thatScenesRoom);
            _usableRooms[4].Add(thatScenesRoom);
        }

        _usableRooms.Add(new List<Room>());
        for (int i = 0; i < 1; i++)
        {
            SceneManager.LoadScene(_startIndex, LoadSceneMode.Additive);
            scenesLoaded++;
            _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);
            yield return null;

            GameObject thatScenesRoot = GameObject.Find((_startIndex.ToString()));
            Room thatScenesRoom = thatScenesRoot.GetComponent<Room>();
            _usableRooms[5].Add(thatScenesRoom);
        }

        _usableRooms.Add(new List<Room>());
        for (int i = 0; i < 1; i++)
        {
            SceneManager.LoadScene(_endIndex, LoadSceneMode.Additive);
            scenesLoaded++;
            _loadingBar.fillAmount = Mathf.InverseLerp(0, scenesToLoad, scenesLoaded);
            yield return null;

            GameObject thatScenesRoot = GameObject.Find((_endIndex.ToString()));
            Room thatScenesRoom = thatScenesRoot.GetComponent<Room>();
            _usableRooms[6].Add(thatScenesRoom);
        }

        _loadingBar.fillAmount = 1;
        Generate();
        yield return null;
    }

    public void Generate()
    {

        //* randomizing seed
        if (_randomizeSeed)
            _seed = Random.Range(0, 1000);
        Random.InitState(_seed);

        _roomsToBuild = new List<Room>();//(_numberOfRooms + 2 + (_numberOfRooms + 1));
        _roomsToBuild.Add(_usableRooms[5][0]);

        int stackCount = Mathf.RoundToInt(_numberOfRooms / 3f);
        for (int i = 0; i < _numberOfRooms; i++)
        {
            SelectCorridor(i);

            //* passe a la difficulte suivante
            if (stackCount <= 0)
            {
                stackCount = Mathf.RoundToInt(_numberOfRooms / 3f);
            }

            //* ajoute une salle avec une difficulte predefinie et la retire du Set pour ne pas retomber dessus.
            AddRandomRoom(_difficultyPerRoom[i]);

            stackCount--;
        }

        // _roomsToBuild.Add(Corridors[Random.Range(0, Corridors.Count)].Get());
        SelectCorridor(999);
        _roomsToBuild.Add(_usableRooms[6][0]);


        foreach (Room room in _roomsToBuild)
        {
            room.FindDoors();
            room.FindTriggers();
            room.Statify();
            // room.InitAltar();
        }
        Build();
    }

    void SelectCorridor(int indexRoom)
    {
        switch (indexRoom)
        {
            case _firstPowerupAfterRoom:
                // _roomsToBuild.Add(CorridorsSpell[Random.Range(0, CorridorsSpell.Count)].Get());
                Room altarRoom = _usableRooms[3][0];
                _roomsToBuild.Add(altarRoom);
                break;

            case _secondPowerupAfterRoom:
                // _roomsToBuild.Add(CorridorsSpell[Random.Range(0, CorridorsSpell.Count)].Get());
                Room altarRoom2 = _usableRooms[3][1];
                _roomsToBuild.Add(altarRoom2);
                break;

            default:
                _roomsToBuild.Add(_usableRooms[4][Random.Range(0, _usableRooms[4].Count)]);
                break;
        }
    }

    private void AddRandomRoom(int difficulty)
    {
        int roomToAddIndex = Random.Range(0, _usableRooms[difficulty].Count);
        if (_usableRooms[difficulty].Count <= 0)
        {
            string roomSetDifficulty = "";
            switch (difficulty)
            {
                case 0:
                    roomSetDifficulty = "EASY";
                    break;
                case 1:
                    roomSetDifficulty = "MEDIUM";
                    break;
                case 2:
                    roomSetDifficulty = "HARD";
                    break;
            }
            Debug.LogError("Not enough room in RoomSet [" + roomSetDifficulty + "]");
        }

        Room roomToAdd = _usableRooms[difficulty][roomToAddIndex];
        _roomsToBuild.Add(roomToAdd);
        _usableRooms[difficulty].RemoveAt(roomToAddIndex);
    }

    private void ResetDungeon()
    {
        //transform.DetachChildren();
        _roomsToBuild.Clear();
        _actualRooms.Clear();
    }

    public void ClearDungeon()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    private void Build()
    {
        Transform lastDoor = null;
        foreach (Room room in _roomsToBuild)
        {
            //* Spawn room
            Room roomInstance;
            if (room.gameObject.activeInHierarchy)
            {
                roomInstance = room;
                roomInstance.transform.SetParent(transform);
            }
            else
            {
                roomInstance = Instantiate(room, Vector3.zero, Quaternion.identity, transform);
            }


            //* set rotation
            if (lastDoor != null)
            {
                roomInstance.transform.rotation = lastDoor.rotation;
                roomInstance.transform.Rotate(0, 180, 0);
            }
            else
                roomInstance.transform.rotation = Quaternion.identity;

            //* set position
            if (lastDoor != null)
            {
                Vector3 roomPosition = lastDoor.position + (roomInstance.transform.position - roomInstance.Entry.transform.position);
                roomInstance.transform.position = roomPosition;
            }

            //* Disable Room and its enemies
            lastDoor = roomInstance.Exit.transform;
            // roomInstance.EnableEnemies(false);
            roomInstance.gameObject.SetActive(false);
            roomInstance.InitAltar();

            _actualRooms.Add(roomInstance);
        }

        foreach (List<Room> rooms in _usableRooms)
            foreach (Room room in rooms)
                room.gameObject.SetActive(false);

        AssignChantToAltars();
        EnableFirstRooms();
        MenuManager.Instance.StartUnfading(1.0f);
        MenuManager.Instance.StopLoading();
    }

    private void AssignChantToAltars()
    {
        Altar.Chants chant0 = (Altar.Chants)Random.Range(0, 3);
        Altar.Chants chant1 = (Altar.Chants)Random.Range(0, 3);
        if (chant0 == chant1)
        {
            Debug.Log("Same chant in both altars, rerolling...");
            AssignChantToAltars();
            return;
        }
        Debug.Log("You will get " + chant0.ToString() + " and " + chant1.ToString());

        AltarList[0].SetChant(chant0);
        AltarList[1].SetChant(chant1);
    }

    private void EnableFirstRooms()
    {
        _actualRooms[0].gameObject.SetActive(true);
        _actualRooms[0].Exit.OpenDoor();
        _actualRooms[1].gameObject.SetActive(true);
        _actualRooms[1].Entry.OpenDoor();
        SetCurrentRoom(_actualRooms[0]);
    }

    public Room GetRoom(int i = 0)
    {
        return _actualRooms[Mathf.Clamp(CurrentRoom + i, 0, TotalRooms)];
    }

    public void SetCurrentRoom(Room room)
    {
        CurrentRoom = _actualRooms.IndexOf(room);
        AdjustColor();
    }

    public int GetRoomIndex(Room room)
    {
        return _actualRooms.IndexOf(room);
    }

    [Foldout("Colors")]
    [SerializeField] Color _easyColor;
    [Foldout("Colors")]
    [SerializeField] Color _mediumColor;
    [Foldout("Colors")]
    [SerializeField] Color _hardColor;
    [Foldout("Colors")]
    [Tooltip("En secondes, combien de temps la transition de couleur va prendre")]
    [SerializeField] float _colorTransitionSpeed = 4.0f;
    const int _transitionMedium = 3;
    const int _transitionHard = 7;

    bool _isColorTransitionning;
    float _colorTransitionT;
    Color _previousColor;
    Color _nextColor;

    [SerializeField] Light _fogLight;

    private void AdjustColor()
    {
        switch (CurrentRoom)
        {
            case 0:
                _isColorTransitionning = false;
                _fogLight.color = _easyColor;
                break;
            case _transitionMedium:
                TransitionToMedium();
                break;
            case _transitionHard:
                TransitionToHard();
                break;
        }
    }

    private void TransitionToMedium()
    {
        // Debug.Log("TransitionToMedium");
        _previousColor = _easyColor;
        _nextColor = _mediumColor;
        _isColorTransitionning = true;
        _colorTransitionT = 0.0f;
    }

    private void TransitionToHard()
    {
        Debug.Log("TransitionToHard");
        _previousColor = _mediumColor;
        _nextColor = _hardColor;
        _isColorTransitionning = true;
        _colorTransitionT = 0.0f;
    }

    private void UpdateColorTransition()
    {
        _colorTransitionT += Time.deltaTime / _colorTransitionSpeed;
        _fogLight.color = Color.Lerp(_previousColor, _nextColor, _colorTransitionT);
        if (_colorTransitionT >= 1.0f)
        {
            _isColorTransitionning = false;
        }
    }

    /*    public void Endgame()
        {
            Debug.Log("EndGame");
            TimerManager.instance.SaveTimer();
            MenuManager.Instance.OpenWin();
        }*/

    /// <summary>
    /// get seed all rooms
    /// </summary>
    /// <returns></returns>
    public List<Room> GetRooms()
    {
        return _roomsToBuild;
    }

    public int GetSeed()
    {
        return _seed;
    }
}
