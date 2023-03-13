using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InteractionRay : MonoBehaviour
{
    [SerializeField] float _interactableDetectionRange = 10.0f;
    [SerializeField] float _interactRange = 5.0f;
    [SerializeField] private Transform _interactable;
    [SerializeField] private bool _isCloseEnough;
    [SerializeField] LayerMask _mask;
    private PlayerInputMap _inputs;
    void Awake()
    {
        _inputs = new PlayerInputMap();
        _inputs.Actions.Interact.started += _ => Interact();
    }

    void Update()
    {
        if (CheckForInteractable())
            CheckForInteractableRange();
    }

    private bool CheckForInteractable()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _interactableDetectionRange, _mask))
        {
            if (_interactable == hit.transform)
                return true;
            _interactable = hit.transform;
            return true;
        }
        else if (_interactable != null)
        {
            _interactable = null;
        }

        return false;
    }


    private void CheckForInteractableRange()
    {
        if ((_interactable.position - transform.position).magnitude <= _interactRange)
        {
            if (!_isCloseEnough)
                _isCloseEnough = true;
        }
        else if (_isCloseEnough)
            _isCloseEnough = false;
    }

    private void Interact()
    {
        if (_interactable != null && _isCloseEnough)
        {
            _interactable.GetComponent<IInteractable>().Interact();
        }
    }

    #region Enable Disable Inputs
    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
    #endregion
}
