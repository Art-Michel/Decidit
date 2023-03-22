using UnityEngine;

// Makes a transform oscillate relative to its start position
public class ProjectileOscillator : MonoBehaviour
{
    [SerializeField] float Amplitude = .3f;
    [SerializeField] float _frequency = 10f;
    Vector3 _actualDirection;
    float _t = 0f;
    Vector3 _lastFrameOffset;

    public void Setup(Vector3 direction, bool centered)
    {
        _actualDirection = direction;
        //_actualDirection = transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z;
        if (centered)
            transform.localPosition = _actualDirection * (Amplitude / 2);

        _lastFrameOffset = Vector3.zero;
        _t = 0.0f;
    }

    void Update()
    {
        _t += Time.deltaTime * _frequency;
        transform.position -= _lastFrameOffset;
        Vector3 offset = _actualDirection * (Mathf.Sin(Mathf.PI * _t)) * Amplitude;
        transform.position += offset;
        _lastFrameOffset = offset;
    }
}
