using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

/*
 * ResourceNode is a harvestable object like a tree or stone. Replicated health
 * counts down as players hit it with the right tool; at zero the server spawns
 * resource pickups and every client hides the depleted node.
 */

public class ResourceNode : Interactable
{
    [SerializeField] List<ObjectType> _toolTypeRequired = new();
    [SerializeField] NetworkObject _producedPrefab;
    [SerializeField] int _amountToSpawn = 3;
    [SerializeField] int _startingHealth = 1;
    [SerializeField] AudioClip _audioClip;
    AudioSource _audioSource;
    readonly NetworkVariable<int> _health = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // TODO Slice 8.1: on the server, set health to _startingHealth.
        if (IsServer)
        {
            _health.Value = _startingHealth;
        }
        // Next: Slice 8.2 CanInteract.


        // TODO Slice 8.5: subscribe to health changes and apply the current health.

        // Check: both windows hide a depleted tree. A late joiner sees it hidden.

        // Next: Slice 8.6 OnNetworkDespawn.
    }

    public override void OnNetworkDespawn()
    {
        // TODO Slice 8.6: unsubscribe from replicated health changes.

        // </> end of Slice 8

        // Next: Slice 9.1 in World/Receptacle.cs.
        base.OnNetworkDespawn();
    }

    public override bool CanInteract(ObjectType heldType)
    {
        // TODO Slice 8.2:

        // 1. Require a living node.
        if (_health.Value <= 0)
        {
            return false;
        }
        // 2. Require an accepted tool.

        // Check: hold the axe. The tree highlights. Empty-handed, it does not.
        if (!_toolTypeRequired.Contains(heldType))
        {
            return false;
        }
        // Next: Slice 8.3 Interact and HitFeedbackRpc.
        return true;
    }

    protected override void Interact(PlayerHeldItem heldItem)
    {
        // TODO Slice 8.3:

        // 1. Reduce health.
        _health.Value--;
        // 2. Call HitFeedbackRpc.
        HitFeedbackRpc();
        // 3. Spawn _amountToSpawn copies of _producedPrefab with InstantiateAndSpawn.
        for (int counter = 0; counter < _amountToSpawn; counter++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            NetworkObject.InstantiateAndSpawn(_producedPrefab.gameObject,
            NetworkManager, position: spawnPosition, rotation: spawnRotation);
        }
        // 4. Place each with a small random XZ offset and random yaw.

        // Check: axe the tree. Wood appears. The mesh is still there until 8.4.

        // Next: Slice 8.4 HandleHealthChanged.
    }

    [Rpc(SendTo.ClientsAndHost)]
    void HitFeedbackRpc()
    {
        // TODO Slice 8.3: play the authored hit sound on each observer.
        _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(_audioClip);
    }

    void HandleHealthChanged(int previousValue, int newValue)
    {
        // TODO Slice 8.4: make the visuals and physics match the health.

        // Next: Slice 8.5 in OnNetworkSpawn — subscribe and apply.
    }

    void ApplyHealth()
    {
        // TODO Slice 8.2: hide depleted nodes and disable their collider.
    }
}
