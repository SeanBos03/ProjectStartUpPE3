using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    private List<GameObject> listOfPlayerCards = new List<GameObject>();
    GameObject card1 = null; //first card player is selecting
    GameObject card2 = null; //second card play is sleecting

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.5f);

            //see if play clicks on a card
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Card"))
                {
                    HandleCardClick(hit.collider.gameObject);
                }
            }
        }
    }

    void HandleCardClick(GameObject cardObject)
    {
        // Custom logic for handling card click
        Debug.Log("Handling click for card: " + cardObject.name);

        if (card1 == null)
        {
            card1 = cardObject;
            card1.transform.Find("cardMark").gameObject.SetActive(true);
            Debug.Log("card 1 selected");
            return;
        }

        if (card2 == null && cardObject != card1)
        {
            card2 = cardObject;
            card2.transform.Find("cardMark").gameObject.SetActive(true);
            Debug.Log("card 2 selected");
            return;
        }
    }
}
