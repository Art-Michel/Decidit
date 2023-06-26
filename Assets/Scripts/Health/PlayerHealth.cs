using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;
using CameraShake;

public class PlayerHealth : Health
{
    public static PlayerHealth Instance;

    [Foldout("Health Frame")]
    [SerializeField] private Sprite[] _easyFrames;
    [Foldout("Health Frame")]
    [SerializeField] private Sprite[] _mediumFrames;
    [Foldout("Health Frame")]
    [SerializeField] private Sprite[] _hardFrames;
    private Sprite[] _currentFrames;
    [Foldout("Health Frame")]
    [SerializeField] private Image _currentFrame;
    [Foldout("Health Frame")]
    [SerializeField] private Image _previousFrame;
    [Foldout("Health Frame")]
    [SerializeField] float _lifeUiChangeSpeed = 0.0f;
    float _lifeUiChangeT = 0.0f;

    [Foldout("Health Frame Dividers")]
    [SerializeField] private Sprite[] _easyDividers;
    [Foldout("Health Frame Dividers")]
    [SerializeField] private Sprite[] _mediumDividers;
    [Foldout("Health Frame Dividers")]
    [SerializeField] private Sprite[] _hardDividers;
    private Sprite[] _currentDividers;
    [Foldout("Health Frame Dividers")]
    [SerializeField] private Image _currentDivider;
    [Foldout("Health Frame Dividers")]
    [SerializeField] private Image _previousDivider;


    [Foldout("References")]
    [SerializeField] Player _player;

    [Foldout("References")]
    [SerializeField] List<Collider> _colliders;

    [Foldout("References")]
    [SerializeField] Image _lowHpVignette;

    [Foldout("References")]
    [SerializeField] Image _probVignette;
    [Foldout("Properties")]
    [SerializeField] AnimationCurve _vignetteAlphaOnProb;
    Vector3 _probVignetteColor;

    [Foldout("References")]
    [SerializeField] Image _healVignette;
    [Foldout("Properties")]
    [SerializeField] AnimationCurve _vignetteAlphaOnHeal;
    private float _healVignetteT;
    private bool _isHealing;
    private const float _healVignetteSpeed = 1.5f;

    [Foldout("References")]
    [SerializeField] Image _damageVignette;
    [Foldout("Properties")]
    [SerializeField] AnimationCurve _vignetteAlphaOnDamage;
    private float _damageVignetteT;
    private bool _isBeingDamaged;
    private const float _damageVignetteSpeed = 2.5f;
    private float _currentDamageVignetteMaxAlpha;

    [Foldout("Stats")]
    [SerializeField]
    private bool _hasSecondChance;

    private float _hurtInvulnerability;

    private float _hurtInvulnerabilityT;
    private bool _isHurtInvulnerable;
    public bool _isDebugInvulnerable;

    [Foldout("Difficulty Modifiers")]
    [SerializeField] private float _easyHp = 5;
    [Foldout("Difficulty Modifiers")]
    [SerializeField] private float _mediumHp = 3;
    [Foldout("Difficulty Modifiers")]
    [SerializeField] private float _hardHp = 3;

    [Foldout("Difficulty Modifiers")]
    [SerializeField] private float _easyInvulTime = 0.6f;
    [Foldout("Difficulty Modifiers")]
    [SerializeField] private float _mediumInvulTime = 0.3f;
    [Foldout("Difficulty Modifiers")]
    [SerializeField] private float _hardInvulTime = 0.1f;


    [SerializeField]
    private BounceShake.Params _hurtShake;

    Vector3 _damageVignetteColor;
    float[] _vignetteOpacityPerHpLeft = { 0.8f, 0.8f, 0.3f, 0.0f, 0.0f, 0.0f };

    //* Unused!
    // [Foldout("Stats")]
    // [SerializeField]
    // [Tooltip("How much Timescale will slow down when player gets hit. Lower is stronger.")]
    // private float _playerHurtFreezeStrength = 0.01f;
    // [Foldout("Stats")]
    // [SerializeField]
    // [Tooltip("For how long Timescale will slow down when player gets hit.")]
    // private float _playerHurtFreezeDuration = 0.2f;

