using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using State.AIBull;
using State.AICAC;
using State.FlyAI;
using State.WallAI;
using NaughtyAttributes;

public class EylauArea : SynergyTrigger
{
    //Basic functionning
    [Foldout("References")][SerializeField] private LayerMask _shouldBuff;
    [Foldout("References")][SerializeField] private Material _material;

    [Foldout("Stats")] public float Radius = 8;
    [Foldout("Stats")][SerializeField] private float _lifeSpan;
    [Foldout("Stats")][SerializeField] private Vector3 _defaultScale;
    [Foldout("Stats")][SerializeField] private float _defaultAlpha;
    [Foldout("Stats")][SerializeField] public bool IsActive;
    [Foldout("Stats")][SerializeField] private AnimationCurve _alphaAppearanceCurve;
    [Foldout("Stats")][SerializeField] private AnimationCurve _sizeAppearanceCurve;
    private float _lifeT;
    private bool _isPlayerInHere = false;
    private bool _wasPlayerInHere = false;

    //Lists
    private List<GlobalRefAICAC> _aiCacInArea;
    private List<GlobalRefBullAI> _aiBullInArea;
    private List<GlobalRefFlyAI> _aiFlyInArea;
    private List<GlobalRefWallAI> _aiWallInArea;


    //Macron trou noir
    [Foldout("Explosion")][SerializeField] private bool _blewUp;
    [Foldout("Explosion")][SerializeField] private AnimationCurve _explosionDisappearanceCurve;
    [Foldout("Explosion")][SerializeField] private AnimationCurve _explosionSizeCurve;
    [Foldout("Explosion")][SerializeField] private float _explosionSpeed;

    //Macron Explosion
    [Foldout("Blackhole")][SerializeField] private bool _blackHoled;
    [Foldout("Blackhole")][SerializeField] private AnimationCurve _blackHoleDisappearanceCurve;
    [Foldout("Blackhole")][SerializeField] private AnimationCurve _blackHoleSizeCurve;
    [Foldout("Blackhole")][SerializeField] private float _blackHoleSpeed;

    float _disappearanceT;

    private void Awake()
    {
        _aiCacInArea = new List<GlobalRefAICAC>();
        _aiBullInArea = new List<GlobalRefBullAI>();
        _aiFlyInArea = new List<GlobalRefFlyAI>();
        _aiWallInArea = new List<GlobalRefWallAI>();
    }

    private void OnEnable()
    {
        Reset();
        IsActive = true;
    }

    public void Reset()
    {
        _lifeT = 0.0f;
        _blackHoled = false;
        _blewUp = false;

        if (_isPlayerInHere)
            Player.Instance.StopEylauBuff();

        _isPlayerInHere = false;
        _wasPlayerInHere = false;

        //again terrible terrible
        foreach (GlobalRefAICAC aiCac in _aiCacInArea)
            aiCac.isInEylau = false;
        foreach (GlobalRefBullAI aiBull in _aiBullInArea)
            aiBull.isInEylau = false;
        foreach (GlobalRefFlyAI aiFly in _aiFlyInArea)
            aiFly.isInEylau = false;
        foreach (GlobalRefWallAI aiWall in _aiWallInArea)
            aiWall.isInEylau = false;

        _aiCacInArea.Clear();
        _aiBullInArea.Clear();
        _aiFlyInArea.Clear();
        _aiWallInArea.Clear();
    }

    void Update()
    {
        if (IsActive)
        {
            UpdateAppearance();
            CheckForPlayer();

            _lifeT += Time.deltaTime;
            if (_lifeT >= _lifeSpan)
                Disappear();
        }
        else if (_blackHoled)
            UpdateBlackHoleDisappearance();
        else if (_blewUp)
            UpdateExplosionDisappearance();
    }

    private void UpdateAppearance()
    {
        transform.localScale = _defaultScale * _sizeAppearanceCurve.Evaluate(_lifeT);
        float alpha = _defaultAlpha * _alphaAppearanceCurve.Evaluate(_lifeT);
        _material.SetFloat("_Opacity", alpha);
    }

    //Synergizing
    public void StartBlackHoleDisappearance()
    {
        _disappearanceT = 0.0f;
        _blackHoled = true;
        IsActive = false;
        // _defaultScale = transform.localScale;
        // _defaultAlpha = 1;
    }

