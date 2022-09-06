using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    PlayerInputMap _inputs;
    [SerializeField] Transform _head;

    private float targetYRotation;
    private float targetXRotation;

    [Range(0.1f, 100)][SerializeField] float _mouseSensitivityX;
    [Range(0.1f, 100)][SerializeField] float _mouseSensitivityY;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _mouseXInverted;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _mouseYInverted;

    [Range(0.1f, 100)][SerializeField] float _stickSensitivityX;
    [Range(0.1f, 100)][SerializeField] float _stickSensitivityY;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _controllerCameraXInverted;
    [Range(-1, 1)][Tooltip("1 is Normal, -1 is Inverted")][SerializeField] int _controllerCameraYInverted;

    [SerializeField] float _cameraSmoothness;

    void Awake()
    {
        _inputs = new PlayerInputMap();
    }

    void Start()
    {
        transform.rotation = Quaternion.identity;

        #region à mettre dans le gamestatemanager
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        #endregion
    }

    private void Update()
    {
        MoveCameraWithRightStick();
        MoveCameraWithMouse();
    }

    private void MoveCameraWithMouse()
    {
        Vector2 mouseMovement = _inputs.FirstPersonCamera.Rotate.ReadValue<Vector2>() * Time.deltaTime;
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