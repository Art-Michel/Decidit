using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Health : MonoBehaviour
{
    [Foldout("References")]
    [SerializeField] protected Image _hpUi;
    [Foldout("References")]
    [SerializeField] protected Image _probHpUi;

    [Foldout("Stats")]
    [Range(1, 300)][SerializeField] protected float _maxHp = 100;
    [Foldout("Stats")]
    [Range(0, 3)][SerializeField] float _probationMaxStartup = 1;
    [Foldout("Stats")]
    [Range(0.1f, 60)][SerializeField] float _probationSpeed = 15;

    public float _hp { get; private set; }
    protected float _probHp;
    protected bool _hasProbation;
    protected float _probationStartup;


    protected virtual void Start()
    {
        _hp = _maxHp;
        _probHp = _hp;
        _hasProbation = false;
        DisplayHealth();
        DisplayProbHealth();
    }

    protected virtual void Update()
    {
        if (_hasProbation)
            UpdateProbHealth();
    }

    public virtual void TakeDamage(int damage)
    {
        _hp -= damage;
        DisplayHealth();

        if (_hp <= 0)
        {
            _hp = 0f;
            Death();
            return;
        }

        StartProbHealth();
    }

    public virtual void TakeCriticalDamage(int damage)
    {
        _hp -= damage * 2;
        DisplayHealth();

        if (_hp <= 0)
        {
            _hp = 0f;
            Death();
            return;
        }

        StartProbHealth();
    }

    public virtual void Knockback(Vector3 direction)
    {

    }

    public void ProbRegen(int amount = 10)
    {
        if (_hp < _probHp)
        {
            _hp = Mathf.Clamp(_hp + amount, 0, _probHp);
            DisplayHealth();
            PlaceHolderSoundManager.Instance.PlayRegen();
            //StartProbHealth(); //uncomment if we want to reset prob timer upon regen
        }
    }

    protected virtual void DisplayHealth()
    {
        if (_hpUi)
        {
            _hpUi.fillAmount = Mathf.InverseLerp(0, _maxHp, _hp);
        }
    }

    protected void StartProbHealth()
    {
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

    private void DisplayProbHealth()
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
