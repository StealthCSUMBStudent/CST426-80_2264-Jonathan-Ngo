using Unity.Netcode;
using UnityEngine;

/*
 * TestServerBall is Demo 07's server-owned non-player object. Only the server
 * simulates its motion and wrapping; NetworkTransform carries that transform
 * to clients, which never own or move the ball themselves.
 * 
 * 
 * I will be modifying this script instead. Much easier for me
 */

public class TestServerBall : NetworkBehaviour
{
    public float speed = 3f;
    public Vector3 direction = new(1f, 0f, 0f);
    public float resetDistance = 0f;
    [SerializeField] private Paddle paddler;
    public bool IsServerOwned => IsSpawned && OwnerClientId == NetworkManager.ServerClientId;

    bool _loggedFirstMove;

    public override void OnNetworkSpawn()
    {
        name = "Server Ball";
        ApplyBallColor();

        Debug.Log(
            $"[ServerBall] Spawned | ownerClientId={OwnerClientId} | " +
            $"isServerOwned={IsServerOwned} | isServer={IsServer} | isClient={IsClient}");
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log($"[ServerBall] Despawned | ownerClientId={OwnerClientId}");
    }

    void Update()
    {
        if (!IsServer) return;

        Vector3 normalizedDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.right;
        transform.Translate(normalizedDirection * (speed * Time.deltaTime), Space.World);

        if (!_loggedFirstMove)
        {
            _loggedFirstMove = true;
            Debug.Log("[ServerBall] Server started authoritative ball movement.");
        }

        if (Mathf.Abs(transform.position.x) <= resetDistance) return;

        Vector3 resetPosition = transform.position;
        resetPosition.x = -Mathf.Sign(resetPosition.x) * resetDistance;
        transform.position = resetPosition;
        Debug.Log($"[ServerBall] Server wrapped ball to x={resetPosition.x:0.00}.");
    }

    void OnCollisionEnter(Collision collision)
    {
        paddler = collision.gameObject.GetComponent<Paddle>(); //get collision and check paddleside on who hits. will update more later
        if (!IsServer) return;
        if (paddler.side == PaddleSide.Left)
        {
            Debug.Log("Ball hit by: " + OwnerClientId + " Aka left");
        }
        if (paddler.side == PaddleSide.Right)
        {
            Debug.Log("Ball hit by: " + OwnerClientId + " Aka right");
        }
    }

    void ApplyBallColor()
    {
        if (!TryGetComponent(out Renderer ballRenderer)) return;

        ballRenderer.material.color = new Color(1f, 0.82f, 0.25f);
    }
}
