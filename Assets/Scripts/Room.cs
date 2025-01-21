using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public enum RoomType { None, Battle, Shop, Boss }
    public RoomType roomType;

    [Header("Room Settings")]
    public Vector2 position; // Position in the map of each room to place them 
    public List<Room> connectedRooms = new List<Room>(); // Array of connected rooms
    public GameObject linePrefab; // Reference to a prefab containing a LineRenderer
    public bool isLocked = true; //set the lock for the rooms

    void Start()
    {
        DrawConnections();
    }

    void DrawConnections()
    {
        foreach (var connectedRoom in connectedRooms)
        {
            if (connectedRoom != null)
            {
                // Instantiate a new line object
                GameObject lineObject = Instantiate(linePrefab, transform.position, Quaternion.identity);
                LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

                // Set the start and end points of the line
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, connectedRoom.transform.position);
            }
        }
    }

    private void OnMouseDown()
    {
        if (isLocked == false)
        {
            Debug.Log($"Entered {roomType} room at position {position}");

            switch (roomType)
            {
                case RoomType.Battle:
                    SceneManager.LoadScene("TheScene");
                    break;
                case RoomType.Shop:
                    SceneManager.LoadScene("ShopScene");
                    break;
                case RoomType.Boss:
                    SceneManager.LoadScene("BossRoom");
                    break;
                default:
                    Debug.LogError($"Room at position {position} has an invalid or unassigned RoomType!");
                    break;
            }
        }
        else if (isLocked == true)
        {
            Debug.Log($"The Room {roomType} is locked");
        }
    }
}
