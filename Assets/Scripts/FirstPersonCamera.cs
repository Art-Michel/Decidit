using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    #region References
    [Header("References")]
    PlayerInputMap _inputs;
    CharacterController _charaCon;
    [SerializeField] Transform _head;
    [SerializeField] TextMeshProUGUI _debugInputVelocityText;
    [SerializeField] TextMeshProUGUI _debugGlobalVelocityText;
    [SerializeField] TextMeshProUGUI _debugGravityText;
    [SerializeField] TextMeshProUGUI _debugGroundedText;
    #endregion

    #region Camera rotation variables
    private float _cameraTargetYRotation;
    private float _cameraTargetXRotation;

    [Header("Camera Settings")]
    //Mouse
    [Range(0.001f, 2)][SerializeField] float _mouseSensitivityX = 0.1f;
    [Range(0.001f, 2)][SerializeField] float _mouseSensitivityY = 0.1f;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted, please don't put anything in between.")][SerializeField] int _mouseXInverted = 1;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted, please don't put anything in between.")][SerializeField] int _mouseYInverted = 1;

    //Joysticks
    [Range(0.1f, 100)][SerializeField] float _stickSensitivityX = 15f;
    [Range(0.1f, 100)][SerializeField] float _stickSensitivityY = 15f;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted, please don't put anything in between.")][SerializeField] int _controllerCameraXInverted = 1;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted, please don't put anything in between.")][SerializeField] int _controllerCameraYInverted = 1;

    //General
    [SerializeField] float _cameraSmoothness = 100000;
    #endregion

    #region Jumping, Falling and Ground Detection variables
    [Header("Jumping Settings")]
    [Range(0, 10)][SerializeField] private float _drag = 1f;
    [Range(0, 50)][SerializeField] private float _jumpStrength = 8f;

    [Range(0, 50)][SerializeField] private float _slideForceOnSlopes = 350f;
    [Range(0, 100)][SerializeField] private float _momentumAirborneReductionSpeed = 8f;
    [Range(0, 100)][SerializeField] private float _momentumGroundedReductionSpeed = 50f;

    private bool _isGrounded;
    private const float _gravity = 9.81f;
    private float _currentlyAppliedGravity;

    RaycastHit _groundStoodOn;
    private const float _groundRaycastLength = .85f; // 1.8 - (_charaCon.height / 2) - 0.25
    private const float _groundSphereCastRadius = 0.25f;
    private bool _canJump;
    private bool _isJumping;
    private float _jumpCooldown;
    private float _coyoteTime;
    private const float _coyoteMaxTime = 0.15f;

    #endregion

    #region Movement Variables
    [Header("Movement Settings")]
    Vector3 _movementInputs;
    Vector3 _globalMomentum;
    [Range(0, 100)][SerializeField] float _movementSpeed = 9f;
    #endregion

    private void Awake()
    {
        _inputs = new PlayerInputMap();
        _charaCon = GetComponent<CharacterController>();
        _inputs.FirstPersonCamera.Jump.started += _ => Jump();
    }

    private void Start()
    {
        transform.rotation = Quaternion.identity;
        _movementInputs = Vector2.zero;
        _globalMomentum = Vector3.zero;
        _currentlyAppliedGravity = 0;
    }

    private void Update()
    {
        // Cooldowns
        if (_coyoteTime > 0f)
            _coyoteTime -= Time.deltaTime;
        if (!_canJump)
        {
            _jumpCooldown -= Time.deltaTime;
            if (_jumpCooldown <= 0) _canJump = true;
        }

        //Camera
        MoveCameraWithRightStick();
        MoveCameraWithMouse();

        CheckGround();

        //Update Movement Vectors
        _movementInputs = MakeDirectionCameraRelative(_inputs.FirstPersonCamera.Move.ReadValue<Vector2>());
        _movementInputs *= Time.deltaTime * _movementSpeed;
        UpdateGlobalMomentum();

        //ApplyGravity
        if (!_isGrounded) ApplyGravity();

        //Debug Stuff
        UpdateDebugTexts();

        //Apply Movement
        ApplyMovementsToCharacter((_globalMomentum * Time.deltaTime) + (_movementInputs) + (Vector3.up * _currentlyAppliedGravity * Time.deltaTime));

    }

    #region Debugs
    private void UpdateDebugTexts()
    {
#if UNITY_EDITOR
        if (_debugGroundedText)
        {
            _debugGroundedText.text = ("Is Grounded: " + _isGrounded);
        }

        if (_debugInputVelocityText)
        {
            _debugInputVelocityText.text =
            ("Input Velocity:\nx= " + (_movementInputs.x / Time.deltaTime).ToString("F2") +
            "\nz= " + (_movementInputs.z / Time.deltaTime).ToString("F2"));
        }
        if(_debugGlobalVelocityText)
        {
            _debugGlobalVelocityText.text =
            ("Momentum Velocity:\nx= " + (_globalMomentum.x / Time.deltaTime).ToString("F2") +
            "\ny= " + (_globalMomentum.y / Time.deltaTime).ToString("F2") +
            "\nz= " + (_globalMomentum.z / Time.deltaTime).ToString("F2"));
        }
        if(_debugGravityText)
        {
            _debugGravityText.text = ("Gravity:\n   " + _currentlyAppliedGravity);
        }
#endif
    }
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.blue;
        Debug.DrawLine(transform.position, transform.position - transform.up * _groundRaycastLength, Color.blue, 0f, false);
        Gizmos.DrawSphere(_groundStoodOn.point + transform.up * _groundSphereCastRadius, _groundSphereCastRadius);

