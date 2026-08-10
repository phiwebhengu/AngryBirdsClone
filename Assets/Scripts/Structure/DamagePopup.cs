using UnityEngine;
using TMPro;
using System.Collections;
//Structure takes damage
//It then displays a popup with the score amount on this scripty
//That means that this script needs to listen to Desteructible.cs and get the score amount from it
//Annnnnnd i am lost lmfoaiooooo

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro popupText; // 3D TextMeshPro, NOT TextMeshProUGUI
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float lifetime = 1f;

    public void ShowPopup(float amount)
    {
        popupText.text = "+" + Mathf.RoundToInt(amount).ToString();
        StartCoroutine(RiseAndFade());
    }

    public void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    private IEnumerator RiseAndFade()
    {
        float elapsed = 0f;
        Color startColor = popupText.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;

            transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
            popupText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}