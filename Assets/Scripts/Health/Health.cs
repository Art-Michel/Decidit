using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Foldout("References")]
    [SerializeField] protected Image _hpUi;
    [Foldout("References")]
    [SerializeField] protected Image _probHpUi;
    [Foldout("References")]
    [SerializeField] private Pooler _bloodVFXPooler;
    [Foldout("References")]
    [SerializeField] Image _hpCursor;
    [Foldout("References")]
    [SerializeField] RectTransform _hpBarStart;
    [Foldout("References")]
    [SerializeField] RectTransform _hpBarEnd;

    [Foldout("Stats")]
    [Range(1, 1000)][SerializeField] protected float _maxHp = 100;
    [Foldout("Stats")]
    [Range(0, 3)][SerializeField] protected float _probationMaxStartup = 1;
    [Foldout("Stats")]
    [Range(0.0f, 60)][SerializeField] float _probationSpeed = 15;
    [Foldout("Stats")]
    [SerializeField] private float _healthBarSpeed = 2.0f;
    protected float _hpBefore;
    private float _healthBarCurrentSpeed;

    public float _hp { get; protected set; }
    protected float _probHp;
    protected bool _hasProbation;
    protected float _probationStartup;
    public bool IsInvulnerable;
    private float _healthT;

    protected virtual void Awake()
    {
        _hp = _maxHp;
    }

    protected virtual void Start()
    {
        //IsInvulnerable = false;
        _healthT = 0f;
        _hpBefore = 0.0f;
        _probHp = _hp;
        _hasProbation = false;
        ResetBarFillage(false);
        DisplayProbHealth();
    }

    protected virtual void Update()
    {
        if (_hasProbation)
            UpdateProbHealth();
        DisplayHealth();
    }

    public virtual bool TakeDamage(float damage)
    {
        if (IsInvulnerable)
            return false;

        _hpBefore = Mathf.InverseLerp(0, _maxHp, _hp);
        _hp -= damage;
        ResetBarFillage(true);
        DisplayProbHealth();
        StartProbHealth();
        //Debug.Log("Received " + damage + " damage");

        if (_hp <= 0)
        {
            _hp = 0f;
            Death();
        }
        return true;
    }

    public virtual bool TakeCriticalDamage(float damage)
    {
        TakeDamage(damage * 3);
        return true;
    }

    public bool TakeDamage(float amount, Vector3 position, Vector3 forward)
    {
        this.TakeDamage(amount);
        SplashBlood(position, forward);
        return true;
    }

    public bool TakeCriticalDamage(float amount, Vector3 position, Vector3 forward)
    {
        this.TakeCriticalDamage(amount);
        SplashBlood(position, forward);
        return true;
    }

    public virtual void Knockback(Vector3 direction)
    {

    }

    private void SplashBlood(Vector3 position, Vector3 forward)
    {
        if (!_bloodVFXPooler)
        {
            Debug.LogWarning("No bloodFX Pooler script found on same object as this script.");
            return;
        }

        Transform splash = _bloodVFXPooler.Get().transform;
        splash.position = position;
        splash.forward = forward;
    }

    [Button]
    public virtual void ProbRegen(int amount = 1)
    {
        if (_hp < _probHp)
        {
            _hpBefore = Mathf.InverseLerp(0, _maxHp, _hp);
            _hp = Mathf.Clamp(_hp + amount, 0, _probHp);
            ResetBarFillage(false);
            DisplayProbHealth();
            //StartProbHealth(); //*uncomment if we want to reset prob timer upon regen
        }
    }

    protected void ResetBarFillage(bool tookDamage)
    {
        _healthT = 0.0f;
        if (!tookDamage)
            _healthBarCurrentSpeed = Mathf.InverseLerp(0, _maxHp, _hp) - _hpBefore;
        else
            _healthBarCurrentSpeed = _hpBefore - Mathf.InverseLerp(0, _maxHp, _hp);
    }

    protected virtual void DisplayHealth()
    {
        if (_hpUi)
        {
            _healthT += (Time.deltaTime * _healthBarSpeed) / _healthBarCurrentSpeed;
            // _hpUi.fillAmount = Mathf.InverseLerp(0, _maxHp, _hp);
            _hpUi.fillAmount = Mathf.Lerp(_hpBefore, Mathf.InverseLerp(0, _maxHp, _hp), _healthT);
        }
        if (_hpCursor)
            _hpCursor.rectTransform.anchoredPosition = Vector2.Lerp(_hpBarStart.anchoredPosition, _hpBarEnd.anchoredPosition, Mathf.InverseLerp(0, _maxHp, _hp));
    }

    protected void StartProbHealth()
    {
        if (!_hasProbation)
            _probationStartup = _probationMaxStartup;
        _hasProbation = true;
    }

    private void UpdateProbHealth()
    {
        if (_probationStartup > 0)
        {
            _probationStartup -= Time.deltaTime;
            return;
        }

        else
        {
            _probHp -= _probationSpeed * Time.deltaTime;
            if (_probHp < _hp)
            {
                _hasProbation = false;
                _probHp = _hp;
            }
            DisplayProbHealth();
        }
    }

    protected virtual void DisplayProbHealth()
    {
        if (_probHpUi)
        {
            _probHpUi.fillAmount = Mathf.InverseLerp(0, _maxHp, _probHp);
        }
    }

    protected virtual void Death()
    {

    }
}
