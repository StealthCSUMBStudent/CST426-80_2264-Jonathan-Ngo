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

        Vector3 start = axe.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < returnDuration)
        {
            float t = elapsedTime / returnDuration;

            Vector3 p0 = start;
            Vector3 p2 = axe.CatchPosition;//hand
            Vector3 p1 = (p0 + p2) * 0.5f + transform.right * bowAmount;

            axe.transform.position = QuadraticBezierMath.SamplePointBernstein(p0, p1, p2, t);
            axe.transform.Rotate(Vector3.forward, axe.spinSpeed * Time.deltaTime, Space.Self);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        axe.AttachToHand();
        _axeState = AxeState.Held;
    }

    (Vector3 p0, Vector3 p1, Vector3 p2) GetReturnControlPoints(Vector3 start)
    {
        Vector3 end = axe.CatchPosition;
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

        _lineRenderer.positionCount = 0;
    }
}
