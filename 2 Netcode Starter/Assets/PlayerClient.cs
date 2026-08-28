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
            _thirdPersonController.enabled = true;
        }
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
