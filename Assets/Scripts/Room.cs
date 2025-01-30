using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public enum RoomType { None, Battle, Shop, Boss }
    public RoomType roomType;
    public SpriteRenderer spriteRenderer;

    [Header("Room Settings")]
    public Vector2 position; // Position in the map of each room to place them 
    public List<Room> connectedRooms = new List<Room>(); // Array of connected rooms
    public GameObject linePrefab; // Reference to a prefab containing a LineRenderer
    public bool isLocked = true; //set the lock for the rooms
    private Color originalColor;       // Stores the original color of the GameObject
    private Renderer objectRenderer;  // Renderer component of the GameObject
    private List<Room> emptyRooms = new List<Room>();
    private Color colorStash;

    void Start()
    {
        DrawConnections();
        colorStash = spriteRenderer.color;
        GameObject child1 = transform.Find("child1").gameObject;
        SpriteRenderer childRenderer = child1.GetComponent<SpriteRenderer>();
        originalColor = childRenderer.color;
        ChangeLockedOpacity();
    }

    void DrawConnections()
    {
        foreach (var connectedRoom in connectedRooms)
        {
            if (connectedRoom != null)
            {
                // Instantiate a new line object
                GameObject lineObject = Instantiate(linePrefab, transform.position, Quaternion.identity, transform);
                LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

                // Set the start and end points of the line
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, connectedRoom.transform.position);
            }
        }
    }
    public void OnMouseEnter()
    {
        if (!isLocked)
        {
            GameObject child1 = transform.Find("child1").gameObject;
            //SpriteRenderer childRenderer = child1.GetComponent<SpriteRenderer>();
            //childRenderer.color = Color.green;

            child1.GetComponent<ImgChg>().ChangeSprite();
            Debug.Log ("GREEN");
        }

        return;
    }

    public void OnMouseExit()
    {
        if (!isLocked)
        {
            GameObject child1 = transform.Find("child1").gameObject;
            //    SpriteRenderer childRenderer = child1.GetComponent<SpriteRenderer>();
            //     childRenderer.color = originalColor;

            child1.GetComponent<ImgChg>().ChangeOrginal();
            Debug.Log("NoHover");
        }
        return;
    }


    private void OnMouseDown()
    {
        if (isLocked == false)
        {
            Debug.Log($"Entered {roomType} room at position {position}");
            MapManager.instance.mapParent.gameObject.SetActive(false);
            MapManager.instance.activeRoom = this;
            OnMouseExit();

            switch (roomType)
            {
                case RoomType.Battle:
                    SceneManager.LoadScene("TheScene");
                    isLocked = true;
                    ChangeLockedOpacity();
                    break;
                case RoomType.Shop:
                    SceneManager.LoadScene("ShopScene");
                    isLocked = true;
                    ChangeLockedOpacity();
                    break;
                case RoomType.Boss:
                    SceneManager.LoadScene("BossRoom");
                    isLocked = true;
                    ChangeLockedOpacity();
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

    public void ChangeLockedOpacity()
    {
        spriteRenderer.color = colorStash * (isLocked ? 0.4f : 1);
    }
}
