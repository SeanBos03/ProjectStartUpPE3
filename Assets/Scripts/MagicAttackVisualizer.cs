using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class MagicAttackVisualizer : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;
    public enum ElementType { Fire, Earth, Water, Nature, Electricity}
    [Header("Elemental Counts")]
    public ElementType element1;
    public ElementType element2;

    private ElementType activeElement1;
    private ElementType activeElement2;

    public int element1Count;
    public int element2Count;

    private float activeElement1Count;
    private float activeElement2Count;

    public float multiplier;

    [Header("Visual Paramaters")]
    [SerializeField]public float changeSpeed;
    [SerializeField] private int externalMultiplier;
    [SerializeField] private int decreaselMultiplier;

    [Header("Shooting Paramaters")]
    [SerializeField] public float moveSpeed;
    [SerializeField] private float angleSpeed;
    [SerializeField] private float maxDegreeChange;
    [SerializeField] private AnimationCurve angleCurve;
    [SerializeField] private UnityEvent hitEvent;

    [Header("Testing Paramater")]
    public Transform testLocation;

    public bool callToggle;

    public float cooldownSeconds = 0.5f;
    Vector3 originalPosition; //want it so the magic can shoot again so it needs to return the position it had before shooting
    Quaternion originalRotation; //want it so the magic can shoot again so it needs to return the rotation it had beofre shooting
    public bool hitPreCooldown;
    public void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void Update()
    {
        ChangeElements();
        Visualize();
    }

    public void SetElement(bool isElement1, string theElement)
    {
        ElementType theElementType = ElementType.Fire;

        switch (theElement)
        {
            case "Element_Fire":
                theElementType = ElementType.Fire;
                break;
            case "Element_Green":
                theElementType = ElementType.Nature;
                break;
            case "Element_Ground":
                theElementType = ElementType.Earth;
                break;
            case "Element_Thunder":
                theElementType = ElementType.Electricity;
                break;
            case "Element_Water":
                theElementType = ElementType.Water;
                break;
        }

        if (isElement1)
        {
            element1 = theElementType;
        }

        else
        {
            element2 = theElementType;
        }
    }

    private void ChangeElements()
    {
        if(activeElement1 != element1)
        {
            if (activeElement1Count > 0)
            {
                activeElement1Count -= Time.deltaTime * changeSpeed * externalMultiplier * decreaselMultiplier;
            }
            else
            {
                activeElement1 = element1;
            }
        }
        else
        {
             activeElement1Count = Mathf.Lerp(activeElement1Count, element1Count * externalMultiplier, changeSpeed * Time.deltaTime);
        }

        if (activeElement2 != element2)
        {
            if (activeElement2Count > 0)
            {
                activeElement2Count -= Time.deltaTime * changeSpeed * externalMultiplier * decreaselMultiplier;
            }
            else
            {
                activeElement2 = element2;
            }
        }
        else
        {
            activeElement2Count = Mathf.Lerp(activeElement2Count, element2Count * externalMultiplier, changeSpeed * Time.deltaTime);
        }
    }

    private void Visualize()
    {
        int element1Index = 0;
        switch (activeElement1)
        {
            case ElementType.Fire:
                element1Index = 1; 
                break;
            case ElementType.Earth:
                element1Index = 2; 
                break;
            case ElementType.Water:
                element1Index = 3; 
                break;
            case ElementType.Nature:
                element1Index = 4; 
                break;
            case ElementType.Electricity:
                element1Index = 5; 
                break;
        }

        int element2Index = 0;
        switch (activeElement2)
        {
            case ElementType.Fire:
                element2Index = 1;
                break;
            case ElementType.Earth:
                element2Index = 2;
                break;
            case ElementType.Water:
                element2Index = 3;
                break;
            case ElementType.Nature:
                element2Index = 4;
                break;
            case ElementType.Electricity:
                element2Index = 5;
                break;
        }

        effect.SetInt("Element1", element1Index);
        effect.SetInt("Element1_Count", Mathf.RoundToInt(activeElement1Count));

        effect.SetInt("Element2", element2Index);
        effect.SetInt("Element2_Count", Mathf.RoundToInt(activeElement2Count));
    }

    public void Shoot(Vector3 location)
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        StartCoroutine(Shooting(location));
    }

    private IEnumerator Shooting(Vector3 location)
    {
        Vector3 dir = -(transform.position - location).normalized;
        Vector3 angle = Quaternion.AngleAxis(90, Vector3.up) * dir;
        float activeAngleChange = 0;

        while(Vector3.Distance(transform.position, location) >= 0.5f)
        {
            transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
            if(activeAngleChange <= maxDegreeChange)
            {
                float value = 1f / maxDegreeChange * activeAngleChange;
                float multiply = angleCurve.Evaluate(value);

                transform.Rotate(angle * angleSpeed * Time.deltaTime * multiply, Space.World);
                activeAngleChange += Mathf.Abs(angleSpeed * Time.deltaTime * multiply);
            }
            yield return null;
        }

        hitPreCooldown = true;
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownSeconds);
        hitPreCooldown = false;
        element1Count = 0;
        element2Count = 0;
        hitEvent.Invoke();
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
