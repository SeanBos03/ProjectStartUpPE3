using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Array of prefabs (Battle, Shop, etc.)
    public GameObject bossRoomPrefab; // Boss Room Prefab
    public Transform mapParent; // Parent object to hold all rooms

    [Header("Map Settings")]
    public int totalRooms = 10; // Total number of rooms in the map
    public float branchProbability = 0.4f; // Probability of branching for a new room
    public float roomSpacing = 3f; // Minimum space between rooms
    public float maxRoomOffset = 2f; // Maximum horizontal/vertical offset to ensure varied room placement

    private List<Room> rooms; // List of all the rooms
    private List<List<Room>> activePaths; // List of active paths (branches)
    private Room startRoom; // Start room
    private Room bossRoom; // Boss room

    void Start()
    {
        GenerateMap();
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

    void GenerateMap()
    {
        rooms = new List<Room>();
        activePaths = new List<List<Room>>();

        // Start with the first room
        List<Room> startPath = new List<Room>();
        Vector2 lastRoomPosition = Vector2.zero;
        startRoom = CreateRoom(Room.RoomType.Battle, lastRoomPosition);
        startPath.Add(startRoom);
        rooms.Add(startRoom);
        activePaths.Add(startPath);

        // Generate the rest of the rooms, adding branches
        for (int i = 1; i < totalRooms - 1; i++) // Last room is always Boss, no need to generate it yet
        {
            // Randomly choose a path (one of the active paths)
            List<Room> currentPath = activePaths[Random.Range(0, activePaths.Count)];

            // Randomly choose a room type
            Room.RoomType roomType = GetRandomRoomType(i);

            // Get position for the new room, creating some variation in direction
            Vector2 newRoomPosition = GetNewRoomPosition(currentPath);

            // Ensure the room doesn't overlap with other rooms in the current path
            newRoomPosition = FindValidPosition(newRoomPosition, currentPath);

            // Create and place the new room
            Room newRoom = CreateRoom(roomType, newRoomPosition);
            rooms.Add(newRoom);

            // Connect the new room to the last room in the current path
            ConnectRooms(currentPath[currentPath.Count - 1], newRoom);
            currentPath.Add(newRoom);

            // Decide if we should branch off from this path
            if (Random.value < branchProbability)
            {
                // Create a branch off the current path
                List<Room> branchPath = new List<Room>();
                Room branchRoom = CreateRoom(roomType, newRoomPosition + new Vector2(Random.Range(-maxRoomOffset, maxRoomOffset), Random.Range(-maxRoomOffset, maxRoomOffset)));
                rooms.Add(branchRoom);
                ConnectRooms(newRoom, branchRoom);
                branchPath.Add(branchRoom);
                activePaths.Add(branchPath); // Add the new branch to active paths
            }
        }

        // Final Boss room
        bossRoom = CreateRoom(Room.RoomType.Boss, activePaths[0][activePaths[0].Count - 1].position + new Vector2(3, 0));
        rooms.Add(bossRoom);
        //activePaths[0].Add(bossRoom);

        // Connect the last room (Boss) to the previous room
       // ConnectRooms(activePaths[0][activePaths[0].Count - 2], bossRoom);

        // Ensure the Boss room has at least one path leading to it
        EnsureBossRoomConnection();

        // Debugging output
        Debug.Log("Map Generated with rooms: " + rooms.Count);
    }

    Room CreateRoom(Room.RoomType roomType, Vector2 position)
    {
        GameObject roomPrefab = GetRoomPrefab(roomType);
        GameObject roomObject = Instantiate(roomPrefab, position, Quaternion.identity, mapParent);
        Room roomScript = roomObject.GetComponent<Room>();

        roomScript.roomType = roomType;
        roomScript.position = position;

        // Customize the room based on type
        CustomizeRoom(roomObject, roomType);

        return roomScript;
    }

    GameObject GetRoomPrefab(Room.RoomType roomType)
    {
        switch (roomType)
        {
            case Room.RoomType.Boss:
                return bossRoomPrefab;
            default:
                return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        }
    }

    Room.RoomType GetRandomRoomType(int index)
    {
        // Randomly pick a room type, but ensure the last room is always the Boss
        if (index == totalRooms - 1)
        {
            return Room.RoomType.Boss;
        }
        else if (Random.value < 0.3f)
        {
            return Room.RoomType.Shop;
        }
        else
        {
            return Room.RoomType.Battle;
        }
    }

    void ConnectRooms(Room roomA, Room roomB)
    {
        // Connect two rooms bidirectionally
        roomA.connectedRooms = AddToArray(roomA.connectedRooms, roomB);
        roomB.connectedRooms = AddToArray(roomB.connectedRooms, roomA);
    }

    Room[] AddToArray(Room[] array, Room item)
    {
        List<Room> list = new List<Room>(array);
        list.Add(item);
        return list.ToArray();
    }

    Vector2 GetNewRoomPosition(List<Room> currentPath)
    {
        // Create variation by adding small random offsets
        Room lastRoom = currentPath[currentPath.Count - 1];
        Vector2 lastRoomPosition = lastRoom.position;
        return lastRoomPosition + new Vector2(2, Random.Range(-1f, 1f)); // slight vertical variation
    }

    Vector2 FindValidPosition(Vector2 desiredPosition, List<Room> currentPath)
    {
        // Try different positions until we find one that doesn't overlap with other rooms in the current path
        Vector2 offset = Vector2.zero;
        int maxAttempts = 50;

        for (int i = 0; i < maxAttempts; i++)
        {
            bool isValid = true;

            // Only check for collisions with rooms in the current path
            foreach (Room room in currentPath)
            {
                if (Vector2.Distance(room.position, desiredPosition + offset) < roomSpacing)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                return desiredPosition + offset;
            }
            else
            {
                offset = new Vector2(Random.Range(-maxRoomOffset, maxRoomOffset), Random.Range(-maxRoomOffset, maxRoomOffset)); // Add small random offset
            }
        }

        // If no valid position found, return the original position
        return desiredPosition;
    }

    // Ensure the Boss room is connected and not a dead-end
    void EnsureBossRoomConnection()
    {

        foreach( List<Room> path in activePaths)
        {
            Room lastRoom = path[path.Count - 1];
         
            ConnectRooms(lastRoom, bossRoom);
        }
        /*
        // Find a room that's connected to the boss room, if it's not connected to any, create a path to it
        if (bossRoom.connectedRooms.Length == 0)
        {
            // Pick a random room and connect it to the boss room
            Room randomRoom = rooms[Random.Range(0, rooms.Count)];
            ConnectRooms(randomRoom, bossRoom);
        }
        */
    }
}
