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

    [SerializeField] float _mouseSensitivityX;
    [SerializeField] float _mouseSensitivityY;
    [SerializeField] float _cameraSmoothness;
    [SerializeField] int _mouseXInverted; ///1 is Normal, -1 is Inverted
    [SerializeField] int _mouseYInverted; ///1 is Normal, -1 is Inverted

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
        Vector2 mouseMovement = _inputs.FirstPersonCamera.Rotate.ReadValue<Vector2>() * Time.deltaTime;
        targetYRotation = Mathf.Repeat(targetYRotation, 360);
        targetYRotation += mouseMovement.x * _mouseSensitivityX * _mouseXInverted;
        targetXRotation -= mouseMovement.y * _mouseSensitivityY * _mouseYInverted;

        targetXRotation = Mathf.Clamp(targetXRotation, -90, 90);

        var targetRotation = Quaternion.Euler(Vector3.up * targetYRotation) * Quaternion.Euler(Vector3.right * targetXRotation);

        _head.rotation = Quaternion.Lerp(_head.rotation, targetRotation, _cameraSmoothness * Time.deltaTime);
    }

    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
}