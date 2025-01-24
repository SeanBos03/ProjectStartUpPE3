using UnityEngine;

public class HighlightCard : MonoBehaviour
{
    public Material highlightMaterial; // The material to apply for highlighting
    private Transform highlightedObject; // The currently highlighted object
    private Material originalMaterial; // Cached original material of the highlighted object

    void Update()
    {
        // Cast a ray from the camera to the cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Transform hitObject = hit.transform;

            // If the object hit is different from the currently highlighted object
            if (highlightedObject != hitObject)
            {
                // Remove highlight from the previously highlighted object
                RemoveHighlight();

                // Apply highlight to the new object
                Renderer renderer = hitObject.GetComponent<Renderer>();
                // Cache the original material
                originalMaterial = renderer.material;
                // Apply the highlight material
                renderer.material = highlightMaterial;

                // Update the currently highlighted object
                highlightedObject = hitObject;
            }
        }
        else
        {
            // Remove highlight if no object is hit
            RemoveHighlight();
        }
    }

    void RemoveHighlight()
    {
        // If there is an object currently highlighted
        if (highlightedObject != null)
        {
            Renderer renderer = highlightedObject.GetComponent<Renderer>();
            // Restore the original material
            renderer.material = originalMaterial;

            // Clear the highlighted object reference
            highlightedObject = null;
        }
    }
}