    public void StartExplosionDisappearance()
    {
        _disappearanceT = 0.0f;
        _blewUp = true;
        IsActive = false;
        // _defaultScale = transform.localScale;
        // _defaultAlpha = 1;
    }


    private void UpdateExplosionDisappearance()
    {
        _disappearanceT += Time.deltaTime * _explosionSpeed;
        transform.localScale = _defaultScale * _explosionSizeCurve.Evaluate(_disappearanceT);
        float alpha = _defaultAlpha * _explosionSizeCurve.Evaluate(_disappearanceT);
        _material.SetFloat("_Opacity", alpha);
        if (_disappearanceT >= 1.0f)
            Disappear();
    }

    private void UpdateBlackHoleDisappearance()
    {
        _disappearanceT += Time.deltaTime * _explosionSpeed;
        transform.localScale = _defaultScale * _blackHoleSizeCurve.Evaluate(_disappearanceT);
        float alpha = _defaultAlpha * _blackHoleDisappearanceCurve.Evaluate(_disappearanceT);
        _material.SetFloat("_Opacity", alpha);
        if (_disappearanceT >= 1.0f)
            Disappear();
    }

    private void Disappear()
    {
        Reset();
        IsActive = false;
        gameObject.SetActive(false);
    }

    // Buff and Debuff
    #region worst detection of all time due to AIs not having a commmon parent class
    void OnTriggerEnter(Collider other)
    {
        if (!IsActive)
            return;

        if (other.CompareTag("Ennemi"))
        {
            // Debug.Log(other.transform.name + " entered");
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/IAInOut", 1f, gameObject);
            if (other.TryGetComponent<GlobalRefAICAC>(out GlobalRefAICAC cacAi))
            {
                cacAi.isInEylau = true;
                _aiCacInArea.Add(cacAi);
            }
            else if (other.TryGetComponent<GlobalRefBullAI>(out GlobalRefBullAI bullAi))
            {
                bullAi.isInEylau = true;
                _aiBullInArea.Add(bullAi);
            }
            else if (other.TryGetComponent<GlobalRefFlyAI>(out GlobalRefFlyAI flyAi))
            {
                flyAi.isInEylau = true;
                _aiFlyInArea.Add(flyAi);
            }
            // else if (other.TryGetComponent<GlobalRefWallAI>(out GlobalRefWallAI wallAi))
            // {
            //     wallAi.isInEylau = true;
            //     _aiWallInArea.Add(wallAi);
            // }

            else
                Debug.Log("exited enemy but couldnt find ai script");
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (!IsActive)
            return;

        if (other.CompareTag("Ennemi"))
        {
            // Debug.Log(other.transform.name + " exited");
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/IAInOut", 1f, gameObject);
            if (other.TryGetComponent<GlobalRefAICAC>(out GlobalRefAICAC cacAi))
            {
                cacAi.isInEylau = false;
                _aiCacInArea.Remove(cacAi);
            }
            else if (other.TryGetComponent<GlobalRefBullAI>(out GlobalRefBullAI bullAi))
            {
                bullAi.isInEylau = false;
                _aiBullInArea.Remove(bullAi);
            }
            else if (other.TryGetComponent<GlobalRefFlyAI>(out GlobalRefFlyAI flyAi))
            {
                flyAi.isInEylau = false;
                _aiFlyInArea.Remove(flyAi);
            }
            // else if (other.TryGetComponent<GlobalRefWallAI>(out GlobalRefWallAI wallAi))
            // {
            //     wallAi.isInEylau = false;
            //     _aiWallInArea.Remove(wallAi);
            // }

            else
                Debug.Log("exited enemy but couldnt find ai script");
        }
    }
    #endregion

    private void CheckForPlayer()
    {
        if (!IsActive)
            return;

        _isPlayerInHere = Physics.OverlapSphere(transform.position, Radius, _shouldBuff).Length > 0;
        if (!_isPlayerInHere && _wasPlayerInHere)
        {
            Player.Instance.StopEylauBuff();
        }
        if (_isPlayerInHere && !_wasPlayerInHere)
        {
            Player.Instance.EylauMovementBuff();
        }
        _wasPlayerInHere = _isPlayerInHere;
    }
}
