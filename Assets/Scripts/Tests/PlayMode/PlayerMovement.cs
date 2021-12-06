using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerMovement
{
    private bool sceneLoaded;
    private Scene testingHall;

    [UnityTest]
    public IEnumerator Inicializar()
    {
        SceneManager.LoadSceneAsync("TestingHall", LoadSceneMode.Single);
        SceneManager.sceneLoaded += (s, m) =>
        {
            testingHall = s;
            sceneLoaded = true;
        };
        
        yield return new WaitUntil(()=> sceneLoaded);
        
        // Test : Esta presente en la escena el GameManager (?)
        var gameManager = GameObject.Find("GameManager");
        Assert.NotNull(gameManager);
        
        // Para que el juego inicie 'limpio' xd
        GameManager.Instance.IsTestingMode = true;
        GameManager.Instance.isGamePaused = false;
        GameManager.Instance.isMainMenuOn = false;

        SpawnearJugador();

        // Test : Spawnio bien el jugador (?)
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);
    }
    
    [UnityTest]
    public IEnumerator PruebaMovimientoHorizontal()
    {
        yield return new WaitForSeconds(2);
        
        if (!sceneLoaded)
            yield return Inicializar();
        
        // Test : Spawnio bien el jugador o no (?)
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);
        
        Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name} | Player Null ? {playerController == null}");

        // Test : Contiene el objeto 'Player' el script RigidBody2D (?)
        var rBody = playerController.transform.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rBody);
        
        while (rBody.velocity.x > 0 || rBody.velocity.x < 0)
        {
            playerController.playerMovementCtrl.Move(
                1, 
                0, 
                false, 
                false, 
                false, 
                false);
            Debug.Log($"Player Velocity X => {rBody.velocity.x}");
        }

        yield return new WaitForSeconds(10);
    }

    private void SpawnearJugador()
    {
        var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Player.prefab");
        GameObject.Instantiate(player, Vector3.zero, Quaternion.identity);
    }
}
