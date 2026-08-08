using System;
using Unity.VisualScripting;
using UnityEngine;

public class Destructible : MonoBehaviour,IDamageable
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private ScoreManager scoreManager;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private float minImpactForce;

    [SerializeField] private SpriteRenderer spriteColour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float score;
    [SerializeField] private float destroyedScore;
    void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        spriteColour = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;  //Run a check with the RB
        if (rb==null)
        {
            Debug.Log("No Rigidbody2D found on collison");
            return;
        }
        float impactForce = collision.relativeVelocity.magnitude; //Convert Collision force to a float value
        //Debug.Log("Impact Force: " + impactForce);  
        if (impactForce < minImpactForce)
        {
            return;  //Minumum force for damage to occur
        }
        float damage = impactForce * damageMultiplier;
      
        TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        //Debug.Log($"{gameObject.name} took {damage} damage.");

        currentHealth -= damage;
        //Debug.Log($"{gameObject.name} Has: {currentHealth} Health left");
        float scoreGained = damage + score;
        UpdateDamageVisuals();
        if (scoreManager != null)
        {
            scoreManager.UpdateScore(scoreGained);
        }
        if (currentHealth <= 0)
        {
            scoreManager.UpdateScore(destroyedScore);
            Destroy(gameObject);
        }

    }
    void UpdateDamageVisuals() //Doing this because they want visual feedback if we do sprites for the blocks update here
    {
        if (spriteColour != null)
        { 
        //    Debug.Log("Updating Damage Visuals");

            float healthPercent = currentHealth / maxHealth;
        Color color =spriteColour.color;
        if (healthPercent <= 0.3f) //30% health or less, make the sprite semi-transparent   
        {
            color.a = 0.3f; // Make the sprite semi-transparent
        }
        else if (healthPercent <= 0.6f) //60% left
        {
            color.a = 0.6f; // Make the sprite partially transparent
        }
        else
        {
            color = Color.white;
        }
    }

        }
      
}
