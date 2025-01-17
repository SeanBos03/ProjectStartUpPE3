using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Array of prefabs (Battle, Shop, etc.)
    public Transform mapParent;     // Parent object to hold all rooms

    [Header("Map Settings")]
    public Vector2[] roomPositions = {
    new Vector2(0, 0),
    new Vector2(2, 0),
    new Vector2(4, 0),
    new Vector2(2, -2),
    new Vector2(1, 1),
};                              // Define positions for the rooms

public int[,] connections = {
    { 0, 1, 0, 0, 0 }, // Room 1 connected to Room 2
    { 1, 0, 1, 1, 0 }, // Room 2 connected to Room 1, 3, 4
    { 0, 1, 0, 0, 1 }, // Room 3 connected to Room 2, 5
    { 0, 1, 0, 0, 1 }, // Room 4 connected to Room 2, 5
    { 0, 0, 1, 1, 0 }, // Room 5 connected to Room 3, 4
};                        // Adjacency matrix for connections (1 = connected, 0 = not)

    private GameObject[] rooms;     // Keep track of instantiated rooms

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        rooms = new GameObject[roomPositions.Length];

        // Instantiate rooms
        for (int i = 0; i < roomPositions.Length; i++)
        {
            int prefabIndex = Random.Range(0, roomPrefabs.Length); // Pick a random room type
            GameObject room = Instantiate(roomPrefabs[prefabIndex], roomPositions[i], Quaternion.identity, mapParent);
            room.name = $"Room {i + 1}";
            room.GetComponent<Room>().position = roomPositions[i];
            rooms[i] = room;
        }

        // Connect rooms
        for (int i = 0; i < roomPositions.Length; i++)
        {
            Room roomScript = rooms[i].GetComponent<Room>();
            roomScript.connectedRooms = new Room[roomPositions.Length];

            for (int j = 0; j < roomPositions.Length; j++)
            {
                if (connections[i, j] == 1)
                {
                    roomScript.connectedRooms[j] = rooms[j].GetComponent<Room>();
                }
            }
        }
    }
}
