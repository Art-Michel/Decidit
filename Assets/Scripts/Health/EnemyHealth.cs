using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;
using State.AIBull;
using State.AICAC;
using State.FlyAI;
using State.WallAI;
using UnityEngine.UI;

public class EnemyHealth : Health
{
    [Foldout("References")]
    [SerializeField] Canvas _canvas;
    [Foldout("References")]
    [SerializeField] Material[] _material;
    [Foldout("References")]
    [SerializeField] VisualEffect _deathVfx;
    [Foldout("References")]
    public Room Room;
    [Foldout("References")]
    [SerializeField] List<CapsuleCollider> _hurtboxes;

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
    float _enableT = 1f;

    [Foldout("Synergies")]
    public bool IsSick;
    [Foldout("Synergies")]
    [SerializeField] Image _sickIcon;
    [Foldout("Synergies")]
    [SerializeField] List<BoxCollider> _sickboxes;

    [Header("KnockBack IA")]
    public Vector3 KnockBackDir;
    public GlobalRefAICAC globalRefAICAC;
    public GlobalRefBullAI globalRefBullAI;
    public GlobalRefFlyAI globalRefFlyAI;
    public GlobalRefWallAI globalRefWallAI;

    protected override void Awake()
    {
        base.Awake();
        _material = GetComponent<Material_Instances>().Material;

        _playerCamera = Camera.main.transform;
        _canvasGroup = _canvas.GetComponent<CanvasGroup>();
        _enableT = 1f;
    }

    protected override void Start()
    {
        base.Start();
        //_regenValue = _hp;
        _appearT = 0f;
        _canvasGroup.alpha = 0f;
        _isDying = false;

        if (_sickboxes.Count > 0)
            foreach (Collider collider in _sickboxes)
                collider.enabled = false;
    }

    [Button]
    private void FindHurtboxes()
    {
        _hurtboxes.Clear();
        foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
        {
            if (col.TryGetComponent<Hurtbox>(out Hurtbox box))
            {
                _hurtboxes.Add(col);
                box.HealthComponent = this;
            }
        }
    }

    [Button]
    private void FindSickboxes()
    {
        _sickboxes.Clear();
        foreach (BoxCollider col in GetComponentsInChildren<BoxCollider>())
        {
            if (col.TryGetComponent<SynergyTrigger>(out SynergyTrigger box))
            {
                _sickboxes.Add(col);
            }
        }
    }

    //[Button]
    private void DecalerSickBoxes()
    {
        foreach (BoxCollider box in _sickboxes)
        {
            GameObject son = new GameObject();
            son.transform.parent = box.transform;
            son.transform.localPosition = Vector3.zero;
            son.transform.localRotation = Quaternion.identity;
            son.transform.localScale = Vector3.zero;
            son.transform.name = ("Sickbox");
            son.layer = 21;

            SynergyTrigger trigger = son.AddComponent<SynergyTrigger>();
            trigger.Chant = Synergies.Chants.MUSE;

            BoxCollider newBox = son.AddComponent<BoxCollider>();
            newBox.size = box.size;
            newBox.center = box.center;
            newBox.isTrigger = true;

            Destroy(box);
        }
    }

    void OnEnable()
    {
        if (this.Room == null && DungeonGenerator.Instance != null)
            Debug.LogError("Room non assignée: l'ennemi [" + transform.name + "] dans la salle [" + DungeonGenerator.Instance.GetRoom(0).transform.name + "]");
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

    public override void TakeDamage(float amount)
    {

        if (globalRefBullAI != null)
            globalRefBullAI.CheckHP();

        else if (globalRefAICAC != null)
            globalRefAICAC.CheckHP();

        else if (globalRefFlyAI != null)
        {
            globalRefFlyAI.CheckHP();
            globalRefFlyAI.LaunchAttack();

        }
        else if (globalRefWallAI != null)
            globalRefWallAI.CheckHP();

        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/HitMarker", .2f, gameObject);
        //Stop probation from counting down when hitting an enemy
        //PlayerHealth.Instance.ResetProbStartup();
        base.TakeDamage(amount);
    }

    public override void TakeCriticalDamage(float amount)
    {
        base.TakeDamage(amount * 2);

        if (globalRefBullAI != null)
            globalRefBullAI.CheckHP();

        else if (globalRefAICAC != null)
            globalRefAICAC.CheckHP();

        else if (globalRefFlyAI != null)
        {
            globalRefFlyAI.CheckHP();
            globalRefFlyAI.LaunchAttack();

        }
        else if (globalRefWallAI != null)
            globalRefWallAI.CheckHP();

        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/HitMarkerHead", .8f, gameObject);
    }

    public override void Knockback(Vector3 direction)
    {
        KnockBackDir = direction;

        if (globalRefBullAI != null)
            globalRefBullAI.ActiveKnockBackState();
        else if (globalRefAICAC != null)
            globalRefAICAC.ActiveKnockBackState();
        else if (globalRefFlyAI != null)
            globalRefFlyAI.ActiveKnockBackState();
    }

    public void GetSick()
    {
        IsSick = true;
        _sickIcon.enabled = true;
        if (_sickboxes.Count > 0)
            foreach (Collider collider in _sickboxes)
                collider.enabled = true;
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
        if (!_isDying)
        {
            //formula to make distance-relative the angle at which you must look at an enemy
            float minimumDot = 1 - _lookStrictness / _distance;

            //Display Healthbar if enemy is looked at or if enemy is taking damage
            _healthBarIsVisible = Vector3.Dot(_camForward, (transform.position - _camPos).normalized) > minimumDot || _hasProbation;
        }
        else
        {
            _healthBarIsVisible = false;
            _disappearStartup = 0f;
        }

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

        //regen player
        SoundManager.Instance.PlaySound("event:/SFX_IA/DeathIA", 3f, gameObject);
        Player.Instance.gameObject.GetComponent<PlayerHealth>().ProbRegen(1);

        //old
        //Player.Instance.gameObject.GetComponent<Health>().ProbRegen(1000);

        //update number of enemies in room
        if (Room)
        {
            Room.CurrentEnemiesInRoom--;
            Room.CheckForEnemies();
        }
        else
            Debug.LogWarning("This enemy was not assigned a room");

        _deathT = _deathAnimationDuration;
        _isDying = true;
        _deathVfx.Play();
        foreach (Collider collider in _hurtboxes)
        {
            collider.enabled = false;
        }

        if (_sickboxes.Count < 0)
            foreach (Collider collider in _sickboxes)
                collider.enabled = false;

        //Adjust Visibility
        _appearT = 1;
    }

    public void SetDissolve()
    {
        for (int i = 0; i < _material.Length; i++)
        {
            _material[i].SetFloat("_clip", 1f);
        }
    }

    public IEnumerator DissolveInverse()
    {
        _enableT -= Time.deltaTime;
        for (int i = 0; i < _material.Length; i++)
        {
            _material[i].SetFloat("_clip", Mathf.Lerp(0, 1f, _enableT));
        }

        yield return new WaitForSeconds(0);
        if (_enableT <= 0)
        {
            Debug.Log(_enableT);
            yield break;
        }
        else
        {
            StartCoroutine("DissolveInverse");
        }
    }

    private void UpdateDeath()
    {
        _deathT -= Time.deltaTime;
        for (int i = 0; i < _material.Length; i++)
        {
            _material[i].SetFloat("_clip", Mathf.InverseLerp(_deathAnimationDuration, 0, _deathT));
        }
        if (_deathT <= 0)
        {
            ActuallyDie();
        }
    }

    private void ActuallyDie()
    {
        Destroy(gameObject);
    }
}
