using System.Collections;
using System.Collections.Generic;
using Controllers.Damage;
using NUnit.Framework;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayMode
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
        
        GameManager.Instance.SetInputEnabled(true);
        
        // Test : Spawnio bien el jugador o no (?)
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);
        
        // Test : Contiene el objeto 'Player' el script RigidBody2D (?)
        var rBody = playerController.transform.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rBody);

        //yield return MoverJugador(playerController);
        playerController.playerMovementCtrl.Move(10, 0, false, false, false, false);
        
        yield return new WaitForEndOfFrame();
        
        Assert.Greater(rBody.velocity.x, 0);

        yield return new WaitForSeconds(1);
        rBody.velocity = new Vector2(0, 0);

        yield return null;
    }
    
    [UnityTest]
    public IEnumerator PruebaSalto()
    {
        yield return new WaitForSeconds(2);
        
        if (!sceneLoaded)
            yield return Inicializar();
        
        GameManager.Instance.SetInputEnabled(true);
        
        // Test : Spawnio bien el jugador o no (?)
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);
        
        // Test : Contiene el objeto 'Player' el script RigidBody2D (?)
        var rBody = playerController.transform.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rBody);

        playerController.playerMovementCtrl.Move(0, 0, false, true, false, false);
        yield return new WaitForSeconds(0.15f);
        Assert.Greater(rBody.velocity.y, 0);
        playerController.playerMovementCtrl.Move(0, 0, false, true, false, false);
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator PruebaDanio()
    {
        yield return new WaitForSeconds(2);
        
        if (!sceneLoaded)
            yield return Inicializar();
        
        GameManager.Instance.SetInputEnabled(true);
        
        // Test : Spawnio bien el jugador o no (?)
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);

        int currentHealth = playerController.currentHealth;
        playerController.Damage(new DamageInfo(20, 0));

        Assert.AreEqual(currentHealth - 20, playerController.currentHealth);

        yield return null;
    }

    private void SpawnearJugador()
    {
        var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Player.prefab");
        GameObject.Instantiate(player, Vector3.zero, Quaternion.identity);
    }
}
