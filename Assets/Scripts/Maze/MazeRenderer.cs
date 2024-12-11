using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject MazeCellPrefab;
    [SerializeField] GameObject ExitPrefab;
    [SerializeField] GameObject DamagePrefab;
    [SerializeField] GameObject HealthPrefab;

    public float damageSpawn = 0, healthSpawn = 0;


    // Physical size of our maze cells. Getting this wrong wil result in overlapping
    // Or gaps between each cell.
    public float CellSize = 1f;

    private void Start()
    {
        // Load the settings
        int difficulty = PlayerPrefs.GetInt("Difficulty", 0);

        switch (difficulty)
        {
            case 0:
                Debug.Log("Easy Mode");
                damageSpawn = 1;
                healthSpawn = 3;
                break;
            case 1:
                Debug.Log("Medium Mode");
                damageSpawn = 3;
                healthSpawn = 3;
                break;
            case 2:
                Debug.Log("Hard Mode");
                damageSpawn = 6;
                healthSpawn = 2;
                break;
        }

        MazeCell[,] maze = mazeGenerator.GetMaze();

        // Create a list to store the maze cells positions
        List<Vector2Int> mazePositions = new List<Vector2Int>();


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



        Vector2Int exitPos = mazeGenerator.exitPosition;

        // Remove player start position and exit position from the maze positions list
        mazePositions.Remove(new Vector2Int(0, 0));
        mazePositions.Remove(exitPos);


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

        // Determine if number of Damage is greater than cells, decrease it until its number of cells if so
        if (damageSpawn > mazePositions.Count) damageSpawn = mazePositions.Count - 2;

        for (int i = 0; i < damageSpawn; i++)
        {
            // Get a random position from the mazePositions list
            int randomIndex = UnityEngine.Random.Range(0, mazePositions.Count);
            Vector2Int damagePos = mazePositions[randomIndex];
            mazePositions.RemoveAt(randomIndex); // Remove the position to avoid duplicates

            // Instantiate Damage at the selected position
            Instantiate(DamagePrefab, new Vector3(damagePos.x * CellSize, 0f, damagePos.y * CellSize), Quaternion.identity, transform);
        }

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

    }
}
