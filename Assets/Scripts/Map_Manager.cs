using System.Collections.Generic;
using UnityEngine;
using static Room;

public class MapManager : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Array of prefabs (Battle, Shop, etc.)
    public Transform mapParent; // Parent object to hold all rooms
    public int maxShops = 1; // Maximum number of shops
    public GameObject bossRoomPrefab; // Boss Room Prefab

    [Header("Map Settings")]
    public Vector2[] roomPositions = {
        new Vector2(0, 0),
        new Vector2(2, 0),
        new Vector2(4, 0),
        new Vector2(2, -2),
        new Vector2(1, 1),
    };

    public int[,] connections = {
        { 0, 1, 0, 0, 0 }, // Room 1 connected to Room 2
        { 1, 0, 1, 1, 0 }, // Room 2 connected to Room 1, 3, 4
        { 0, 1, 0, 0, 1 }, // Room 3 connected to Room 2, 5
        { 0, 1, 0, 0, 1 }, // Room 4 connected to Room 2, 5
        { 0, 0, 1, 1, 0 }, // Room 5 connected to Room 3, 4
    };

    private GameObject[] rooms; // Keep track of instantiated rooms

    void Start()
    {
        GenerateMap();
    }

    void CustomizeRoom(GameObject room, Room.RoomType roomType)
    {
        // Customize room appearance based on type
        var spriteRenderer = room.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        switch (roomType)
        {
            case Room.RoomType.Shop:
                spriteRenderer.color = Color.yellow;
                break;
            case Room.RoomType.Boss:
                spriteRenderer.color = Color.red;
                break;
            case Room.RoomType.Battle:
                spriteRenderer.color = Color.black;
                break;
        }
    }

    void GenerateMap()
    {
        rooms = new GameObject[roomPositions.Length];
        int shopCount = 0;

        for (int i = 0; i < roomPositions.Length; i++)
        {
            Room.RoomType roomType = Room.RoomType.Battle;

            // Set room type
            if (i == roomPositions.Length - 1) // Last room is always Boss
            {
                roomType = Room.RoomType.Boss;
            }
            else if (shopCount < maxShops && Random.value > 0.7f) // Random chance for Shop
            {
                roomType = Room.RoomType.Shop;
                shopCount++;
            }

            // Instantiate the appropriate prefab
            GameObject prefabToInstantiate = (roomType == Room.RoomType.Boss) ? bossRoomPrefab : roomPrefabs[Random.Range(0, roomPrefabs.Length)];
            GameObject roomObject = Instantiate(prefabToInstantiate, roomPositions[i], Quaternion.identity, mapParent);

            // Assign properties to Room component
            Room roomScript = roomObject.GetComponent<Room>();

            roomScript.roomType = roomType;
            roomScript.position = roomPositions[i];

            rooms[i] = roomObject;

            // Customize the room's appearance
            CustomizeRoom(roomObject, roomType);
        }

        // Connect rooms
        for (int i = 0; i < roomPositions.Length; i++)
        {
            Room roomScript = rooms[i].GetComponent<Room>();
            List<Room> connectedRoomList = new List<Room>();

            for (int j = 0; j < roomPositions.Length; j++)
            {
                if (connections[i, j] == 1)
                {
                    connectedRoomList.Add(rooms[j].GetComponent<Room>());
                }
            }
            roomScript.connectedRooms = connectedRoomList.ToArray();
        }

        // Final Debugging to ensure no duplicate shops
        int totalShops = 0;
        foreach (var room in rooms)
        {
            if (room.GetComponent<Room>().roomType == Room.RoomType.Shop)
            {
                totalShops++;
            }
        }
        Debug.Log($"Total Shops Generated: {totalShops}");
    }
}