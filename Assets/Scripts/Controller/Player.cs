using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] TextMeshProUGUI _debugInputVelocityText;
    [SerializeField] TextMeshProUGUI _debugGlobalVelocityText;
    [SerializeField] TextMeshProUGUI _debugGravityText;
    [SerializeField] TextMeshProUGUI _debugGroundedText;
    [SerializeField] TextMeshProUGUI _debugSpeedText;
    [SerializeField] Transform _head;
    [SerializeField] CharacterController _charaCon;
    PlayerInputMap _inputs;
    #endregion

    #region Camera rotation variables
    private float _cameraTargetYRotation;
    private float _cameraTargetXRotation;

    [Header("Camera Settings")]
    //Mouse
    [Range(0.1f, 100)][SerializeField] private float _mouseSensitivityX = 10f;
    [Range(0.1f, 100)][SerializeField] private float _mouseSensitivityY = 10f;
    [SerializeField] private bool _mouseXInverted = false;
    [SerializeField] private bool _mouseYInverted = false;

    //Joysticks
    [Range(1f, 100)][SerializeField] private float _stickSensitivityX = 15f;
    [Range(1f, 100)][SerializeField] private float _stickSensitivityY = 15f;
    [SerializeField] private bool _controllerCameraXInverted = false;
    [SerializeField] private bool _controllerCameraYInverted = false;

    int _mouseXInvertedValue;
    int _mouseYInvertedValue;
    int _controllerCameraXInvertedValue;
    int _controllerCameraYInvertedValue;

    //General
    [SerializeField] float _cameraSmoothness = 100000;
    #endregion

    #region Jumping, Falling and Ground Detection variables
    [Header("Jumping Settings")]
    [Range(0, 50)][SerializeField] private float _jumpStrength = 10f;
    [Range(0, 10)][SerializeField] private float _drag = 2f;

    private bool _isGrounded;
    private const float _gravity = 9.81f;
    private float _currentlyAppliedGravity;
    private Vector3 _steepSlopesMovement;

    private RaycastHit _groundStoodOn;
    private const float _groundSpherecastLength = .65f; // _charaCon.height/2 - _charaCon.radius
    private const float _groundSpherecastRadius = .35f; // _charaCon.radius + 0.1f to mitigate skin width
    private bool _canJump;
    private bool _isJumping;
    private float _jumpCooldown;
    private float _coyoteTime;
    private const float _coyoteMaxTime = 0.3f;

    #endregion

    #region Movement Variables
    [Header("Movement Settings")]
    [Range(0, 20)][SerializeField] private float _movementSpeed = 9f;
    [Range(0, 1)][SerializeField] private float _movementAccelerationSpeed = 0.0666f; //Approx 4 frames
    [Range(0, 1)][SerializeField] private float _movementDecelerationSpeed = 0.0666f; //Approx 4 frames
    [Range(0, 100)][SerializeField] private float _slideForceOnSlopes = 40f;
    [Range(0, 100)][Tooltip("if lower than movement speed, you will accelerate when airborne")][SerializeField] private float _airborneFriction = 9f;
    [Range(0, 100)][SerializeField] private float _groundedFriction = 50f;
    //// ADD LATER [Range(0, 100)][SerializeField] private float _slidingFriction = 5f;

    private Vector3 _movementInputs; // X is Left-Right and Z is Backward-Forward
    private Vector3 _lastFrameMovementInputs;
    private Vector3 _globalMomentum;
    private float _movementAcceleration;
    private bool _isPressingADirection;

    private Vector3 _finalMovement;
    #endregion

    private void Awake()
    {
        _inputs = new PlayerInputMap();
        _inputs.FirstPersonCamera.Jump.started += _ => Jump();
    }

    private void Start()
    {
        transform.rotation = Quaternion.identity;
        _movementInputs = Vector2.zero;
        _globalMomentum = Vector3.zero;
        _currentlyAppliedGravity = 0;
        _movementAcceleration = 0;
        SetCameraInvert();
    }

    //!To Do list
    //// Movement Acceleration when inputting a direction
    //// Movement Deceleration when not inputting a direction anymore
    //TODO make ceilings work
    //? when airborne, Raycast towards inputDirection and if wall we be wallriding
    //? jump again when wallriding to walljump => add jumpStrength to gravity; reset momentum; and add wall's normal to momentum
    //// Remove spherecast and just use charactercontroller.isgrounded
    //nevermind, charactercontroller.isgrounded sucks ass
    //TODO // Stop being slower when going up and down slopes
    //TODO // Gain speed up steep slopes only when sliding
    //// Snap when slightly inside ground

    private void Update()
    {
        //Cooldowns
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

        //Ground Spherecast and Storage of its normal
        CheckGround();

        //Update Movement Vectors
        UpdateMovement();
        HandleMovementAcceleration();
        UpdateGlobalMomentum();

        //Gravity
        if (!_isGrounded) ApplyGravity();

        //Final Movement Formula //I got lost with the deltatime stuff but i swear it works perfectly
        _finalMovement = (_globalMomentum * Time.deltaTime) + (_movementInputs) + (Vector3.up * _currentlyAppliedGravity * Time.deltaTime) + (_steepSlopesMovement * Time.deltaTime);

        //Debug Values on screen
        UpdateDebugTexts();

        //Apply Movement
        ApplyMovementsToCharacter(_finalMovement);
    }

    #region Debugs
    private void UpdateDebugTexts()
    {
#if UNITY_EDITOR
        if (_debugGroundedText)
            _debugGroundedText.text = ("Is Grounded: " + _isGrounded);

        if (_debugInputVelocityText)
            _debugInputVelocityText.text =
            ("Input Velocity:\nx= " + (_movementInputs.x / Time.deltaTime).ToString("F1") +
            "\nz= " + (_movementInputs.z / Time.deltaTime).ToString("F1") +
            "\n\n Acceleration:\n  " + _movementAcceleration.ToString("F1"));

        if (_debugGlobalVelocityText)
            _debugGlobalVelocityText.text =
            ("Momentum Velocity:\nx= " + (_globalMomentum.x).ToString("F1") +
            "\ny= " + (_globalMomentum.y).ToString("F1") +
            "\nz= " + (_globalMomentum.z).ToString("F1"));

        if (_debugGravityText)
            _debugGravityText.text = ("Gravity:\n   " + _currentlyAppliedGravity.ToString("F1"));

        if (_debugSpeedText)
            _debugSpeedText.text = ("Total Speed:\n   " + (_finalMovement.magnitude / Time.deltaTime).ToString("F3"));
#endif
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        //ground cast
        Gizmos.color = Color.blue;
        Debug.DrawLine(transform.position, transform.position - transform.up * (_groundSpherecastLength), Color.blue, 0f, false);
        RaycastHit debugGroundcast;
        if (Physics.SphereCast(transform.position, _groundSpherecastRadius, -transform.up, out debugGroundcast, _groundSpherecastLength)) // should be the same as CheckGround()'s
            Gizmos.DrawSphere(new Vector3(transform.position.x, (debugGroundcast.point.y + _groundSpherecastRadius), transform.position.z), (_groundSpherecastRadius));

        //vector
        Debug.DrawLine(transform.position, transform.position + _finalMovement /Time.deltaTime * 0.5f, Color.red, 0f);
#endif
    }
    #endregion

    #region Camera Functions
    private void SetCameraInvert()
    {
        if (_mouseXInverted) _mouseXInvertedValue = -1;
        else _mouseXInvertedValue = 1;

        if (_mouseYInverted) _mouseYInvertedValue = -1;
        else _mouseYInvertedValue = 1;

        if (_controllerCameraXInverted) _controllerCameraXInvertedValue = -1;
        else _controllerCameraXInvertedValue = 1;

        if (_controllerCameraYInverted) _controllerCameraYInvertedValue = -1;
        else _controllerCameraYInvertedValue = 1;
    }

    private void MoveCameraWithMouse()
    {
        Vector2 mouseMovement = _inputs.FirstPersonCamera.Rotate.ReadValue<Vector2>()/* * Time.deltaTime */;
        _cameraTargetYRotation = Mathf.Repeat(_cameraTargetYRotation, 360);
        _cameraTargetYRotation += mouseMovement.x * _mouseSensitivityX * 0.01f * _mouseXInvertedValue;
        _cameraTargetXRotation -= mouseMovement.y * _mouseSensitivityY * 0.01f * _mouseYInvertedValue;

        _cameraTargetXRotation = Mathf.Clamp(_cameraTargetXRotation, -85, 85);

        var targetRotation = Quaternion.Euler(Vector3.up * _cameraTargetYRotation) * Quaternion.Euler(Vector3.right * _cameraTargetXRotation);

        _head.rotation = Quaternion.Lerp(_head.rotation, targetRotation, _cameraSmoothness * Time.deltaTime);
    }

    private void MoveCameraWithRightStick()
    {
        float RStickMovementX = _inputs.FirstPersonCamera.RotateX.ReadValue<float>() * Time.deltaTime;
        float RStickMovementY = _inputs.FirstPersonCamera.RotateY.ReadValue<float>() * Time.deltaTime;
        _cameraTargetYRotation = Mathf.Repeat(_cameraTargetYRotation, 360);
        _cameraTargetYRotation += RStickMovementX * _stickSensitivityX * 10 * _controllerCameraXInvertedValue;
        _cameraTargetXRotation -= RStickMovementY * _stickSensitivityY * 10 * _controllerCameraYInvertedValue;

        _cameraTargetXRotation = Mathf.Clamp(_cameraTargetXRotation, -85, 85f);

        var targetRotation = Quaternion.Euler(Vector3.up * _cameraTargetYRotation) * Quaternion.Euler(Vector3.right * _cameraTargetXRotation);

        _head.rotation = Quaternion.Lerp(_head.rotation, targetRotation, _cameraSmoothness * Time.deltaTime);
    }
    #endregion

    #region Ground Detection Functions
    private void CheckGround()
    {
        //if there is ground below
        if (Physics.SphereCast(transform.position, _groundSpherecastRadius, -transform.up, out _groundStoodOn, _groundSpherecastLength)) //! The cast is perfect and should not be touched
        {
            if (!_isGrounded && _canJump)
            {
                //and ground is flat enough: Land
                if (Vector3.Angle(transform.up, _groundStoodOn.normal) < _charaCon.slopeLimit)
                    Land();
                //if ground is too steep: Slide along
                else
                    _steepSlopesMovement = (Vector3.down + _groundStoodOn.normal).normalized * _slideForceOnSlopes * -_currentlyAppliedGravity;
            }
        }
        // if there is no ground below and we're grounded, then we are not anymore
        else if (_isGrounded)
            TakeOff();
    }

    private void Land()
    {
        //Reset many values when landing
        _currentlyAppliedGravity = 0;
        _steepSlopesMovement = Vector3.zero;
        _globalMomentum.y = 0;
        _isGrounded = true;
        _isJumping = false;
        _coyoteTime = -1f;

        //Jump immediately if player is pressing jump
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

    private void UpdateMovement()
    {
        //Register new movement input
        Vector3 _newMovementInputs = MakeDirectionCameraRelative(_inputs.FirstPersonCamera.Move.ReadValue<Vector2>());

        //only use new movement input if it is not null
        _isPressingADirection = _newMovementInputs.x != 0 || _newMovementInputs.z != 0;
        if (_isPressingADirection)
            _movementInputs = _newMovementInputs;
        //else, we use last frame's movement input again
        else
            _movementInputs = _lastFrameMovementInputs;

        //Store this frame's input
        _lastFrameMovementInputs = _movementInputs;

        _movementInputs *= Time.deltaTime * _movementSpeed;
    }

    private void HandleMovementAcceleration()
    {
        if (!_isPressingADirection)
            _movementAcceleration = Mathf.Clamp01(_movementAcceleration -= Time.deltaTime / _movementDecelerationSpeed);

        else
            _movementAcceleration = Mathf.Clamp01(_movementAcceleration += Time.deltaTime / _movementAccelerationSpeed);

        _movementInputs *= _movementAcceleration;
    }

    private void UpdateGlobalMomentum()
    {
        //Add Input Vector to Momentum
        _globalMomentum += _movementInputs;

        //Decrease Momentum Each Frame slower when airborne and faster when grounded
        float velocityDecreaseRate;
        if (_isGrounded)
            velocityDecreaseRate = _groundedFriction;
        else
            velocityDecreaseRate = _airborneFriction;

        //Store last frame's direction inside a variable
        var lastFrameXVelocitySign = Mathf.Sign(_globalMomentum.x);
        var lastFrameZVelocitySign = Mathf.Sign(_globalMomentum.z);

        _globalMomentum = _globalMomentum.normalized * (_globalMomentum.magnitude - velocityDecreaseRate * Time.deltaTime);

        //If last frame's direction was opposite, snap to 0
        if (Mathf.Sign(_globalMomentum.x) != lastFrameXVelocitySign) _globalMomentum.x = 0;
        if (Mathf.Sign(_globalMomentum.z) != lastFrameZVelocitySign) _globalMomentum.z = 0;
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
