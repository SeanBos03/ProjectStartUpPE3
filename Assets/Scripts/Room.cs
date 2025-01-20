using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public enum RoomType { None, Battle, Shop, Boss }
    public RoomType roomType;


    [Header("Room Settings")]
    public Vector2 position; // Position in the map of each room to place them 
    public Room[] connectedRooms; // Array of connected rooms

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
        Debug.Log($"Entered {roomType} room at position {position}");

        switch (roomType)
        {
            case RoomType.Battle:
                SceneManager.LoadScene("TheScene");
                break;
            case RoomType.Shop:
                SceneManager.LoadScene("Shop");
                break;
            case RoomType.Boss:
                SceneManager.LoadScene("BossRoom");
                break;
            default:
                Debug.LogError($"Room at position {position} has an invalid or unassigned RoomType!");
                break;
        }
    }
}