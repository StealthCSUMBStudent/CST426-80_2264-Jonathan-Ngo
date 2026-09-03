using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class ClientPaddle : NetworkBehaviour
{
    [SerializeField] private Paddle p_playerInstance;

    private void Awake()
    {
        p_playerInstance.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            p_playerInstance.enabled = true;
        }
        if (OwnerClientId != 0)
        {
            p_playerInstance.side = PaddleSide.Right;
            transform.position = new Vector3(7.5f, transform.position.y, transform.position.z);
        }
    }
}
