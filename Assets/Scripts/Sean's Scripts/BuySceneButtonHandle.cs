using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySceneButtonHandle : MonoBehaviour
{
    public BuyMixtureScript theMixtureScript;  // Reference to the script handling mixture purchases
    public List<Button> buttons;              // List of buttons
    public List<GameData.Mixture> mixtures;   // Matching list of mixtures
    void Start()
    {
        // Ensure the lists have the same length to avoid mismatches
        if (buttons.Count != mixtures.Count)
        {
            Debug.LogError("Buttons list and Mixtures list must have the same length!");
            return;
        }

        // Loop through the buttons and add their corresponding listeners
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;  // Capture the index to avoid closure issues
            buttons[i].onClick.AddListener(() => theMixtureScript.BuyMixture(mixtures[index]));
        }
    }
}
