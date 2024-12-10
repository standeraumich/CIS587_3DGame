using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject MazeCellPrefab;
    [SerializeField] GameObject ExitPrefab;

    // Physical size of our maze cells. Getting this wrong wil result in overlapping
    // Or gaps between each cell.
    public float CellSize = 1f;

    private void Start()
    {
        MazeCell[,] maze = mazeGenerator.GetMaze();

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
            }
        }



        Vector2Int exitPos = mazeGenerator.exitPosition;

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
}
