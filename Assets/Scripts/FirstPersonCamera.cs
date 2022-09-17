using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    #region references
    PlayerInputMap _inputs;
    Rigidbody _rigidbody;
    [SerializeField] Transform _head;
    #endregion

    #region camera rotation
    private float targetYRotation;
    private float targetXRotation;

    [Range(0.001f, 2)][SerializeField] float _mouseSensitivityX;
    [Range(0.001f, 2)][SerializeField] float _mouseSensitivityY;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _mouseXInverted;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _mouseYInverted;

    [Range(0.1f, 100)][SerializeField] float _stickSensitivityX;
    [Range(0.1f, 100)][SerializeField] float _stickSensitivityY;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _controllerCameraXInverted;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _controllerCameraYInverted;

    [SerializeField] float _cameraSmoothness;
    #endregion

    #region Jumping and Falling
    private bool _isGrounded;

    private float _verticalVelocity;
    [Range(0, 10)][SerializeField] private float _drag;
    private const float _gravity = 9.81f;

    private bool _canJump;
    private bool _isJumping;
    private float _jumpCooldown;
    private float _coyoteTime;
    private const float _coyoteMaxTime = 0.15f;
    #endregion

    [Range(0, 100)][SerializeField] float _movementSpeed;

    void Awake()
    {
        _inputs = new PlayerInputMap();
        _rigidbody = GetComponent<Rigidbody>();
        _inputs.FirstPersonCamera.Jump.started += _ => Jump();
    }

    void Start()
    {
        transform.rotation = Quaternion.identity;

        Land();

        #region curseurs, à mettre dans le gamestatemanager
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        #endregion
    }

    private void Update()
    {
        #region Cooldowns
        if (_coyoteTime > 0f)
            _coyoteTime -= Time.deltaTime;
        if (!_canJump)
        {
            _jumpCooldown -= Time.deltaTime;
            if (_jumpCooldown <= 0) _canJump = true;
        }


        #endregion

        MoveCameraWithRightStick();
        MoveCameraWithMouse();

        CheckGround();
        Move();
        if (!_isGrounded) ApplyGravity();
    }

    private void MoveCameraWithMouse()
    {
        Vector2 mouseMovement = _inputs.FirstPersonCamera.Rotate.ReadValue<Vector2>()/* * Time.deltaTime */;
        targetYRotation = Mathf.Repeat(targetYRotation, 360);
        targetYRotation += mouseMovement.x * _mouseSensitivityX * _mouseXInverted;
        targetXRotation -= mouseMovement.y * _mouseSensitivityY * _mouseYInverted;

        targetXRotation = Mathf.Clamp(targetXRotation, -90, 90);

        var targetRotation = Quaternion.Euler(Vector3.up * targetYRotation) * Quaternion.Euler(Vector3.right * targetXRotation);

        _head.rotation = Quaternion.Lerp(_head.rotation, targetRotation, _cameraSmoothness * Time.deltaTime);
    }

    private void MoveCameraWithRightStick()
    {
        float RStickMovementX = _inputs.FirstPersonCamera.RotateX.ReadValue<float>() * Time.deltaTime;
        float RStickMovementY = _inputs.FirstPersonCamera.RotateY.ReadValue<float>() * Time.deltaTime;
        targetYRotation = Mathf.Repeat(targetYRotation, 360);
        targetYRotation += RStickMovementX * _stickSensitivityX * 10 * _controllerCameraXInverted;
        targetXRotation -= RStickMovementY * _stickSensitivityY * 10 * _controllerCameraYInverted;

        targetXRotation = Mathf.Clamp(targetXRotation, -90, 90);

        var targetRotation = Quaternion.Euler(Vector3.up * targetYRotation) * Quaternion.Euler(Vector3.right * targetXRotation);

        _head.rotation = Quaternion.Lerp(_head.rotation, targetRotation, _cameraSmoothness * Time.deltaTime);
    }

    private void Move()
    {
        Vector2 inputDirection = _inputs.FirstPersonCamera.Move.ReadValue<Vector2>();

        Vector3 forward = _head.forward;
        Vector3 right = _head.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 rightRelative = inputDirection.x * right;
        Vector3 forwardRelative = inputDirection.y * forward;

        Vector3 cameraRelativeMovement = (forwardRelative + rightRelative) * Time.deltaTime * _movementSpeed;

        //if (!CollisionCheck(cameraRelativeMovement.normalized, cameraRelativeMovement.magnitude))
        transform.position += cameraRelativeMovement;
    }

    /*private bool CollisionCheck(Vector3 direction, float distance)
    {
        return Physics.SphereCast(transform.position, 0.5f, direction, out RaycastHit hitInfo, distance);
    }*/

    private void CheckGround()
    {
        if (Physics.SphereCast(transform.position, 0.2f, -transform.up, out RaycastHit hitInfo, 1.3f))
        {
            if (!_isGrounded && _canJump)
                Land();

            //* Snap on top of grounds slightly above player
            if ((transform.position.y - 1.1f) - hitInfo.point.y < 0.4f)
                transform.position += new Vector3(0, (0.4f - (transform.position.y - 1.1f - hitInfo.point.y)), 0);
        }
        else
        {
            if (_isGrounded)
                TakeOff();
        }
    }

    private void Land()
    {
        _isGrounded = true;
        _verticalVelocity = 0f;
        _isJumping = false;
        _coyoteTime = -1f;
    }

    private void TakeOff()
    {
        _isGrounded = false;
        _coyoteTime = _coyoteMaxTime;
    }

    private void Jump()
    {
        if (_isGrounded || _coyoteTime > 0f && !_isJumping)
        {
            _isGrounded = false;
            _verticalVelocity = 8f;
            _isJumping = true;
            _canJump = false;
            _jumpCooldown = 0.1f;
        }
    }

    private void ApplyGravity()
    {
        transform.position += transform.up * _verticalVelocity * Time.deltaTime;
        _verticalVelocity -= _drag * _gravity * Time.deltaTime;
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