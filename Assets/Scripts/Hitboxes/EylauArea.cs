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

public class EylauArea : MonoBehaviour
{
    [Foldout("References")][SerializeField] private LayerMask _shouldEnhance;
    [Foldout("References")][SerializeField] private LayerMask _shouldBuff;
    [Foldout("Stats")][SerializeField] private float _radius;
    [Foldout("Stats")][SerializeField] private float _lifeSpan;
    private float _lifeT;
    private bool _isPlayerInHere = false;
    private bool _wasPlayerInHere = false;

    void OnEnable()
    {
        _lifeT = _lifeSpan;
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
        gameObject.SetActive(false);
    }

    #region worst detection of all time due to AIs not having a commmon parent class
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            Debug.Log(other.transform.name + " entered");
            if (other.TryGetComponent<GlobalRefAICAC>(out GlobalRefAICAC cacAi))
            {
                cacAi.isInEylau = true;
            }
            else if (other.TryGetComponent<GlobalRefBullAI>(out GlobalRefBullAI bullAi))
            {
                bullAi.isInEylau = true;
            }
            else if (other.TryGetComponent<GlobalRefFlyAI>(out GlobalRefFlyAI flyAi))
            {
                flyAi.isInEylau = true;
            }
            else if (other.TryGetComponent<GlobalRefWallAI>(out GlobalRefWallAI wallAi))
            {
                wallAi.isInEylau = true;
            }

            else
                Debug.Log("exited enemy but couldnt find ai script");
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            Debug.Log(other.transform.name + " exited");
            if (other.TryGetComponent<GlobalRefAICAC>(out GlobalRefAICAC cacAi))
            {
                cacAi.isInEylau = false;
            }
            else if (other.TryGetComponent<GlobalRefBullAI>(out GlobalRefBullAI bullAi))
            {
                bullAi.isInEylau = false;
            }
            else if (other.TryGetComponent<GlobalRefFlyAI>(out GlobalRefFlyAI flyAi))
            {
                flyAi.isInEylau = false;
            }
            else if (other.TryGetComponent<GlobalRefWallAI>(out GlobalRefWallAI wallAi))
            {
                wallAi.isInEylau = false;
            }

            else
                Debug.Log("exited enemy but couldnt find ai script");
        }
    }
    #endregion

    private void CheckForPlayer()
    {
        _isPlayerInHere = Physics.OverlapCapsule(transform.position + Vector3.down * 100, transform.position + Vector3.up * 100, _radius, _shouldBuff).Length > 0;
        if (!_isPlayerInHere && _wasPlayerInHere)
        {
            Player.Instance.ResetMovement();
        }
        if (_isPlayerInHere && !_wasPlayerInHere)
        {
            Player.Instance.BuffMovement();
        }
        _wasPlayerInHere = _isPlayerInHere;
    }
}
