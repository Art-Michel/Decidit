using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [Header("References")]
    [SerializeField] Canvas _canvas;
    CanvasGroup _canvasGroup;
    Transform _playerCamera;
    Vector3 _camPos;
    Vector3 _camForward;
    float _distance;

    [Header("Values")]
    [SerializeField][Range(0.05f, 0.6f)] float _uiScale = 0.1f;
    [SerializeField][Range(0.0001f, 10f)] float _lookStrictness = 0.99f;
    [SerializeField][Range(0.5f, 10f)] float _appearSpeed = 9f;
    [SerializeField][Range(0.5f, 10f)] float _disappearSpeed = 1f;
    float _appearT;
    bool _isVisible;

    void Awake()
    {
        _playerCamera = Camera.main.transform;
        _canvasGroup = _canvas.GetComponent<CanvasGroup>();
    }

    protected override void Start()
    {
        base.Start();
        _appearT = 0f;
        _canvasGroup.alpha = 0f;
    }

    protected override void Update()
    {
        base.Update();
        _camPos = _playerCamera.transform.position;
        _camForward = _playerCamera.transform.forward;
        LookAtCamera();
        AdjustScale();
        AdjustVisibility();
    }

    void LookAtCamera()
    {
        //self explanatory
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
        _isVisible = Vector3.Dot(_camForward, (transform.position - _camPos).normalized) > minimumDot; //Display Healthbar if enemy is looked at
        _isVisible = _isVisible || _hasProbation; //Display Healthbar if enemy is taking damage

        //progressively display healthbar
        if (_isVisible && _appearT < 1)
        {
            _appearT = Mathf.Clamp01(_appearT + Time.deltaTime * _appearSpeed);
            _canvasGroup.alpha = Mathf.Lerp(0, 1, _appearT);
        }

        //progressively undisplay healthbar
        if (!_isVisible && _appearT > 0)
        {
            _appearT = Mathf.Clamp01(_appearT - Time.deltaTime * _disappearSpeed);
            _canvasGroup.alpha = Mathf.Lerp(0, 1, _appearT);
        }
    }
}
