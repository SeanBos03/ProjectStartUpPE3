using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType { Battle, Shop, BossFight}
    public RoomType roomType;

    [Header("Room Settings")]
    public Vector2 position;  // Position in the map of each room to place them 
    public Room[] connectedRooms; // Array of connected rooms because that way we can keep track of what rooms the player can access

    void OnDrawGizmos()
    {
        // Draw lines to connected rooms in the editor for debugging 
        if (connectedRooms != null)
        {
            Gizmos.color = Color.black;
            foreach (var connectedRoom in connectedRooms)
            {
                if (connectedRoom != null)
                {
                    Gizmos.DrawLine(transform.position, connectedRoom.transform.position);
                }
            }
        }
    }

    private void OnMouseDown()
    {
        // Handle clicking on the room
        Debug.Log($"Entered {roomType} room at position {position}");
    }


// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
