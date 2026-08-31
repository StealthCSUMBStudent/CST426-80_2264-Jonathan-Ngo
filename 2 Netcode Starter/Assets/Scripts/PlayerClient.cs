using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClient : NetworkBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] StarterAssetsInputs _starterAssetsInputs;
    [SerializeField] ThirdPersonController _thirdPersonController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerInput.enabled = false;
        _starterAssetsInputs.enabled = false;
        _thirdPersonController.enabled = false;

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            _playerInput.enabled = true;
            _starterAssetsInputs.enabled = true;
            //_thirdPersonController.enabled = true;
        }
        if (IsServer)
        {
            _thirdPersonController.enabled = true;
        }
    }
    [Rpc(SendTo.Server)]
    private void UpdatedInputServerRpc(Vector2 move, Vector2 look, bool jump, bool sprint)
    {
        _starterAssetsInputs.MoveInput(move);
        _starterAssetsInputs.LookInput(look);
        _starterAssetsInputs.JumpInput(jump);
        _starterAssetsInputs.SprintInput(sprint);
    }
    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        UpdatedInputServerRpc(_starterAssetsInputs.move,_starterAssetsInputs.look, _starterAssetsInputs.jump, _starterAssetsInputs.sprint);
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
