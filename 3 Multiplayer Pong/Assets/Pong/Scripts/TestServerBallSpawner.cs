using Unity.Netcode;
using UnityEngine;

/*
 * TestServerBallSpawner creates Demo 07's ball at runtime. It is deliberately
 * a plain scene MonoBehaviour: once the local process is the listening server,
 * it instantiates one registered prefab and calls NetworkObject.Spawn().
 */

public class TestServerBallSpawner : MonoBehaviour
{
    [SerializeField] NetworkObject ballPrefab;
    [SerializeField] Vector3 spawnPosition = new(0f, 1.5f, 0f);

    public NetworkObject SpawnedBall => _spawnedBall;
    public bool HasSpawnedBall => _spawnedBall != null && _spawnedBall.IsSpawned;

    NetworkObject _spawnedBall;

    void Update()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || !networkManager.IsListening || !networkManager.IsServer) return;

        SpawnBall();
    }

    void OnDestroy()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || !networkManager.IsServer) return;

        DespawnBall();
    }

    void SpawnBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("[ServerBall] Cannot spawn ball because no ball prefab is assigned.");
            return;
        }

        if (HasSpawnedBall) return;

        _spawnedBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        _spawnedBall.Spawn();

        Debug.Log(
            $"[ServerBall] Server spawned ball | ownerClientId={_spawnedBall.OwnerClientId} | " +
            $"networkObjectId={_spawnedBall.NetworkObjectId}");
    }

    void DespawnBall()
    {
        if (_spawnedBall == null) return;

        if (_spawnedBall.IsSpawned)
        {
            Debug.Log($"[ServerBall] Server despawning ball | networkObjectId={_spawnedBall.NetworkObjectId}");
            _spawnedBall.Despawn();
        }

        _spawnedBall = null;
    }
}
