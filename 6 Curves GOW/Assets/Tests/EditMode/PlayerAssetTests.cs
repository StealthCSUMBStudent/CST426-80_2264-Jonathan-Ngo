using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerAssetTests
{
    [Test]
    public void PlayerPrefab_HasCompleteAxeWiring()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Curves/Prefabs/Player.prefab");
        PlayerController player = prefab.GetComponent<PlayerController>();
        AssertPlayerWiring(player);
    }

    [Test]
    public void MainGame_HasWiredPlayerAndOneRenderer()
    {
        var scene = EditorSceneManager.OpenPreviewScene("Assets/Curves/Scenes/Main Game.unity");
        try
        {
            PlayerController player = null;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                PlayerController candidate = root.GetComponentInChildren<PlayerController>(true);
                if (candidate != null) player = candidate;
            }

            Assert.That(player, Is.Not.Null);
            AssertPlayerWiring(player);
        }
        finally
        {
            EditorSceneManager.ClosePreviewScene(scene);
        }
    }

    static void AssertPlayerWiring(PlayerController player)
    {
        Assert.That(player, Is.Not.Null);
        Assert.That(player.characterController, Is.Not.Null);
        Assert.That(player.animator, Is.Not.Null);
        Assert.That(player.axe, Is.Not.Null);
        Assert.That(player.axe.rigidbody, Is.Not.Null);
        Assert.That(player.axe.axeCollider, Is.Not.Null);
        Assert.That(player.GetComponentInChildren<AnimationEvents>(true).playerController, Is.SameAs(player));
        LineRenderer[] lines = player.GetComponentsInChildren<LineRenderer>(true);
        Assert.That(lines, Has.Length.EqualTo(1));
        Assert.That(lines[0].gameObject, Is.EqualTo(player.gameObject));
        Assert.That(lines[0].useWorldSpace, Is.True);

        foreach (Transform child in player.GetComponentsInChildren<Transform>(true))
            Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject), Is.Zero, child.name);
    }
}