#endif
    }
    #endregion

    #region Camera Functions
    private void MoveCameraWithMouse()
    {
        Vector2 mouseMovement = _inputs.FirstPersonCamera.Rotate.ReadValue<Vector2>()/* * Time.deltaTime */;
        _cameraTargetYRotation = Mathf.Repeat(_cameraTargetYRotation, 360);
        _cameraTargetYRotation += mouseMovement.x * _mouseSensitivityX * _mouseXInverted;
        _cameraTargetXRotation -= mouseMovement.y * _mouseSensitivityY * _mouseYInverted;

        _cameraTargetXRotation = Mathf.Clamp(_cameraTargetXRotation, -85, 85);

        var targetRotation = Quaternion.Euler(Vector3.up * _cameraTargetYRotation) * Quaternion.Euler(Vector3.right * _cameraTargetXRotation);

        _head.rotation = Quaternion.Lerp(_head.rotation, targetRotation, _cameraSmoothness * Time.deltaTime);
    }

    private void MoveCameraWithRightStick()
    {
        float RStickMovementX = _inputs.FirstPersonCamera.RotateX.ReadValue<float>() * Time.deltaTime;
        float RStickMovementY = _inputs.FirstPersonCamera.RotateY.ReadValue<float>() * Time.deltaTime;
        _cameraTargetYRotation = Mathf.Repeat(_cameraTargetYRotation, 360);
        _cameraTargetYRotation += RStickMovementX * _stickSensitivityX * 10 * _controllerCameraXInverted;
        _cameraTargetXRotation -= RStickMovementY * _stickSensitivityY * 10 * _controllerCameraYInverted;

        _cameraTargetXRotation = Mathf.Clamp(_cameraTargetXRotation, -85, 85f);

        var targetRotation = Quaternion.Euler(Vector3.up * _cameraTargetYRotation) * Quaternion.Euler(Vector3.right * _cameraTargetXRotation);

        _head.rotation = Quaternion.Lerp(_head.rotation, targetRotation, _cameraSmoothness * Time.deltaTime);
    }
    #endregion

    #region Ground Detection Functions
    private void CheckGround()
    {
        if (Physics.SphereCast(transform.position, _charaCon.radius + 0.01f, -transform.up, out _groundStoodOn, _groundRaycastLength))
        {
            if (!_isGrounded && _canJump)
                if (Vector3.Angle(transform.up, _groundStoodOn.normal) > _charaCon.slopeLimit)
                {
                    _globalMomentum += (Vector3.down + _groundStoodOn.normal).normalized * _slideForceOnSlopes * Time.deltaTime;
                }
                else
                    Land();
        }
        else
        {
            if (_isGrounded)
                TakeOff();
        }
    }

    private void Land()
    {
        _currentlyAppliedGravity = 0;
        _globalMomentum.y = 0;
        _isGrounded = true;
        _isJumping = false;
        _coyoteTime = -1f;
        if (_inputs.FirstPersonCamera.Jump.IsPressed()) Jump();
    }

    private void TakeOff()
    {
        _isGrounded = false;
        _coyoteTime = _coyoteMaxTime;
    }
    #endregion

    #region Jumping and Falling Functions
    private void Jump()
    {
        if (_isGrounded || _coyoteTime > 0f && !_isJumping)
        {
            _isGrounded = false;
            _currentlyAppliedGravity += _jumpStrength;
            _isJumping = true;
            _canJump = false;
            _jumpCooldown = 0.1f; //min time allowed between two jumps, to avoid mashing jump up slopes and so we dont check for a ground before the character actually jumps.
        }
    }

    private void ApplyGravity()
    {
        _currentlyAppliedGravity -= _gravity * _drag * Time.deltaTime;
    }
    #endregion

    #region Shmovement Functions

    private Vector3 MakeDirectionCameraRelative(Vector2 inputDirection)
    {
        Vector3 forward = _head.forward;
        Vector3 right = _head.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 rightRelative = inputDirection.x * right;
        Vector3 forwardRelative = inputDirection.y * forward;

        return (forwardRelative + rightRelative);
    }

    private void UpdateGlobalMomentum()
    {
        //Add Input Vector to Momentum
        _globalMomentum += _movementInputs;

        //Decrease Momentum Each Frame slower when airborne and faster when grounded
        float velocityDecreaseRate;
        if (_isGrounded)
            velocityDecreaseRate = _momentumGroundedReductionSpeed;
        else
            velocityDecreaseRate = _momentumAirborneReductionSpeed;

        //Store last frame's direction inside a variable
        var lastFrameXVelocitySign = Mathf.Sign(_globalMomentum.x);
        var lastFrameZVelocitySign = Mathf.Sign(_globalMomentum.z);

        _globalMomentum = _globalMomentum.normalized * (_globalMomentum.magnitude - velocityDecreaseRate * Time.deltaTime);

        //If last frame's direction was opposite, snap to 0
        if(Mathf.Sign(_globalMomentum.x) != lastFrameXVelocitySign) _globalMomentum.x = 0;
        if(Mathf.Sign(_globalMomentum.z) != lastFrameZVelocitySign) _globalMomentum.z = 0;
    }

    private void ApplyMovementsToCharacter(Vector3 direction)
    {
        //Move along slopes
        if (_isGrounded)
        {
            if (Vector3.Angle(transform.up, _groundStoodOn.normal) < _charaCon.slopeLimit)
                direction = (direction - (Vector3.Dot(direction, _groundStoodOn.normal)) * _groundStoodOn.normal);
        }

        //Actually move
        _charaCon.Move(direction);
    }

    #endregion

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
