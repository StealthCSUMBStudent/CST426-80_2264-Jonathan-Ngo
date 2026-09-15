using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

/*
 * PlayerController is the owner's local input loop: movement, target
 * selection, and the client-to-server interaction request. The server
 * still owns every world mutation.
 */

public class PlayerController : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] CharacterController _characterController;
    [SerializeField] Animator _animator;
    [SerializeField] PlayerHeldItem _heldItem;

    [Header("Detection")]
    [SerializeField] float _detectionRadius = 3f;
    [SerializeField] float _detectionAngle = 60f;
    [SerializeField] LayerMask _pickupLayer;

    [Header("Movement")]
    [SerializeField] float _movementSpeed = 4f;
    [SerializeField] float _rotationSpeed = 200f;

    Interactable _closestTarget;
    Vector2 _smoothedInput;

    void Update()
    {
        if (!IsOwner) return;

        // TODO Slice 2.2: read this owner's movement in Update.
        Vector3 input = ReadMovementInput();

        // TODO Slice 2.5: smooth _smoothedInput toward the raw input so the walk cycle does not pop.
        _smoothedInput = Vector2.MoveTowards(_smoothedInput,input,Time.deltaTime * 10f);
        
        // TODO Slice 2.3: rotate and move forward/back.
        float rotation = _smoothedInput.x * _rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);

        Vector3 direction = transform.forward; // forward vs vector3.forward. Former is what we need.
        _characterController.Move(direction * _smoothedInput.y * _movementSpeed * Time.deltaTime);

        // TODO Slice 2.4: set the "Speed" animator float so walk speed matches input.
        _animator.SetFloat("Speed", _characterController.velocity.magnitude);

        UpdateInteractionTarget();

        // TODO Slice 6.1:
        // 1. Detect E or left-click this frame.
        // 2. Call HandleInteractionPressed.
        // Check: Play Mode, Host, highlight the axe, press E.
        // The Interact clip plays. The axe still stays on the ground.
        if (Keyboard.current.eKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) 
            HandleInteractionPressed();
        

        
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        // TODO Slice 2.6: make the main camera follow only its local player. </> end of Slice 2
        FindAnyObjectByType<FollowCamera>().Target = transform;

    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            // TODO Slice 5.2: turn off the current target's Highlightable,
            // then clear _closestTarget.
            ClearSelection();
        }

        base.OnNetworkDespawn();
    }

    void HandleInteractionPressed()
    {
        if (!IsOwner) return;

        // TODO Slice 6.2:

        // 1. If there is no target, return.

        // 2. Fire the Animator's "Interact" trigger.

        // 3. Send the target's NetworkObjectId to the server.
        if (_closestTarget == null) return;
        _animator.SetTrigger("Interact");
        RequestInteractRpc(_closestTarget.NetworkObjectId);

    }


    static Vector2 ReadMovementInput()
    {
        // TODO Slice 2.1: return WASD input as a two-dimensional vector.
        Vector2 input = Vector2.zero;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        return input;
        //return new Vector2();
    }


    void UpdateInteractionTarget()
    {
        // PROVIDED Slice 5.1: find the closest valid Interactable in front of the player.
        // When the target changes, clear the old highlight and select the new one.
        Interactable interactable = null;
        interactable = FindClosestValidInteractable();
        if (interactable == _closestTarget) return;
        ClearSelection();
        if (interactable == null) return;

        _closestTarget = interactable;
        _closestTarget.GetComponent<Highlightable>().SetHighlighted(true);

    }


    Interactable FindClosestValidInteractable()
    {
        Collider[] candidates = Physics.OverlapSphere(transform.position, _detectionRadius, _pickupLayer);
        Interactable closestInteractable = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (Collider c in candidates) {
            //Has Interactable
            if(!c.TryGetComponent(out Interactable interactable)) continue;
            

            if (!interactable.CanInteract(_heldItem.ObjectType)) continue;

            Vector3 directionToInteractable = interactable.transform.position - transform.position;

            float angle =  Vector3.Angle(transform.forward, directionToInteractable.normalized);
            if (angle > _detectionAngle) continue;

            float distanceSqr = directionToInteractable.sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestInteractable = interactable;
                closestDistanceSqr = distanceSqr;
            }
        }

        return closestInteractable;
    }
    
    void ClearSelection()
    {
        if (_closestTarget != null)
        {
            _closestTarget.GetComponent<Highlightable>().enabled = false;
        }
        _closestTarget = null;
    }
    
    [Rpc(SendTo.Server)]
    void RequestInteractRpc(ulong networkObjectId)
    {
        //Debug.Log($"Requesting Interact on server for {networkObjectId}");
        // TODO Slice 6.3:

        // 1. Look up networkObjectId in SpawnedObjects.

        // 2. If that object is gone, return. It may have despawned after you selected it.

        // 3. If it has an Interactable, call ServerInteract(_heldItem).


        // Check: E still only plays Interact. Console stays clean. The pickup

        // (e.g. axe) does not move yet.


        // Next: Slice 6.4 in World/Interactable.cs — ServerInteract.
        Dictionary<ulong, NetworkObject> spawnedObjectMap = NetworkManager.SpawnManager.SpawnedObjects;
        if (!spawnedObjectMap.TryGetValue(networkObjectId, out NetworkObject spawnedObject))
        {
            Debug.LogError($"Couldn't Find id {networkObjectId}");
            return;
        }
        
        if (!spawnedObject.TryGetComponent(out Interactable interactable))
        {
            Debug.LogError($"Object doesn't have interactable");
            return;
        }

        if (interactable.CanInteract(_heldItem.ObjectType)) 
            interactable.ServerInteract(_heldItem);


    }
}
