using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class EditMode
{
    private bool cargado;

    // A Test behaves as an ordinary method
    [Test]
    public void EditModeSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Cargar()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main/TestingHall.unity");

        // Test : Esta presente en la escena el GameManager (?)
        var gameManager = GameObject.Find("GameManager");
        Assert.NotNull(gameManager);
        
        GameManager.Instance.IsTestingMode = true;
        GameManager.Instance.isGamePaused = false;
        GameManager.Instance.isMainMenuOn = false;

        SpawnearJugador();
        
        // Test : Spawnio bien el jugador (?)
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);
        
        yield return null;
    }
    
    private void SpawnearJugador()
    {
        var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Player.prefab");
        GameObject.Instantiate(player, Vector3.zero, Quaternion.identity);
    }
}