    protected override void Awake()
    {
        AdaptToDifficulty();
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = GetComponent<PlayerHealth>();
        base.Awake();
    }

    [Button]
    private void FindBoxes()
    {
        _colliders.Clear();
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (col.TryGetComponent<Hurtbox>(out Hurtbox box))
            {
                _colliders.Add(col);
                box.HealthComponent = this;
            }
        }
    }

    protected override void Start()
    {
        _damageVignetteColor = new Vector3(_damageVignette.color.r, _damageVignette.color.g, _damageVignette.color.b);
        _probVignetteColor = new Vector3(_probVignette.color.r, _probVignette.color.g, _probVignette.color.b);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (_isHealing) HandleHealVignette();
        if (_isBeingDamaged) HandleDamageVignette();
        if (_isHurtInvulnerable) HandleHurtInvul();
        UpdateChangeLifeFrame();
    }

    public override bool TakeDamage(float amount)
    {
        // SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/DamageTaken", 1f, gameObject);
        if (IsInvulnerable || amount <= 0 || _isHurtInvulnerable || _isDebugInvulnerable)
            return false;

        if (_hasSecondChance && amount >= _hp && _hp > 1)
            amount = _hp - 1;

        //Lose all probation health when hit a second time
        _probHp = _hp;
        ResetProbStartup();

        base.TakeDamage(amount);

        Player.Instance.StartBounceShake(_hurtShake, transform.position);

        HandleLowHpVignette();
        StartDamageVignette(amount);
        StartHurtInvul();
        DisplayProbHealth();
        return true;
    }

    public void DirectionalHurtSound(GameObject go)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/DamageTaken", 1f, go);
    }

    public void ResetProbStartup()
    {
        _probationStartup = _probationMaxStartup;
    }

    private void HandleLowHpVignette()
    {
        _lowHpVignette.color = new Color(1.0f, 1.0f, 1.0f, _vignetteOpacityPerHpLeft[Mathf.RoundToInt(_hp)]);
    }

    [Button]
    public void TrueHeal(float i = 1)
    {
        if (_hp < _maxHp)
        {
            _hpBefore = Mathf.InverseLerp(0, _maxHp, _hp);
            _hp = Mathf.Clamp(_hp + i, 0, _maxHp);

            ResetBarFillage(false);

            DisplayProbHealth();
            SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/FullHeal", 1f, gameObject);
            StartHealVignette();
            HandleLowHpVignette();
            if (PlayerManager.Instance._isDying)
            {
                PlayerManager.Instance.CancelDeath();
            }
        }
    }

    public override void ProbRegen(int i)
    {
        if (_hp < _probHp)
        {
            base.ProbRegen(i);
            SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/BaseHeal", 1f, gameObject);
            HandleLowHpVignette();
            StartHealVignette();
            if (PlayerManager.Instance._isDying)
            {
                PlayerManager.Instance.CancelDeath();
            }
        }
    }

    private void StartHealVignette()
    {
        _isHealing = true;
        _healVignetteT = 0.0f;
    }

    private void HandleHealVignette()
    {
        if (_healVignetteT <= 1)
        {
            _healVignetteT += Time.deltaTime * _healVignetteSpeed;
            float alpha = _vignetteAlphaOnHeal.Evaluate(_healVignetteT);
            _healVignette.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
        else
        {
            _healVignetteT = 1.0f;
            _healVignette.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            _isHealing = false;
        }
    }

    private void StartDamageVignette(float damage)
    {
        _isBeingDamaged = true;
        _damageVignetteT = 0.0f;

        //*Minimum vignette intensity from 1hp lost (80% opacity) to 2 hp lost (100% opacity)
        _currentDamageVignetteMaxAlpha = Mathf.Lerp(0.8f, 1.0f, Mathf.InverseLerp(1.0f, 2.0f, damage));
    }

    private void HandleDamageVignette()
    {
        if (_damageVignetteT <= 1)
        {
            _damageVignetteT += Time.deltaTime * _damageVignetteSpeed;
            float alpha = _vignetteAlphaOnDamage.Evaluate(_damageVignetteT);
            _damageVignette.color = new Color(_damageVignetteColor.x, _damageVignetteColor.y, _damageVignetteColor.z, alpha * _currentDamageVignetteMaxAlpha);
        }
        else
        {
            _damageVignetteT = 1.0f;
            _damageVignette.color = new Color(_damageVignetteColor.x, _damageVignetteColor.y, _damageVignetteColor.z, 0.0f);
            _isBeingDamaged = false;
        }
    }

    private void StartHurtInvul()
    {
        if (DungeonGenerator.Instance != null && DungeonGenerator.Instance.GetRoom().CurrentEnemiesInRoom > 0)
            SoundManager.Instance.SoundAfterHit();
        _hurtInvulnerabilityT = _hurtInvulnerability;
        _isHurtInvulnerable = true;
    }

    private void HandleHurtInvul()
    {
        _hurtInvulnerabilityT -= Time.deltaTime;
        if (_hurtInvulnerabilityT <= 0.0f)
        {
            _isHurtInvulnerable = false;
            SoundManager.Instance.PlaySound("event:/SFX_Controller/InvulEnd", 1.0f, gameObject);
            if (DungeonGenerator.Instance != null && DungeonGenerator.Instance.GetRoom().CurrentEnemiesInRoom > 0)
                SoundManager.Instance.SoundEndHit();
        }
    }

    protected override void DisplayProbHealth()
    {
        base.DisplayProbHealth();
        float alpha = _vignetteAlphaOnProb.Evaluate(_probHp - _hp);
        _probVignette.color = new Color(_probVignetteColor.x, _probVignetteColor.y, _probVignetteColor.z, alpha);
    }

    public override void Knockback(Vector3 direction)
    {
        if (_isDebugInvulnerable)
            return;
        _player.AddMomentum(direction);
    }

    protected override void Death()
    {
        PlayerManager.Instance.StartDying();
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    private void AdaptToDifficulty()
    {
        switch (ApplyDifficulty.indexDifficulty)
        {
            case 0:
                _maxHp = _easyHp;
                _currentFrames = _easyFrames;
                _currentDividers = _easyDividers;
                _hurtInvulnerability = _easyInvulTime;
                break;
            case 1:
                _maxHp = _mediumHp;
                _currentFrames = _mediumFrames;
                _currentDividers = _mediumDividers;
                _hurtInvulnerability = _mediumInvulTime;
                break;
            case 2:
                _maxHp = _hardHp;
                _currentFrames = _hardFrames;
                _currentDividers = _hardDividers;
                _hurtInvulnerability = _hardInvulTime;
                break;
        }

        _currentDivider.sprite = _currentDividers[0];
        _currentFrame.sprite = _currentFrames[0];
        StartChangeLifeFrame(0);
    }

    public void StartChangeLifeFrame(int advance)
    {
        _previousFrame.sprite = _currentFrame.sprite;
        _previousDivider.sprite = _currentDivider.sprite;

        _currentFrame.sprite = _currentFrames[advance];
        _currentDivider.sprite = _currentDividers[advance];

        _previousFrame.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        _currentFrame.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        _lifeUiChangeT = 0.0f;
    }

    private void UpdateChangeLifeFrame()
    {
        if (_lifeUiChangeT >= 1)
            return;

        _lifeUiChangeT += Time.deltaTime * _lifeUiChangeSpeed;

        _previousFrame.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - _lifeUiChangeT);
        _currentFrame.color = new Color(1.0f, 1.0f, 1.0f, _lifeUiChangeT);
    }

    /// <summary>
    /// Return the amount of hp
    /// </summary>
    /// <returns></returns>
    public float GetHP()
    {
        return _hp;
    }

}
