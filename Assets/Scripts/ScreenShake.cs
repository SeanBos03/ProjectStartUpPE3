using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake activeScreenShake;
    [Header("Shake settings")]
    public float defaultNoiseSpeed;
    public float screenShakeNoiseSpeedIncrease;
    public float screenShakeReturnSpeed;
    public float screenShakeReturnSpeedRotational;
    public float maxPositionOffset;
    public float maxRotationOffset;
    public float moveSpeed;
    public float positionOffset;
    [Header("Debugging")]
    public bool screenShakeTest;
    public float screenShakeStrength;

    private Vector3 defaultLocation;
    private Quaternion defaultRotation;
    private float activeNoiseStrength;
    private float activeNoiseStrengthRotation;
    private float seed;
    private float activeOffset;
    private float activeOffsetR;

    public void Start()
    {
        activeScreenShake = this;

        defaultLocation = transform.position;
        defaultRotation = transform.rotation;

        seed = Random.Range(0, 99999);
        activeNoiseStrength = defaultNoiseSpeed;
    }

    public void Update()
    {
        Wobble();
        if(screenShakeTest)
        {
            screenShakeTest = false;
            CallScreenShake(screenShakeStrength);
        }
    }

    public void Wobble()
    {
        float positionOffsetX = (Mathf.PerlinNoise(seed + activeOffset, seed + activeOffset) - 0.5f) * 2f;
        float positionOffsetY = (Mathf.PerlinNoise(seed - activeOffset, seed + activeOffset) - 0.5f) * 2f;
        float rotationOffset = (Mathf.PerlinNoise(seed + activeOffsetR, seed - activeOffsetR) - 0.5f) * 2f;

        positionOffsetX = Mathf.Clamp(positionOffsetX * (activeNoiseStrength / 2), -maxPositionOffset, maxPositionOffset);
        positionOffsetY = Mathf.Clamp(positionOffsetY * (activeNoiseStrength / 2), -maxPositionOffset, maxPositionOffset);
        rotationOffset = Mathf.Clamp(rotationOffset * activeNoiseStrengthRotation, -maxRotationOffset, maxRotationOffset);

        Vector3 locationOffset = ((transform.up * positionOffsetY) + (transform.right * positionOffsetX)) * positionOffset;

        transform.position = Vector3.Lerp(transform.position, defaultLocation + locationOffset, moveSpeed * Time.deltaTime);

        activeOffset += Time.deltaTime * activeNoiseStrength;
        activeOffsetR += Time.deltaTime * activeNoiseStrengthRotation;
        activeNoiseStrength = Mathf.Lerp(activeNoiseStrength, defaultNoiseSpeed, screenShakeReturnSpeed * Time.deltaTime);
        activeNoiseStrengthRotation = Mathf.Lerp(activeNoiseStrengthRotation, defaultNoiseSpeed, screenShakeReturnSpeedRotational * Time.deltaTime);

        transform.rotation = defaultRotation;
        transform.Rotate(Vector3.forward * rotationOffset);
    }

    public void CallScreenShake(float strength)
    {
        activeNoiseStrength = strength * screenShakeNoiseSpeedIncrease;
        activeNoiseStrengthRotation = strength * screenShakeNoiseSpeedIncrease;
    }

    /*public static IEnumerator CallScreenShake(float duration, Vector2 direction = new Vector2(), float intensityMultiply = 1f)
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
    }*/
}
