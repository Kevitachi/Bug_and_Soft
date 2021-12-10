using System.Collections;
using System.Collections.Generic;
using CameraManagement;
using Dialogues;
using Inventory;
using Managers;
using Misc;
using NUnit.Framework;
using Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class EditMode
{
    [Test]
    public void PruebaObjetosNivel1()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main/Level1.unity");
        
        Assert.AreEqual("Level1", EditorSceneManager.GetActiveScene().name);
        
        // Buscamos objetos que esten en el nivel
        // Si no estan, esta prueba falla
        UIManager ui = GameObject.FindObjectOfType<UIManager>();
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        DialogueManager dm = GameObject.FindObjectOfType<DialogueManager>();
        InventorySlotManager im = GameObject.FindObjectOfType<InventorySlotManager>();
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        DynamicCamera dynamicCamera = GameObject.FindObjectOfType<DynamicCamera>();
        
        // Test : Esta presente el UIManager ?
        Assert.NotNull(ui);        
        // Test : Esta presente el GameManager ?
        Assert.NotNull(gm);        
        // Test : Esta presente el DialogueManager ?
        Assert.NotNull(dm);        
        // Test : Esta presente el InventorySlotManager ?
        Assert.NotNull(im);        
        // Test : Esta presente el PlayerController ?
        Assert.NotNull(player);        
        // Test : Esta presente la camara ?
        Assert.NotNull(dynamicCamera);
    }
    
    [Test]
    public void PruebaObjetosNivel2()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main/Level2.unity");
        
        Assert.AreEqual("Level2", EditorSceneManager.GetActiveScene().name);
        
        // Buscamos objetos que esten en el nivel
        // Si no estan, esta prueba falla
        UIManager ui = GameObject.FindObjectOfType<UIManager>();
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        DialogueManager dm = GameObject.FindObjectOfType<DialogueManager>();
        InventorySlotManager im = GameObject.FindObjectOfType<InventorySlotManager>();
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        DynamicCamera dynamicCamera = GameObject.FindObjectOfType<DynamicCamera>();
        
        // Test : Esta presente el UIManager ?
        Assert.NotNull(ui);        
        // Test : Esta presente el GameManager ?
        Assert.NotNull(gm);        
        // Test : Esta presente el DialogueManager ?
        Assert.NotNull(dm);        
        // Test : Esta presente el InventorySlotManager ?
        Assert.NotNull(im);        
        // Test : Esta presente el PlayerController ?
        Assert.NotNull(player);        
        // Test : Esta presente la camara ?
        Assert.NotNull(dynamicCamera);
    }
    
    [Test]
    public void PruebaObjetosNivel21()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main/Level2.1.unity");
        
        Assert.AreEqual("Level2.1", EditorSceneManager.GetActiveScene().name);
        
        // Buscamos objetos que esten en el nivel
        // Si no estan, esta prueba falla
        UIManager ui = GameObject.FindObjectOfType<UIManager>();
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        DialogueManager dm = GameObject.FindObjectOfType<DialogueManager>();
        InventorySlotManager im = GameObject.FindObjectOfType<InventorySlotManager>();
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        DynamicCamera dynamicCamera = GameObject.FindObjectOfType<DynamicCamera>();
        
        // Test : Esta presente el UIManager ?
        Assert.NotNull(ui);        
        // Test : Esta presente el GameManager ?
        Assert.NotNull(gm);        
        // Test : Esta presente el DialogueManager ?
        Assert.NotNull(dm);        
        // Test : Esta presente el InventorySlotManager ?
        Assert.NotNull(im);        
        // Test : Esta presente el PlayerController ?
        Assert.NotNull(player);        
        // Test : Esta presente la camara ?
        Assert.NotNull(dynamicCamera);
    }    
    
    [Test]
    public void PruebaObjetosNivel3()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main/Level2.1.unity");
        
        Assert.AreEqual("Level2.1", EditorSceneManager.GetActiveScene().name);
        
        // Buscamos objetos que esten en el nivel
        // Si no estan, esta prueba falla
        UIManager ui = GameObject.FindObjectOfType<UIManager>();
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        DialogueManager dm = GameObject.FindObjectOfType<DialogueManager>();
        InventorySlotManager im = GameObject.FindObjectOfType<InventorySlotManager>();
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        DynamicCamera dynamicCamera = GameObject.FindObjectOfType<DynamicCamera>();
        
        // Test : Esta presente el UIManager ?
        Assert.NotNull(ui);        
        // Test : Esta presente el GameManager ?
        Assert.NotNull(gm);        
        // Test : Esta presente el DialogueManager ?
        Assert.NotNull(dm);        
        // Test : Esta presente el InventorySlotManager ?
        Assert.NotNull(im);        
        // Test : Esta presente el PlayerController ?
        Assert.NotNull(player);        
        // Test : Esta presente la camara ?
        Assert.NotNull(dynamicCamera);
    }
    
    [Test]
    public void PruebaUI()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main/Level1.unity");
        
        Assert.AreEqual("Level1", EditorSceneManager.GetActiveScene().name);

        GameObject UI = GameObject.Find("/UI");

        Canvas canvas = UI.GetComponentInChildren<Canvas>();
        
        // Test : UI tiene un canvas ?
        Assert.NotNull(canvas);
        
        // Test : UI tiene los 16 gameobjets por defecto ?
        Assert.True(UI.transform.childCount == 16);
    }
    
    [Test]
    public void PruebaComponentesEnPlayer()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main/Level1.unity");
        
        Assert.AreEqual("Level1", EditorSceneManager.GetActiveScene().name);

        GameObject player = GameObject.Find("/Player");

        PlayerController playerController = player.GetComponent<PlayerController>();
        PlayerMovementController playerMovementController = player.GetComponent<PlayerMovementController>();
        PlayerCombatController playerCombatController = player.GetComponent<PlayerCombatController>();
        PlayerLadderClimbingController playerLadderClimbingController = player.GetComponent<PlayerLadderClimbingController>();
        EffectController effectController = player.GetComponent<EffectController>();
        Animator animator = player.GetComponent<Animator>();
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        Rigidbody2D rbody = player.GetComponent<Rigidbody2D>();
        
        // Test : player contiene todos los componenentes ?
        Assert.NotNull(playerController);
        Assert.NotNull(playerMovementController);
        Assert.NotNull(playerCombatController);
        Assert.NotNull(playerLadderClimbingController);
        Assert.NotNull(effectController);
        Assert.NotNull(animator);
        Assert.NotNull(spriteRenderer);
        Assert.NotNull(rbody);
    }
}
