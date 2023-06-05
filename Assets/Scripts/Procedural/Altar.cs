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

    [SerializeField] public Chants _chant;
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

    [SerializeField] static List<Altar> altarListScript = new List<Altar>();

    //[SerializeField] bool disableRandom;

    void Awake()
    {
        /* if(!disableRandom)
         {
             altarListScript.Add(this);
             SetRandomChant();
         }*/
        altarListScript.Add(this);
        SetRandomChant();
        // Debug.Log(altarListScript.Count);
    }

    void Start()
    {
        SetChant(_chant);
        _shouldMovePlayer = false;
        _hasBeenUsed = false;
        _isPlayerInside = false;
        CheckIfSameSpell();
        /*if (!disableRandom)
            CheckIfSameSpell();*/
    }

    void SetRandomChant()
    {
        //_chant = (Chants)Random.Range(0, 3);
        //Debug.Log(_chant);
        _chant = Chants.Muse;
    }

    public void SetChant(Chants chant)
    {
        _chant = chant;
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

    void CheckIfSameSpell()
    {
        while (altarListScript[0]._chant == altarListScript[1]._chant)
        {
            altarListScript[1]._chant = (Chants)Random.Range(0, 3);
        }
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
    }

    void OnDestroy()
    {
        altarListScript.Clear();
    }
}