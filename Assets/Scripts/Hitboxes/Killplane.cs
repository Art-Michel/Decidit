using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killplane : MonoBehaviour
{
    [SerializeField] Transform _spawnPoint;
    private const float _delay = 0.2f;
    private const float _respawnUnfade = 0.1f;
    Queue<GameObject> _entitiesToRespawn;

    void Awake()
    {
        _entitiesToRespawn = new Queue<GameObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health health))
        {
            if (other.CompareTag("Player"))
                MenuManager.Instance.StartFading(_delay);

            _entitiesToRespawn.Enqueue(other.gameObject);
            Invoke("RespawnEntity", _delay);
        }
    }

    void RespawnEntity()
    {
        GameObject entity = _entitiesToRespawn.Dequeue();

        if (entity.CompareTag("Player"))
        {
            MenuManager.Instance.StartUnfading(_respawnUnfade);
            Player.Instance.KillMomentum();

            Player.Instance.CharaCon.detectCollisions = false;
            Player.Instance.CharaCon.enabled = false;
            Player.Instance.transform.position = _spawnPoint.position;
            Player.Instance.CharaCon.detectCollisions = true;
            Player.Instance.CharaCon.enabled = true;
        }

        else
            entity.transform.position = _spawnPoint.position;
    }
}
