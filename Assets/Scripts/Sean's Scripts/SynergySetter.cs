using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergySetter : MonoBehaviour
{
    [SerializeField] List<GameData.Combo> synergyList = new List<GameData.Combo>();
    void Start()
    {
        if (GameData.startedComboSet)
        {
            return;
        }

        GameData.startedComboSet = true;

        foreach (GameData.Combo theCombo in synergyList)
        {
            GameData.combosKnown.Add(theCombo);
        }
    }
}
