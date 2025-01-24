using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int theHealth;
    void Start()
    {
        theHealth = maxHealth;
    }
}
