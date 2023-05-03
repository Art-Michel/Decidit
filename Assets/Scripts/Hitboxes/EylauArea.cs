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
    [Foldout("References")][SerializeField] private LayerMask _shouldBuff;
    [Foldout("Stats")][SerializeField] private float _radius;
    [Foldout("Stats")][SerializeField] private float _lifeSpan;
    private float _lifeT;
    private bool _isPlayerInHere = false;
    private bool _wasPlayerInHere = false;

    private List<GlobalRefAICAC> _aiCacInArea;
    private List<GlobalRefBullAI> _aiBullInArea;
    private List<GlobalRefFlyAI> _aiFlyInArea;
    private List<GlobalRefWallAI> _aiWallInArea;

    private void Awake()
    {
        _aiCacInArea = new List<GlobalRefAICAC>();
        _aiBullInArea = new List<GlobalRefBullAI>();
        _aiFlyInArea = new List<GlobalRefFlyAI>();
        _aiWallInArea = new List<GlobalRefWallAI>();
    }

    public void Reset()
    {
        _lifeT = _lifeSpan;

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
        CheckForPlayer();

        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
            Disappear();
    }

    private void Disappear()
    {
        Reset();
        gameObject.SetActive(false);
    }

    #region worst detection of all time due to AIs not having a commmon parent class
    void OnTriggerEnter(Collider other)
    {
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
        _isPlayerInHere = Physics.OverlapSphere(transform.position, _radius, _shouldBuff).Length > 0;
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
