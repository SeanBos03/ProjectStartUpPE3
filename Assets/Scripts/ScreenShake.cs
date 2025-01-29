using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake activeScreenShake;
    [Header("Shake settings")]
    public float noiseSpeed;
    public float maxPositionOffset;
    public float maxRotationOffset;
    public float directionMultiply;
    [Range(1.5f,5)]
    public float intensityPower;

    public void Start()
    {
        activeScreenShake = this;
    }

    public static IEnumerator CallScreenShake(float duration, Vector2 direction = new Vector2(), float intensityMultiply = 1f)
    {
        float time = duration;
        Transform transform = activeScreenShake.transform;
        float seed = Random.Range(0, 99999);
        float noisePosition = 0;
        while (time >= 0)
        {
            float intensity = 1f / duration * time;
            float amount = Mathf.Pow(intensity, activeScreenShake.intensityPower);
            Vector2 directionOffset = Vector2.Lerp(direction * activeScreenShake.directionMultiply, Vector2.zero, amount);

            float positionOffsetX = (Mathf.PerlinNoise(seed + noisePosition, seed + noisePosition) - 0.5f + directionOffset.x) * 2f;
            float positionOffsetY = (Mathf.PerlinNoise(seed - noisePosition, seed + noisePosition) - 0.5f + directionOffset.y) * 2f;
            float rotationOffset = (Mathf.PerlinNoise(seed + noisePosition, seed - noisePosition) - 0.5f) * 2f;

            transform.position = transform.parent.position;
            transform.rotation = transform.parent.rotation;

            transform.Translate(new Vector3(positionOffsetX * amount * activeScreenShake.maxPositionOffset, positionOffsetY * amount * activeScreenShake.maxPositionOffset) * intensityMultiply);
            transform.Rotate(Vector3.forward * rotationOffset * amount * activeScreenShake.maxRotationOffset * intensityMultiply);
            yield return null;
            time -= Time.deltaTime;
            noisePosition += Time.deltaTime * activeScreenShake.noiseSpeed;
        }
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }
}
