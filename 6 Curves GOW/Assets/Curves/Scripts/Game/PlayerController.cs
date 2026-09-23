using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * PlayerController reads movement and axe input and tracks the axe through
 * Held, Throwing, Away, and Returning. It owns the aim and return line;
 * AnimationEvents triggers release and ThrownAxe handles the detached axe.
 */

public class PlayerController : MonoBehaviour
{
    static readonly int Speed = Animator.StringToHash("Speed");
    static readonly int ThrowHash = Animator.StringToHash("Throw");

    [Header("References")]
    public CharacterController characterController;
    public Animator animator;
    public ThrownAxe axe;

    [Header("Movement")]
    public float movementSpeed = 4f;
    public float rotationSpeed = 360f;

    [Header("Axe")]
    public float throwImpulse = 25f;
    public float returnDuration = 1f;
    public float bowAmount = 0.5f;

    enum AxeState { Held, Throwing, Away, Returning }

    Vector2 _smoothedInput;
    AxeState _axeState = AxeState.Held;
    LineRenderer _lineRenderer;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        UpdateMovement();
        UpdateAxeInput();
        UpdateAimVisual();
    }

    void UpdateMovement()
    {
        Vector2 input = ReadMovementInput();
        _smoothedInput = Vector2.MoveTowards(_smoothedInput, input, Time.deltaTime * 10f);

        transform.Rotate(Vector3.up, _smoothedInput.x * Time.deltaTime * rotationSpeed);

        Vector3 direction = characterController.transform.forward;
        characterController.Move(direction * _smoothedInput.y * movementSpeed * Time.deltaTime);
        animator.SetFloat(Speed, characterController.velocity.magnitude);
    }

    static Vector2 ReadMovementInput()
    {
        float x = (Keyboard.current.dKey.isPressed ? 1f : 0f)
            - (Keyboard.current.aKey.isPressed ? 1f : 0f);
        float y = (Keyboard.current.wKey.isPressed ? 1f : 0f)
            - (Keyboard.current.sKey.isPressed ? 1f : 0f);

        return new Vector2(x, y);
    }

    void UpdateAxeInput()
    {
        if (_axeState == AxeState.Held && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _axeState = AxeState.Throwing;
            animator.SetTrigger(ThrowHash);
        }

        if (_axeState == AxeState.Away && Mouse.current.rightButton.wasPressedThisFrame)
            StartCoroutine(ReturnAxe());
    }

    public void LaunchAxe()
    {
        if (_axeState != AxeState.Throwing) return;

        Vector3 direction = transform.forward;
        direction.y = 0f;
        direction.Normalize();
        axe.Launch(direction, throwImpulse, characterController);
        _axeState = AxeState.Away;
    }

    IEnumerator ReturnAxe()
    {
        _axeState = AxeState.Returning;
        axe.rigidbody.isKinematic = true;
        axe.axeCollider.enabled = false;
        // TODO Slice 8.3 (recall hook): start visual spin for the return.
        // Next: the Slice 8.3 catch hook in ThrownAxe.AttachToHand.

        Vector3 start = axe.transform.position;
        // TODO Slice 5.3: advance recall progress from 0 to 1 over returnDuration,
        // one step per frame, replacing the one-frame wait below.
        // The catch runs only after progress reaches 1.
        // Check: a stuck or mid-flight axe waits returnDuration, then snaps to the hand.
        // Next: Slice 5.4 below.

        // TODO Slice 5.4: each frame, place the axe on the GetReturnControlPoints curve
        // at the recall progress. The basic curve can stay fixed for the whole recall.
        // Check: standing still, recall follows the preview's bow at two bowAmount values.
        // Two full throw-and-recall cycles work, and so does recall mid-flight.
        // Next: optional Slice 5.5 below, or open Demo, Slice 6.1 in
        // Bezier/QuadraticBezierMath.cs. </> end of Slice 5

        // TODO Slice 5.5 (optional): keep the start fixed; let the handle and end
        // follow the moving hand.
        // Check: turn during recall. The axe still lands in the animated grip.
        // Next: open Demo, Slice 6.1 in Bezier/QuadraticBezierMath.cs.
        yield return null;

        axe.AttachToHand();
        _axeState = AxeState.Held;
    }

    (Vector3 p0, Vector3 p1, Vector3 p2) GetReturnControlPoints(Vector3 start)
    {
        Vector3 end = axe.CatchPosition;
        // TODO Slice 5.1: bow the return curve sideways to the axe-to-hand direction.
        // bowAmount controls how far.
        // Next: Slice 5.2 in DrawReturnPath, where you can see the bow.
        Vector3 middle = (start + end) * 0.5f;
        return (start, middle, end);
    }

    void UpdateAimVisual()
    {
        switch (_axeState)
        {
            case AxeState.Held:
                DrawAimLine();
                break;
            case AxeState.Away:
                DrawReturnPath();
                break;
            default:
                _lineRenderer.positionCount = 0;
                break;
        }
    }

    void DrawAimLine()
    {
        if (Physics.Raycast(axe.transform.position, transform.forward, out RaycastHit hit))
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, axe.transform.position);
            _lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            _lineRenderer.positionCount = 0;
        }
    }

    void DrawReturnPath()
    {
        // TODO Slice 5.2: draw the curve from GetReturnControlPoints with ten samples,
        // evenly spaced in t, from the axe to the hand. Recall (5.4) reuses the same curve.
        // Check: throw. While Away, the preview bows from the axe to the hand.
        // Changing bowAmount changes the bow.
        // Next: Slice 5.3 in ReturnAxe.
        _lineRenderer.positionCount = 0;
    }
}
