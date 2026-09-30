using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/*
 * Receptacle collects one resource type, like a pallet accepting wood. A
 * replicated stack count drives which stacked visuals are shown, so current
 * and late-joining clients render the same pile.
 */

public class Receptacle : Interactable
{
    [SerializeField] ObjectType _acceptedObjectType;
    [SerializeField] List<GameObject> _stackedResourceVisuals = new();
    [SerializeField] AudioClip _audioClip;
    AudioSource _audioSource;
    readonly NetworkVariable<int> _stackedCount = new();

    public bool IsFilled => _stackedCount.Value >= _stackedResourceVisuals.Count;

    public override void OnNetworkSpawn()
    {
        
        base.OnNetworkSpawn();
        _stackedCount.OnValueChanged += HandleStackedCountChanged;
        HandleStackedCountChanged(_stackedCount.Value, _stackedCount.Value);
        // TODO Slice 9.4: subscribe to count changes and apply the current count.
        // Check: both windows show the pile. A late joiner sees it without a deposit sound.
        // Next: Slice 9.5 OnNetworkDespawn.

    }

    public override void OnNetworkDespawn()
    {
        // TODO Slice 9.5: unsubscribe from replicated count changes.
        // </> end of Slice 9
        _stackedCount.OnValueChanged -= HandleStackedCountChanged;
        HandleStackedCountChanged(_stackedCount.Value, _stackedCount.Value);
        base.OnNetworkDespawn();
    }

    public override bool CanInteract(ObjectType heldType)
    {
        // TODO Slice 9.1:
        // 1. Accept only the configured resource.
        // 2. Only while space remains.
        // Check: hold wood. The pallet highlights. Hold an axe, it does not.
        if (heldType == _acceptedObjectType)
        {
            if (IsFilled == true) return false;
            if (IsFilled == false) return true;
        }
        // E still does nothing until 9.2. Count already starts at 0.
        // Next: Slice 9.2 Interact.
        return false;
    }

    protected override void Interact(PlayerHeldItem heldItem)
    {
        // TODO Slice 9.2:
        // 1. Add one resource.
        _stackedCount.Value++;
        // 2. Clear the player's hand.
        heldItem.Clear();
        // Check: deposit wood. The hand empties. Stack visuals stay off until 9.4.
        // Next: Slice 9.3 HandleStackedCountChanged.
    }

    void HandleStackedCountChanged(int previousValue, int newValue)
    {
        // TODO Slice 9.3:
        // 1. Make the number of shown visuals match the count.
        _audioSource = GetComponent<AudioSource>();
        //_audioSource.PlayOneShot(_audioClip);
        for (int i = 0; i < _stackedResourceVisuals.Count; i++)
        {
            if (i < newValue)
            {
                _stackedResourceVisuals[i].SetActive(true);
            } else
            {
                _stackedResourceVisuals[i].SetActive(false);
            }
            // 2. Play audio only when the stack grows.
            if (newValue > previousValue)
            {
                _audioSource.PlayOneShot(_audioClip);
            }
        }
        

        // Next: Slice 9.4 in OnNetworkSpawn — subscribe and apply.
    }

}
