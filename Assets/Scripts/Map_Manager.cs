using System.Collections.Generic;
using UnityEngine;
using static Room;

public class MapManager : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Array of room prefabs (Battle, Shop, etc.)
    public GameObject bossRoomPrefab; // Boss Room prefab
    public GameObject startRoomPrefab; // Start Room prefab
    public Transform mapParent; // Parent object to hold all rooms

    [Header("Map Settings")]
    public int totalPillars = 5; // Number of vertical pillars
    public float pillarSpacing = 5f; // Horizontal spacing between pillars
    public int minRoomsPerPillar = 1; // Minimum number of rooms per pillar
    public int maxRoomsPerPillar = 3; // Maximum number of rooms per pillar
    public float verticalSpacing = 3f; // Vertical spacing between rooms within a pillar
    public Vector2 mapMinBounds; // Minimum bounds of the map
    public Vector2 mapMaxBounds; // Maximum bounds of the map

    private List<Room> rooms; // List of all generated rooms

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        rooms = new List<Room>();
        List<Room> rowStash = new List<Room>();

        // Step 1: Create the start room at the far-left pillar
        Vector2 startRoomPosition = new Vector2(mapMinBounds.x, 0);
        Room startRoom = CreateRoom(Room.RoomType.Battle, startRoomPosition, startRoomPrefab);
        rooms.Add(startRoom);
        rowStash.Add(startRoom);
        startRoom.isLocked = false;

        // Step 2: Generate rooms for each pillar
        for (int pillarIndex = 1; pillarIndex <= totalPillars; pillarIndex++)
        {
            List<Room> currentRow = new List<Room>();

            // Calculate the x-position for this pillar
            float pillarX = mapMinBounds.x + pillarIndex * pillarSpacing;

            // Generate a random number of rooms for this pillar
            int roomCount = Random.Range(minRoomsPerPillar, maxRoomsPerPillar + 1);

            // Create rooms along the y-axis for this pillar
            for (int roomIndex = 0; roomIndex < roomCount; roomIndex++)
            {
                float roomY = mapMinBounds.y + (roomIndex * verticalSpacing) - ((verticalSpacing * roomCount) * 0.5f) + (verticalSpacing * 0.5f);
                Vector2 roomPosition = new Vector2(pillarX, roomY);

                // Randomly choose a room type
                Room.RoomType roomType = GetRandomRoomType();
                foreach (GameObject Room in roomPrefabs)
                {
                    if (Room.GetComponent<Room>().roomType == roomType)
                    {
                        // Create the room
                        Room newRoom = CreateRoom(roomType, roomPosition, Room);
                        int currentIndex = currentRow.Count;
                        rooms.Add(newRoom);
                        currentRow.Add(newRoom);
                        while (true)
                        {
                            if(rowStash.Count - 1 >= currentIndex)
                            {
                                rowStash[currentIndex].connectedRooms.Add(newRoom);
                                break;
                            }
                            else
                                currentIndex --;    
                        }
                        break;
                    }

                }

            }
            foreach (Room room in rowStash)
            {
                if(room.connectedRooms.Count == 0)
                {
                    room.connectedRooms.Add(currentRow[currentRow.Count - 1]);
                }
            }
            rowStash = currentRow;
        }

        // Step 3: Create the boss room at the far-right pillar
        Vector2 bossRoomPosition = new Vector2((totalPillars + 1)* pillarSpacing, 0);
        Room bossRoom = CreateRoom(Room.RoomType.Boss, bossRoomPosition, bossRoomPrefab);
        rooms.Add(bossRoom);
        foreach (Room room in rowStash)
        {
            room.connectedRooms.Add(bossRoom);
        }

        // Debugging output
        Debug.Log("Map generated with total rooms: " + rooms.Count);
    }

    Room CreateRoom(Room.RoomType roomType, Vector2 position, GameObject customPrefab = null)
    {
        // Use the provided prefab if available, otherwise select a random prefab
        GameObject roomPrefab = customPrefab ?? roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        GameObject roomObject = Instantiate(roomPrefab, position, Quaternion.identity, mapParent);

        // Assign properties to the Room script
        Room roomScript = roomObject.GetComponent<Room>();
        roomScript.roomType = roomType;
        roomScript.position = position;

        // Customize the room based on type
        CustomizeRoom(roomObject, roomType);

        return roomScript;
    }

    Room.RoomType GetRandomRoomType()
    {
        // Randomly pick a room type (excluding None and Boss)
        if (Random.value < 0.3f)
        {
            return Room.RoomType.Shop;
        }
        return Room.RoomType.Battle;
    }

    void CustomizeRoom(GameObject room, Room.RoomType roomType)
    {
        // Customize room appearance based on room type
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
}
