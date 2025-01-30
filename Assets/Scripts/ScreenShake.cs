using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake activeScreenShake;
    public float lerpSpeed;
    public Camera cam;

    [Header("Wobble settings")]
    public float wobbleSpeed;
    public float wobbleAmount;

    private float activeOffset;

    [Header("ScreenShake settings")]
    public float screenDefaultShakeIntensity;
    public float angleOffset;
    public AnimationCurve intensityCurve;

    private IEnumerator activeLoop;

    [Header("Fov")]
    public float defaultFov;
    public float addedFov;
    public AnimationCurve fovCurve;

    private Vector3 screenShakeOffset;

    [Header("Debugging")]
    public bool screenShakeTest;
    public float screenShakeStrength;
    public float screenShakeTime;

    private Vector3 defaultLocation;

    public void Start()
    {
        activeScreenShake = this;
        defaultLocation = transform.position;
    }

    public void Update()
    {
        Wobble();
        if(screenShakeTest)
        {
            screenShakeTest = false;
            CallScreenShake(screenShakeStrength, screenShakeTime);
        }
    }

    public void Wobble()
    {
        float WobbleX = (Mathf.PerlinNoise(activeOffset, activeOffset) - 0.5f) * 2f * wobbleAmount;
        float WobbleY = (Mathf.PerlinNoise(activeOffset, activeOffset) - 0.5f) * 2f * wobbleAmount;

        Vector3 offset = transform.right * WobbleX + transform.up * WobbleY;
        activeOffset += Time.deltaTime * wobbleSpeed;

        transform.position = Vector3.Lerp(transform.position, defaultLocation + offset + screenShakeOffset, lerpSpeed * Time.deltaTime);
    }

    public void CallScreenShake(float strength, float time = 0.6f)
    {
        if (activeLoop != null)
            StopCoroutine(activeLoop);

        activeLoop = ScreenShakeLoop(strength, time);
        StartCoroutine(activeLoop);
    }

    public IEnumerator ScreenShakeLoop(float strength, float time)
    {
        Vector3 dir = (Vector3.down + Vector3.right * angleOffset).normalized;

        float activeTime = 0;
        while(activeTime <= time)
        {
            float checkAmount = 1f / time * activeTime;
            screenShakeOffset = dir * intensityCurve.Evaluate(checkAmount) * strength * screenDefaultShakeIntensity;
            cam.fieldOfView = Mathf.Lerp(defaultFov, defaultFov + addedFov, fovCurve.Evaluate(checkAmount) * strength);

            activeTime += Time.deltaTime;
            yield return null;
        }
        screenShakeOffset = Vector3.zero;
        cam.fieldOfView = defaultFov;

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
