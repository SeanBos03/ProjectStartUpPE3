using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuck : MonoBehaviour
{
    public GameObject button; // Drag the button in here

    void Update()
    {
        if (Input.GetMouseButtonDown(1))  // 1 is the right mouse button
        {
            // Check if the right-click was over the button
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                if (rectTransform.rect.Contains(localPoint))
                {
                    Debug.Log("Right-click on the button!");
                    // Add your custom logic here
                }
            }
        }
    }
}
