using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    #region References
    [Header("References")]
    PlayerInputMap _inputs;
    CharacterController _charaCon;
    [SerializeField] Transform _head;
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

    private bool _isGrounded;
    private const float _gravity = 9.81f;

    RaycastHit _groundStoodOn;
    private const float _groundRaycastLength = 1.2f; // _charaCon.height / 2
    private bool _canJump;
    private bool _isJumping;
    private float _jumpCooldown;
    private float _coyoteTime;
    private const float _coyoteMaxTime = 0.15f;
    #endregion

    #region Movement Variables
    [Header("Movement Settings")]
    Vector3 _inputMovement;
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
        _inputMovement = Vector2.zero;
        _globalMomentum = Vector3.zero;

        #region curseurs, à mettre dans le gamestatemanager
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        #endregion
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

        UpdateInputDirection(_inputs.FirstPersonCamera.Move.ReadValue<Vector2>());
        UpdateGlobalMomentum();
        MoveCharacter(_inputMovement);
        MoveCharacter(_globalMomentum * Time.deltaTime);

        //Verticality
        CheckGround();
    }

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
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position - transform.up * _groundRaycastLength, Color.white, 0f, false);
    }
#endif

    private void CheckGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out _groundStoodOn, _groundRaycastLength))
        {
            if (!_isGrounded && _canJump)
                Land();
        }
        else
        {
            if (_isGrounded)
                TakeOff();
        }

        /*if (_isGrounded && ((transform.position - _groundStoodOn.point).magnitude < _groundRaycastLength -0.1f))
            _globalMomentum += Vector3.up * Time.deltaTime;*/
            //!Faire remonter le controller quand un peu dedans le sol
    }

    private void Land()
    {
        _isGrounded = true;
        _globalMomentum.y = 0f;
        _isJumping = false;
        _coyoteTime = -1f;
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
            _globalMomentum.y += _jumpStrength;
            _isJumping = true;
            _canJump = false;
            _jumpCooldown = 0.1f; //min time allowed between two jumps, to avoid mashing jump up slopes and so we dont check for a ground before the character actually jumps.
        }
    }
    #endregion

    #region Shmovement Functions

    private void UpdateInputDirection(Vector2 inputDirection)
    {
        Vector3 forward = _head.forward;
        Vector3 right = _head.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 rightRelative = inputDirection.x * right;
        Vector3 forwardRelative = inputDirection.y * forward;

        _inputMovement = (forwardRelative + rightRelative) * Time.deltaTime * _movementSpeed;
    }

    void UpdateGlobalMomentum()
    {
        if (!_isGrounded) _globalMomentum.y -= _drag * _gravity * Time.deltaTime;
    }

    private void MoveCharacter(Vector3 direction)
    {
        //Move along slopes
        if (_isGrounded)
        {
            if (Vector3.Dot(transform.up, _groundStoodOn.normal) > Mathf.InverseLerp(90, 0, _charaCon.slopeLimit)) //! Suboptimal, lerp in awake instead when we sure we aint gon be changing slopelimit no more
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
