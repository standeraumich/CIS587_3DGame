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

    [Range(0, 100)]
    public float damageSpawnPercentage = 0f, healthSpawnPercentage = 0f;


    // Physical size of our maze cells. Getting this wrong wil result in overlapping
    // Or gaps between each cell.
    public float CellSize = 1f;

    private void Start()
    {
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

        // Determine the number of maze cells to spawn damage based on the percentage
        int numberOfDamage = Mathf.RoundToInt(mazePositions.Count * (damageSpawnPercentage / 100f));

        for (int i = 0; i < numberOfDamage; i++)
        {
            // Get a random position from the mazePositions list
            int randomIndex = UnityEngine.Random.Range(0, mazePositions.Count);
            Vector2Int damagePos = mazePositions[randomIndex];
            mazePositions.RemoveAt(randomIndex); // Remove the position to avoid duplicates

            // Instantiate Damage at the selected position
            Instantiate(DamagePrefab, new Vector3(damagePos.x * CellSize, 0f, damagePos.y * CellSize), Quaternion.identity, transform);
        }

        //Determine the nubmer of maze cells to spawn health based on the percentage
        int numberOfHealth = Mathf.RoundToInt(mazePositions.Count * (damageSpawnPercentage / 100f));
        for (int i = 0; i < numberOfHealth; i++)
        {
            // Get a random position from the mazePositions list
            int randomIndex = UnityEngine.Random.Range(0, mazePositions.Count);
            Vector2Int healthPos = mazePositions[randomIndex];
            mazePositions.RemoveAt(randomIndex); // Remove the position to avoid duplicates

            // Instantiate Damage at the selected position
            Instantiate(HealthPrefab, new Vector3(healthPos.x * CellSize, 0.5f, healthPos.y * CellSize), Quaternion.identity, transform);
        }

    }
}
