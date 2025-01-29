using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
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
    [SerializeField]private float changeSpeed;
    [SerializeField] private int externalMultiplier;
    [SerializeField] private int decreaselMultiplier;

    [Header("Shooting Paramaters")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float angleSpeed;
    [SerializeField] private float maxDegreeChange;
    [SerializeField] private AnimationCurve angleCurve;
    [SerializeField] private UnityEvent hitEvent;

    [Header("Testing Paramater")]
    public Transform testLocation;
    public bool callToggle;

    public void Update()
    {
        ChangeElements();
        Visualize();

        if(callToggle)
        {
            callToggle = false;
            Shoot(testLocation.position);
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

        Destroy(gameObject);
        hitEvent.Invoke();
        //Exploding part
    }
}
