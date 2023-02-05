using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyHealth : Health
{
    [Foldout("References")]
    [SerializeField] Canvas _canvas;
    [Foldout("References")]
    [SerializeField] Material _material;
    [Foldout("References")]
    [SerializeField] VisualEffect _deathVfx;
    [Foldout("References")]
    public Room Room;
    [Foldout("References")]
    [SerializeField] List<Collider> _colliders;

    float _regenValue;
    CanvasGroup _canvasGroup;
    Transform _playerCamera;
    Vector3 _camPos;
    Vector3 _camForward;
    float _distance;

    [Foldout("Ui Values")]
    [SerializeField][Range(0.05f, 0.6f)] float _uiScale = 0.1f;
    [Foldout("Ui Values")]
    [SerializeField][Range(0.0001f, 10f)] float _lookStrictness = 0.08f;
    [Foldout("Ui Values")]
    [SerializeField][Range(0.5f, 20f)] float _appearSpeed = 15f;
    [Foldout("Ui Values")]
    [SerializeField][Range(0.5f, 20f)] float _disappearSpeed = 4f;
    [Foldout("Ui Values")]
    [SerializeField][Range(0f, 2f)] float _disappearMaxStartup = 1f;
    [Foldout("Ui Values")]
    [SerializeField][Range(0f, 2f)] float _deathAnimationDuration = 1f;
    float _disappearStartup;
    float _appearT;
    bool _healthBarIsVisible;
    float _deathT;
    bool _isDying;

    protected override void Awake()
    {
        base.Awake();
        _regenValue = _hp;
        _playerCamera = Camera.main.transform;
        _canvasGroup = _canvas.GetComponent<CanvasGroup>();
    }

    private void FindRoom()
    {
        Room = GameObject.FindObjectOfType<Room>();
    }

    protected override void Start()
    {
        base.Start();
        _appearT = 0f;
        _canvasGroup.alpha = 0f;
        _isDying = false;
        _material = GetComponent<Material_Instances>().Material;
    }

    protected override void Update()
    {
        base.Update();
        _camPos = _playerCamera.transform.position;
        _camForward = _playerCamera.transform.forward;
        LookAtCamera();
        AdjustScale();
        AdjustVisibility();

        if (_isDying)
            UpdateDeath();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        PlaceHolderSoundManager.Instance.PlayHitSound();
    }

    public override void TakeCriticalDamage(int amount)
    {
        base.TakeCriticalDamage(amount);
        PlaceHolderSoundManager.Instance.PlayCriticalHitSound();
    }


    //used to make healthbar face the camera
    void LookAtCamera()
    {
        _canvas.transform.forward = _camForward;
    }

    void AdjustScale()
    {
        //grow or shrink depending on distance so the canvas size is consistent
        _distance = Vector3.Distance(_camPos, _canvas.transform.position);
        _canvas.transform.localScale = Vector3.one * _distance * _uiScale;
    }

    ///<summary> Display Healthbar when looking at an enemy or when they take damage </summary>
    void AdjustVisibility()
    {
        float minimumDot = 1 - _lookStrictness / _distance; //formula to make distance-relative the angle at which you must look at an enemy
        //Display Healthbar if enemy is looked at or if enemy is taking damage
        _healthBarIsVisible = Vector3.Dot(_camForward, (transform.position - _camPos).normalized) > minimumDot || _hasProbation;

        //progressively display healthbar
        if (_healthBarIsVisible && _appearT < 1)
        {
            _appearT = Mathf.Clamp01(_appearT + Time.deltaTime * _appearSpeed);
            _canvasGroup.alpha = Mathf.Lerp(0, 1, _appearT);
            _disappearStartup = _disappearMaxStartup;
        }

        //progressively undisplay healthbar
        if (!_healthBarIsVisible && _appearT > 0)
        {
            if (_disappearStartup > 0f)
            {
                _disappearStartup -= Time.deltaTime;
                return;
            }

            _appearT = Mathf.Clamp01(_appearT - Time.deltaTime * _disappearSpeed);
            _canvasGroup.alpha = Mathf.Lerp(0, 1, _appearT);
        }
    }

    protected override void Death()
    {
        if (_isDying)
            return;

        _deathT = _deathAnimationDuration;
        _isDying = true;
        _deathVfx.Play();
        foreach (Collider collider in _colliders)
        {
            collider.enabled = false;
        }

        //update number of enemies in room
        if (Room)
        {
            Room.CurrentEnemiesInRoom--;
            Room.CheckForEnemies();
        }

        //regen player
        Player.Instance.gameObject.GetComponent<Health>().ProbRegen(Mathf.RoundToInt(_regenValue / 4f));
    }

    private void UpdateDeath()
    {
        _deathT -= Time.deltaTime;
        _material.SetFloat("_clip", Mathf.InverseLerp(_deathAnimationDuration, 0, _deathT));
        if (_deathT <= 0)
            Destroy(gameObject);
    }

}
