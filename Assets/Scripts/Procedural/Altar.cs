using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Altar : MonoBehaviour, IInteractable
{
    //? Chants
    public enum Chants
    {
        Fugue,
        Muse,
        Cimetiere
    }

    [Foldout("References")]
    [SerializeField] private Collider _interactHitbox;

    [SerializeField] public Chants Chant;
    [Foldout("References for each Chant")]
    [SerializeField] private GameObject[] AestheticsParentsPerSong;
    [Foldout("References for each Chant")]
    [SerializeField] private MenuManager.Menus[] MenusPerSong;
    public MenuManager.Menus MenuToOpen { get; private set; }
    public GameObject AestheticsParent { get; private set; }

    //? Snapping
    [Foldout("Moving player in front of the altar")]
    [SerializeField] Transform _targetPositionReference;
    private Vector3 _initialPlayerPosition;
    private Quaternion _initialPlayerRotation;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private bool _shouldMovePlayer;
    private const float _lerpSpeed = 5.0f;
    private float _movementT;

    //? General
    private bool _hasBeenUsed;
    private bool _isPlayerInside;

    //[SerializeField] bool disableRandom;

    void Awake()
    {
        /* if(!disableRandom)
         {
             altarListScript.Add(this);
             SetRandomChant();
         }*/
        // Debug.Log(altarListScript.Count);
    }

    void Start()
    {
        SetChant(Chant);
        _shouldMovePlayer = false;
        _hasBeenUsed = false;
        _isPlayerInside = false;
        /*if (!disableRandom)
            CheckIfSameSpell();*/
    }

    void OnEnable()
    {
        SetRandomChant();
        CheckIfSameSpell();
    }

    void SetRandomChant()
    {
        Chant = (Chants)Random.Range(0, 3);
        //Debug.Log(_chant);
        //_chant = Chants.Muse;
    }

    public void SetChant(Chants chant)
    {
        Chant = chant;
        switch (chant)
        {
            case Chants.Fugue:
                AestheticsParent = AestheticsParentsPerSong[0];
                AestheticsParent.SetActive(true);
                MenuToOpen = MenusPerSong[0];
                break;
            case Chants.Muse:
                AestheticsParent = AestheticsParentsPerSong[1];
                AestheticsParent.SetActive(true);
                MenuToOpen = MenusPerSong[1];
                break;
            case Chants.Cimetiere:
                AestheticsParent = AestheticsParentsPerSong[2];
                AestheticsParent.SetActive(true);
                MenuToOpen = MenusPerSong[2];
                break;
        }
    }

    public void AddAltarToStaticList()
    {
        DungeonGenerator.Instance.AltarListScript.Add(this);
        Debug.Log(DungeonGenerator.Instance.AltarListScript.Count);
    }

    void CheckIfSameSpell()
    {
        if (DungeonGenerator.Instance.AltarListScript.Count > 0)
        {
            if (DungeonGenerator.Instance.AltarListScript[0].Chant == DungeonGenerator.Instance.AltarListScript[1].Chant)
            {
                DungeonGenerator.Instance.AltarListScript[1].Chant = (Chants)Random.Range(0, 3);
                Debug.Log("aaaaaaaa");
            }
        }
        else
            Debug.LogError("Altar list empty");
        //altarListScript[1]._chant = Chants.Cimetiere;
    }

    public void Interact()
    {
        if (!_hasBeenUsed)
        {
            PlayerManager.Instance.StartAltarMenuing(this);
            StartMovingPlayer();
            // _isPlayerInside = true;
        }
    }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         _isPlayerInside = false;
    //     }
    // }

    void Update()
    {
        if (_shouldMovePlayer && !_hasBeenUsed)
            MovePlayer();
    }

    private void StartMovingPlayer()
    {
        //Movement
        _initialPlayerPosition = Player.Instance.transform.position;
        _targetPosition = _targetPositionReference.position;
        _shouldMovePlayer = true;
        _movementT = 0.0f;

        //Rotation
        _initialPlayerRotation = Player.Instance.Head.rotation;
        _targetRotation = _targetPositionReference.rotation;
    }

    private void MovePlayer()
    {
        _movementT += Time.deltaTime * _lerpSpeed;

        Player.Instance.transform.position = (Vector3.Lerp(_initialPlayerPosition, _targetPosition, _movementT));
        Player.Instance.Head.rotation = Quaternion.Slerp(_initialPlayerRotation, _targetRotation, _movementT);

        if (_movementT >= 1)
            _shouldMovePlayer = false;
    }

    public void TurnOff()
    {
        _hasBeenUsed = true;
        PlayerHealth.Instance.TrueHeal(1);
        AestheticsParent.SetActive(false);
        _interactHitbox.enabled = false;

        // //if second altar
        // if (_altarListScript[1] == this)
        // {
        //     Debug.Log("this is indeed the second altar");
        //     //and if player has both upgrades
        //     bool cond = Player.Instance.CurrentArm != PlayerManager.Instance.Arms[0];
        //     cond = cond && Player.Instance.CurrentGun != PlayerManager.Instance.Guns[0];
        //     if (cond)
        //     {
        //         Debug.Log("player is upgraded enough");
        //         switch (_chant)
        //         {
        //             case Chants.Fugue:
        //                 TutorialManager.Instance.StartTutorial(TutorialManager.Tutorials.FugueSynergy);
        //                 break;
        //             case Chants.Muse:
        //                 TutorialManager.Instance.StartTutorial(TutorialManager.Tutorials.MuseSynergy);
        //                 break;
        //             case Chants.Cimetiere:
        //                 TutorialManager.Instance.StartTutorial(TutorialManager.Tutorials.EylauSynergy);
        //                 break;
        //         }
        //     }
        // }
    }

}