using System;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private ScoreManager scoreManager;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private float minImpactForce;

    [SerializeField] private SpriteRenderer spriteColour;
    [SerializeField] private float score;
    [SerializeField] private float destroyedScore;
    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private float breakFadeDuration = 0.25f;

    void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        if (spriteColour == null) spriteColour = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null)
        {
            return;
        }
        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce < minImpactForce)
        {
            return;
        }
        float damage = impactForce * damageMultiplier;
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        float scoreGained = damage + score;
        UpdateDamageVisuals();

        if (scoreManager != null)
        {
            scoreManager.UpdateScore(scoreGained);
        }

        if (damagePopupPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);
            DamagePopup popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            popup.ShowPopup(scoreGained);
        }

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    void UpdateDamageVisuals()
    {
        if (spriteColour != null)
        {
            float healthPercent = currentHealth / maxHealth;
            Color color = spriteColour.color;
            if (healthPercent <= 0.3f)
            {
                color.a = 0.3f;
            }
            else if (healthPercent <= 0.6f)
            {
                color.a = 0.6f;
            }
            else
            {
                color = Color.white;
            }
            spriteColour.color = color;
        }
    }

    private void OnDeath()
    {
        if (damagePopupPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);
            DamagePopup popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            popup.ShowPopup(destroyedScore);
        }

        if (scoreManager != null)
        {
            scoreManager.UpdateScore(destroyedScore);
        }

        StartCoroutine(BreakAndDestroy());
    }

    private System.Collections.IEnumerator BreakAndDestroy()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < breakFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / breakFadeDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}