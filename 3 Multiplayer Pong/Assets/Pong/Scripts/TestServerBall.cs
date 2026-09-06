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
    public int randomNum;
    Vector3 normalizedDirection = new(0, 0, 0);
    public bool IsServerOwned => IsSpawned && OwnerClientId == NetworkManager.ServerClientId;

    bool _loggedFirstMove;

    public override void OnNetworkSpawn()
    {
        name = "Server Ball";
        ApplyBallColor();

        Debug.Log(
            $"[ServerBall] Spawned | ownerClientId={OwnerClientId} | " +
            $"isServerOwned={IsServerOwned} | isServer={IsServer} | isClient={IsClient}");

        /*
        Vector3 force = new Vector3(0f, 0f, 0f);
        randomNum = Random.Range(0, 9);
        if (randomNum >= 5)
        {
            Debug.Log($" Ball Turn to Right" + randomNum);
            force = new Vector3(Random.Range(2f, 4f), 0f, Random.Range(2f, 4f));
        }
        if (randomNum <= 4)
        {
            Debug.Log($" Ball Turn to Left" + randomNum);
            force = new Vector3(Random.Range(-4f, -2f), 0f, Random.Range(-4f, -2f));
        }
        */
    }
    void Start()
    {
        randomNum = Random.Range(0, 9);
    }
    public override void OnNetworkDespawn()
    {
        Debug.Log($"[ServerBall] Despawned | ownerClientId={OwnerClientId}");
    }

    void Update()
    {
        if (!IsServer) return;
        //Vector3 normalizedDirection = new(0,0,0);
        //modified snippet from old ballscript
        if (randomNum >= 5) 
        {
            Debug.Log($" Ball Turn to Right" + randomNum);
            normalizedDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.right;
        }
        if (randomNum <= 4)
        {
            Debug.Log($" Ball Turn to Left" + randomNum);
            normalizedDirection = direction.sqrMagnitude > 0f ? -direction.normalized : Vector3.left;
        }
        //Vector3 normalizedDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.right;
        transform.Translate(normalizedDirection * (speed * Time.deltaTime), Space.World);

        if (!_loggedFirstMove)
        {
            _loggedFirstMove = true;
            Debug.Log("[ServerBall] Server started authoritative ball movement.");
        }

        if (Mathf.Abs(transform.position.x) <= resetDistance) return;
        Vector3 resetPosition = transform.position;
        resetPosition.x = -Mathf.Sign(resetPosition.x) * 0;
        transform.position = resetPosition;
        Debug.Log($"[ServerBall] Server wrapped ball to x={resetPosition.x:0.00}.");
        randomNum = Random.Range(0, 9);
    }


    void OnCollisionEnter(Collision collision)
    {
        paddler = collision.gameObject.GetComponent<Paddle>(); //get collision and check paddleside on who hits. will update more later
        if (!IsServer) return;
        if (paddler.side == PaddleSide.Left)
        {
            Debug.Log("Ball hit by: " + OwnerClientId + " Aka left");
            randomNum = 6; //make ball go right thanks to randonnum in update
        }
        if (paddler.side == PaddleSide.Right)
        {
            Debug.Log("Ball hit by: " + OwnerClientId + " Aka right");
            randomNum = 1; //make ball go left thanks to randonnum in update
        }
    }

    void ApplyBallColor()
    {
        if (!TryGetComponent(out Renderer ballRenderer)) return;

        ballRenderer.material.color = new Color(1f, 0.82f, 0.25f);
    }
}
