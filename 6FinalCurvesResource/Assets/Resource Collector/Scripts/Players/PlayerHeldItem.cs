using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/*
 * PlayerHeldItem tracks which resource or tool a player is carrying and shows
 * the matching held model on every client.
 *
 * This is the core NetworkVariable pattern for the lesson: server changes the
 * value, every client subscribes in OnNetworkSpawn, and each client applies the
 * current value immediately for late-join correctness.
 */

public class PlayerHeldItem : NetworkBehaviour
{
    [Serializable]
    public struct ItemCatalogEntry
    {
        public ObjectType type;
        public GameObject model;
        public NetworkObject prefab;
    }

    [Header("Item Catalog")]
    [SerializeField] List<ItemCatalogEntry> _itemCatalog = new();

    public ObjectType ObjectType => _heldObjectType.Value;

    readonly NetworkVariable<ObjectType> _heldObjectType = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // TODO Slice 6.9: subscribe to held-item changes and apply the current value.
        _heldObjectType.OnValueChanged += HandleObjectTypeChanged;
        HandleObjectTypeChanged(ObjectType.None, _heldObjectType.Value);

    }

    public override void OnNetworkPreDespawn()
    {
        base.OnNetworkPreDespawn();

        // Every Peer runs this callback, but only the server may create the drop. ...
        if (!IsServer) return;
        if (!NetworkManager.ShutdownInProgress)
            DropHeldItem(transform.position);
    }

    public override void OnNetworkDespawn()
    {
        // TODO Slice 6.10: unsubscribe from held-item changes. </> end of Slice 6
        base.OnNetworkDespawn();

        /*
        if (NetworkObject.InScenePlaced && !NetworkManager.ShutdownInProgress)
            gameObject.SetActive(false);
        */
        _heldObjectType.OnValueChanged -= HandleObjectTypeChanged;
    }

    public void SetHeldItem(ObjectType objectType)
    {
        if (!IsServer) return;

        // TODO Slice 6.6: store the authoritative held item.
        // No extra Game-view check yet. The value replicates, but the held model
        // stays hidden until 6.8 subscribes.
        _heldObjectType.Value = objectType;
    }

    public void Clear()
    {
        if (!IsServer) return;

        _heldObjectType.Value = ObjectType.None;
    }

    // Spawns the held item back into the world at the player's feet, then
    // empties the hand. Used for swap and for drop-on-disconnect.
    public void DropHeldItem(Vector3 position) // DropHeldItem
    {
        if (!IsServer) return;

        // TODO Slice 7.1:
        // 1. If the hand is empty, return.
        // 2. Find the matching catalog prefab and spawn it with NetworkObject.InstantiateAndSpawn.
        // 3. Clear().
        // Next: Slice 7.2 in ItemPickup.Interact.

        if (_heldObjectType.Value == ObjectType.None) return;
        ItemCatalogEntry matchingEntry = _itemCatalog.Find((item) => item.type == _heldObjectType.Value);
        NetworkObject.InstantiateAndSpawn(matchingEntry.prefab.gameObject, 
            NetworkManager, position: position, rotation: Quaternion.identity);

        Clear();
    }

    void HandleObjectTypeChanged(ObjectType previousValue, ObjectType newValue)
    {
        //TODO Slice 6.7: show only the held model matching newValue
        //No Play Mode check until 6.8 wires this to the NetworkVariable
        foreach (var item in _itemCatalog)
            item.model.SetActive(item.type == newValue);
    }
}
