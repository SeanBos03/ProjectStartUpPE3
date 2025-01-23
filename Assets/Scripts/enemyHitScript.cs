using System.Collections;
using UnityEngine;

public class BounceOnHit : MonoBehaviour
{
    public float bounceHeight = 0.5f; // Height of the bounce
    public float bounceSpeed = 5f;   // Speed of the bounce
    public int bounceCount = 3;      // Number of bounces

    private Vector3 originalPosition;
    private bool isBouncing = false;

    void Start()
    {
        // Save the original position of the GameObject
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with {collision.gameObject.name}");
        // Start bouncing if not already bouncing
        if (!isBouncing)
        {
            StartCoroutine(Bounce());
        }
    }

    IEnumerator Bounce()
    {
        isBouncing = true;
        for (int i = 0; i < bounceCount; i++)
        {
            // Move up
            float elapsedTime = 0f;
            while (elapsedTime < 1f / bounceSpeed)
            {
                transform.position = Vector3.Lerp(originalPosition, originalPosition + Vector3.up * bounceHeight, elapsedTime * bounceSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Move down
            elapsedTime = 0f;
            while (elapsedTime < 1f / bounceSpeed)
            {
                transform.position = Vector3.Lerp(originalPosition + Vector3.up * bounceHeight, originalPosition, elapsedTime * bounceSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // Ensure it ends at the original position
        transform.position = originalPosition;
        isBouncing = false;
    }
}