using UnityEngine;

// Makes a transform oscillate relative to its start position
public class ProjectileOscillator : MonoBehaviour
{
    [SerializeField] float _amplitude = 1.0f;
    [SerializeField] float _frequency = 10f;
    Vector3 _actualDirection;
    float _t = 0f;
    Vector3 _lastFrameOffset;

    public void Setup(Vector3 direction)
    {
        _t = 0f;
        if (true)
            _actualDirection = transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z;
        _lastFrameOffset = Vector3.zero;
    }

    void Update()
    {
        _t += Time.deltaTime * _frequency;
        transform.position -= _lastFrameOffset;
        Vector3 offset = _actualDirection * (Mathf.Sin(Mathf.PI * _t)) * _amplitude;
        transform.position += offset;
        _lastFrameOffset = offset;
    }
}
