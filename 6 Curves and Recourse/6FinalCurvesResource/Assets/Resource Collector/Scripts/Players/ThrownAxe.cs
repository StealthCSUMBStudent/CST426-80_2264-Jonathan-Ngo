using UnityEngine;

/*
 * ThrownAxe keeps the axe's held pose so it can be attached to the hand
 * after a throw. Launch physics and collision response belong here;
 * PlayerController decides when to throw and recall it.
 */

public class ThrownAxe : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Collider axeCollider;
    public float spinSpeed = 1500f;
    public TrailRenderer axeTrail;
    bool _stuck;
    [SerializeField] Transform _hand;
    Vector3 _heldLocalPosition;
    Quaternion _heldLocalRotation;
    //[SerializeField] AudioClip flight;a
    [SerializeField] AudioClip attach;
    AudioSource _audioSource;
    ParticleSystem _particleSystem;
    public Vector3 CatchPosition => _hand.TransformPoint(_heldLocalPosition);
    void Start()
    {
        axeTrail = GetComponent<TrailRenderer>();
        axeTrail.enabled = false;
        _audioSource = GetComponent<AudioSource>();
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Stop();
    }
    public void Launch(Vector3 direction, float impulse, CharacterController thrower, Transform hand)
    {
        _hand = hand;
        _heldLocalPosition = transform.localPosition;
        _heldLocalRotation = transform.localRotation;

        Physics.IgnoreCollision(axeCollider, thrower);
        transform.SetParent(null);
        //transform.position += direction * 0.5f;
        transform.right = direction;

        rigidbody.isKinematic = false;
        axeCollider.enabled = true;

        rigidbody.AddForce(direction * impulse, ForceMode.VelocityChange);
        rigidbody.AddTorque(transform.forward * (-spinSpeed * Mathf.Deg2Rad), ForceMode.VelocityChange);
        axeTrail.enabled = true;
        
    }

    public void AttachToHand()
    {
        //transform.SetParent(_hand);
        axeTrail.enabled = true;
        transform.SetLocalPositionAndRotation(_heldLocalPosition, _heldLocalRotation);
        rigidbody.isKinematic = true;
        axeCollider.enabled = false;
        axeTrail.enabled = false;
        _audioSource.PlayOneShot(attach);
        _particleSystem.Stop();
    }

    void OnCollisionEnter(Collision collision)
    {
        rigidbody.isKinematic = true;
        axeTrail.Clear();
        axeTrail.enabled = false;
        _audioSource.PlayOneShot(attach);
        _particleSystem.Play();
    }
}
