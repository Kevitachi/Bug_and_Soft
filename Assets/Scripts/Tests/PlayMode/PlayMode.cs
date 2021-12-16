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
    private bool cargado;

    [UnityTest]
    public IEnumerator Spawn()
    {
        SceneManager.LoadSceneAsync("TestingHall", LoadSceneMode.Single);
        SceneManager.sceneLoaded += (s, m) =>
        {
            cargado = true;
        };
        
        yield return new WaitUntil(()=> cargado);

        GameManager.Instance.IsTestingMode = true;
        GameManager.Instance.isGamePaused = false;
        GameManager.Instance.isMainMenuOn = false;

        SpawnearJugador();
        
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        
        Assert.NotNull(player);

        yield return null;
    }
    
    [UnityTest]
    public IEnumerator PruebaMovimientoHorizontal()
    {
        yield return new WaitForSeconds(2);
        
        if (!cargado)
            yield return Spawn();
        
        GameManager.Instance.SetInputEnabled(true);
        
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);
        
        var rBody = playerController.transform.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rBody);

        playerController.playerMovementCtrl.Move(10, 0, false, false, false, false);
        
        yield return new WaitForEndOfFrame();
        
        Assert.Greater(rBody.velocity.x, 0);

        yield return new WaitForSeconds(1);
        rBody.velocity = Vector2.zero;

        yield return null;
    }
    
    [UnityTest]
    public IEnumerator PruebaSalto()
    {
        yield return new WaitForSeconds(2);
        
        if (!cargado)
            yield return Spawn();
        
        GameManager.Instance.SetInputEnabled(true);
        
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);
        
        var rBody = playerController.transform.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rBody);

        playerController.playerMovementCtrl.Move(0, 0, false, true, false, false);
        yield return new WaitForSeconds(0.15f);
        Assert.Greater(rBody.velocity.y, 0);
        rBody.velocity = Vector2.zero;
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator PruebaDanio()
    {
        yield return new WaitForSeconds(2);
        
        if (!cargado)
            yield return Spawn();
        
        GameManager.Instance.SetInputEnabled(true);
        
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);

        int currentHealth = playerController.currentHealth;
        playerController.Damage(new DamageInfo(20, 0));

        Assert.AreEqual(currentHealth - 20, playerController.currentHealth);
        
        playerController.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return null;
    }
    
    [UnityTest]
    public IEnumerator PruebaAnimacion()
    {
        yield return new WaitForSeconds(2);
        
        if (!cargado)
            yield return Spawn();
        
        GameManager.Instance.SetInputEnabled(true);
        
        var playerController = GameObject.FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController);

        playerController.combatCtrl.AttackPerformed();

        yield return new WaitForEndOfFrame();
        
        Assert.IsTrue(playerController.characterAnimator.GetBool("Attacking"));
        
        playerController.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return null;
    }

    private void SpawnearJugador()
    {
        var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Player.prefab");
        GameObject.Instantiate(player, Vector3.zero, Quaternion.identity);
    }
}
