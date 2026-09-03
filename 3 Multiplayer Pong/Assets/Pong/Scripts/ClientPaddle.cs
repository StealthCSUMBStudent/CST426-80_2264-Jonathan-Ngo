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
            p_playerInstance.side = PaddleSide.Left;
            p_playerInstance.enabled = true;
        }


        if (OwnerClientId != 0)
        {
            p_playerInstance.side = PaddleSide.Right;
            Debug.Log("Player ClientID is " + OwnerClientId + "side is " + p_playerInstance.side);
            transform.position = new Vector3(7.5f, transform.position.y, transform.position.z);
        }
    }
}
