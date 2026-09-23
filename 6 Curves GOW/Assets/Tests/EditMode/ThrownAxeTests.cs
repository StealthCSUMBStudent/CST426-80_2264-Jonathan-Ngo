using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class ThrownAxeTests
{
    GameObject _player;
    ThrownAxe _axe;
    Transform _hand;
    CharacterController _thrower;
    Vector3 _heldPosition;
    Quaternion _heldRotation;

    [SetUp]
    public void SetUp()
    {
        _player = new GameObject("Thrower");
        _thrower = _player.AddComponent<CharacterController>();
        _hand = new GameObject("Hand").transform;
        _hand.SetParent(_player.transform);
        _hand.SetLocalPositionAndRotation(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 30f, 0f));

        GameObject axePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Curves/Prefabs/Axe.prefab");
        _axe = Object.Instantiate(axePrefab).GetComponent<ThrownAxe>();
        _axe.rigidbody.isKinematic = true;
        _axe.axeCollider.enabled = false;
        _axe.transform.SetParent(_hand);
        _heldPosition = new Vector3(0.2f, -0.3f, 0.4f);
        _heldRotation = Quaternion.Euler(20f, 40f, 60f);
        _axe.transform.SetLocalPositionAndRotation(_heldPosition, _heldRotation);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_axe.gameObject);
        Object.DestroyImmediate(_player);
    }

    [Test]
    public void CatchPosition_TracksMovingHandAndHeldOffsetAfterLaunch()
    {
        Vector3 originalPosition = _axe.transform.position;
        _axe.Launch(Vector3.forward, 25f, _thrower);
        Assert.That(Vector3.Distance(_axe.CatchPosition, originalPosition), Is.LessThan(0.0001f));

        _player.transform.SetPositionAndRotation(new Vector3(10f, 0f, -5f), Quaternion.Euler(0f, 90f, 0f));
        _hand.localRotation = Quaternion.Euler(40f, -20f, 15f);

        Assert.That(Vector3.Distance(_axe.CatchPosition, _hand.TransformPoint(_heldPosition)), Is.LessThan(0.0001f));
        Assert.That(Vector3.Distance(_axe.CatchPosition, _hand.position), Is.GreaterThan(0.1f));
        Assert.That(_axe.transform.parent, Is.Null);
    }

    [Test]
    public void Launch_DetachesAndEnablesPhysicsWhileIgnoringThrower()
    {
        _axe.Launch(Vector3.forward, 25f, _thrower);
        Assert.That(_axe.rigidbody.isKinematic, Is.False);
        Assert.That(_axe.axeCollider.enabled, Is.True);
        Assert.That(Physics.GetIgnoreCollision(_axe.axeCollider, _thrower), Is.True);
        Assert.That(_axe.transform.parent, Is.Null);
    }

    [Test]
    public void AttachToHand_RestoresPoseAndSupportsAnotherThrow()
    {
        for (int cycle = 0; cycle < 3; cycle++)
        {
            _axe.Launch(Vector3.forward, 25f, _thrower);
            _axe.transform.position += new Vector3(3f, 5f, 10f);
            _axe.transform.rotation = Quaternion.Euler(90f, 0f, 90f);
            _hand.position += Vector3.right;
            _hand.Rotate(0f, 20f, 0f);
            Vector3 catchPosition = _axe.CatchPosition;

            _axe.AttachToHand();

            Assert.That(_axe.transform.parent, Is.SameAs(_hand));
            Assert.That(Vector3.Distance(_axe.transform.localPosition, _heldPosition), Is.LessThan(0.0001f));
            Assert.That(Vector3.Distance(_axe.transform.position, catchPosition), Is.LessThan(0.0001f));
            Assert.That(Quaternion.Angle(_axe.transform.localRotation, _heldRotation), Is.LessThan(0.001f));
            Assert.That(_axe.rigidbody.isKinematic, Is.True);
            Assert.That(_axe.axeCollider.enabled, Is.False);
        }
    }
}
