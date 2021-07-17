using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBoundsComponent : MonoBehaviour
{
    [SerializeField] private AudioSource deathSFX;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.KillPlayer();
            deathSFX.Play();
        }
    }
}
