using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    PlayerInputMap _inputs;
    [SerializeField] Transform _head;
    [SerializeField] float _mouseSensitivityX;
    [SerializeField] float _mouseSensitivityY;
    [SerializeField] int _mouseXInverted; ///1 is No, -1 is Yes
    [SerializeField] int _mouseYInverted; ///1 is No, -1 is Yes

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
    void Update()
    {
        Vector2 mouseMovement = _inputs.FirstPersonCamera.Rotate.ReadValue<Vector2>() * Time.deltaTime;
        float cameraMovementX = mouseMovement.x * _mouseSensitivityY * _mouseYInverted;
        float cameraMovementY = -mouseMovement.y * _mouseSensitivityX * _mouseXInverted;

        transform.Rotate(0f, cameraMovementX, 0f);
        _head.Rotate(cameraMovementY, 0f, 0f);
        _head.localEulerAngles = new Vector3(Mathf.Clamp(_head.rotation.x, -90, 90), _head.rotation.y, _head.rotation.z);
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
