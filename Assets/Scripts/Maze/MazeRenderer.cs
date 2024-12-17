using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] private GameObject mazeParent;
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject MazeCellPrefab;
    [SerializeField] GameObject ExitPrefab;
    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] GameObject HealthPrefab;
    [SerializeField] GameObject WaypointPrefab;
    private NavMeshSurface navMeshSurface;


    public float enemySpawn = 0, healthSpawn = 0;
    public int waypointCount = 0;


    // Physical size of our maze cells. Getting this wrong wil result in overlapping
    // Or gaps between each cell.
    public float CellSize = 1f;

    private void Start()
    {
        // Load the settings
        // int difficulty = PlayerPrefs.GetInt("Difficulty", 0);

        // switch (difficulty)
        // {
        //     case 0:
        //         Debug.Log("Easy Mode");
        //         enemySpawn = 1;
        //         healthSpawn = 3;
        //         waypointCount = 2;
        //         break;
        //     case 1:
        //         Debug.Log("Medium Mode");
        //         enemySpawn = 3;
        //         healthSpawn = 3;
        //         waypointCount = 3;
        //         break;
        //     case 2:
        //         Debug.Log("Hard Mode");
        //         enemySpawn = 6;
        //         healthSpawn = 2;
        //         waypointCount = 4;
        //         break;
        // }

        MazeCell[,] maze = mazeGenerator.GetMaze();

        // Create a list to store the maze cells positions
        List<Vector2Int> mazePositions = new List<Vector2Int>();

        Debug.Log("Generating Maze...");
        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                // Instantiate a new maze cell prefab as a child of the MazeRenderer object
                GameObject newCell = Instantiate(MazeCellPrefab, new Vector3((float)x * CellSize, 0f, (float)y * CellSize), Quaternion.identity, transform);

                // Get a refrence to the cell's MazeCellPrefab script
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                // Determine which walls need to be active
                bool top = maze[x, y].topWall;
                bool left = maze[x, y].leftWall;

                // Bottom and right walls are deactivated by default unless we are at the bottom or right
                bool right = false;
                bool bottom = false;
                if (x == mazeGenerator.mazeWidth - 1) right = true;
                if (y == 0) bottom = true;

                mazeCell.Init(top, bottom, right, left);

                // Add position to maze positions list
                mazePositions.Add(new Vector2Int(x, y));
            }
        }
        // Combine Meshes
        CombineMeshes();

        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();

        // Add MeshCollider to the combined mesh for collision
        gameObject.AddComponent<MeshCollider>();

        //Bake NavMeshSurface
        BakeNavMesh();

        Debug.Log("Generating Maze Exit...");
        // Modify maze to spawn an exit cell
        Vector2Int exitPos = mazeGenerator.exitPosition;
        mazePositions.Remove(new Vector2Int(0, 0));
        mazePositions.Remove(exitPos);
        SpawnExitCell(exitPos);

        Debug.Log("Spawning Health");
        // Spawn health.
        mazePositions = SpawnHealth(mazePositions);

        // // Use the Path GameObject and assign it to the GameManager Path
        // GameManager.Instance.Path = Path;

        Debug.Log("Spawning Enemies");
        // Spawn enemies
        SpawnEnemies(mazePositions);

    }

    private void SpawnExitCell(Vector2Int exitPos)
    {
        // Find the specific MazeCellObject corresponding to the exit position
        GameObject exitCellObject = GameObject.Find($"MazeCell ({exitPos.x},{exitPos.y})");
        if (exitCellObject != null)
        {
            MazeCellObject exitCell = exitCellObject.GetComponent<MazeCellObject>();
            if (exitCell != null)
            {
                if (exitPos.x == mazeGenerator.mazeWidth - 1) exitCell.rightWall.SetActive(false);
                if (exitPos.y == mazeGenerator.mazeHeight - 1) exitCell.topWall.SetActive(false);
            }
        }

        // Instantiate the exit prefab at the exit position
        Instantiate(ExitPrefab, new Vector3(exitPos.x * CellSize, 0f, exitPos.y * CellSize), Quaternion.identity, transform);

    }

    private List<Vector2Int> SpawnHealth(List<Vector2Int> mazePositions)
    {
        // Determine if number of Health is greater than cells, decrease it until its number of cells if so
        if (healthSpawn > mazePositions.Count) healthSpawn = mazePositions.Count;
        for (int i = 0; i < healthSpawn; i++)
        {
            // Get a random position from the mazePositions list
            int randomIndex = UnityEngine.Random.Range(0, mazePositions.Count);
            Vector2Int healthPos = mazePositions[randomIndex];
            mazePositions.RemoveAt(randomIndex); // Remove the position to avoid duplicates

            // Instantiate Damage at the selected position
            Instantiate(HealthPrefab, new Vector3(healthPos.x * CellSize, 1f, healthPos.y * CellSize), Quaternion.identity, transform);
        }
        return mazePositions;
    }
    private List<Transform> CreateWaypoints(List<Vector2Int> mazePositions)
    {
        List<Transform> waypointPositions = new List<Transform>();

        if (waypointCount > mazePositions.Count) waypointCount = mazePositions.Count;

        for (int i = 0; i < waypointCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, mazePositions.Count);
            Vector2Int waypointPos = mazePositions[randomIndex];
            mazePositions.RemoveAt(randomIndex);
            GameObject waypoint = Instantiate(WaypointPrefab, new Vector3(waypointPos.x * CellSize, 0f, waypointPos.y * CellSize), Quaternion.identity, transform);
            waypointPositions.Add(waypoint.transform);
        }

        return waypointPositions;
    }

    private void SpawnEnemies(List<Vector2Int> mazePositions)
    {
        List<Vector2Int> waypointMazePositions = DeepCopy(mazePositions);
        if (enemySpawn > mazePositions.Count) enemySpawn = mazePositions.Count;
        for (int i = 0; i < enemySpawn; i++)
        {
            Debug.Log("Creating waypoints for enemy number " + i);
            // Create path
            List<Transform> waypointPositions = CreateWaypoints(waypointMazePositions);

            Debug.Log("Spawning enemy number " + i);
            // Spawn enemies at random positions.
            Vector2Int randomPos = mazePositions[UnityEngine.Random.Range(0, mazePositions.Count)];
            Vector3 spawnPosition = new Vector3(randomPos.x * CellSize, 0f, randomPos.y * CellSize);
            GameObject enemy = Instantiate(EnemyPrefab, spawnPosition, Quaternion.identity, transform);
            // Add enemy patrol path
            Enemy enemyScript = enemy.AddComponent<Enemy>();
            Debug.Log(waypointPositions);
            enemyScript.path.waypoints = waypointPositions;
            // Add StateMachine
            enemy.AddComponent<StateMachine>();

        }
    }

    public static List<Vector2Int> DeepCopy(List<Vector2Int> originalList)
    {
        List<Vector2Int> copyList = new List<Vector2Int>();
        foreach (var item in originalList)
        {
            copyList.Add(new Vector2Int(item.x, item.y));
        }
        return copyList;
    }

    void CombineMeshes()
    {
        MeshFilter[] meshFilters = mazeParent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // Disable individual mesh renderers
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        mazeParent.SetActive(false); // Hide the original maze parent GameObject
        gameObject.SetActive(true); // Ensure the combined mesh GameObject is active
    }

    void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh baked successfully.");
        }
        else
        {
            Debug.LogError("NavMeshSurface component not found.");
        }
    }


}
