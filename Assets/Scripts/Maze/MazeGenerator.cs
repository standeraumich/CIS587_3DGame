using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 50)]
    public int mazeWidth = 5, mazeHeight = 5; //Dimensions of the maze
    public int startX, startY; //The position our algorithm will start from
    MazeCell[,] maze; //An array of maze cells represneting the maze grid 

    Vector2Int currentCell; // The maze cell we're currently looking at 

    public Vector2Int exitPosition;

    public MazeCell[,] GetMaze()
    {
        // Load the size 
        int size = PlayerPrefs.GetInt("Size", 0);

        // Handle size settings
        switch (size)
        {
            case 0:
                Debug.Log("Small Size");
                mazeHeight = 5;
                mazeWidth=5;
                break;
            case 1:
                Debug.Log("Medium Size");
                mazeHeight = 10;
                mazeWidth = 10;
                break;
            case 2:
                Debug.Log("Large Size");
                mazeHeight = 15;
                mazeWidth = 15;
                break;
        }

        maze = new MazeCell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        CarvePath(startX, startY);

        return maze;
    }
    List<Direction> directions = new List<Direction>{
    Direction.Up, Direction.Down, Direction.Right, Direction.Left,
    };

    List<Direction> GetRandomDirections()
    {
        //Make a copy of our list 
        List<Direction> dir = new List<Direction>(directions);

        //Make a directions list to put our randomised directions into.
        List<Direction> rndDir = new List<Direction>();

        while (dir.Count > 0)
        {
            int rnd = UnityEngine.Random.Range(0, dir.Count);
            rndDir.Add(dir[rnd]);
            dir.RemoveAt(rnd);
        }

        return rndDir;
    }

    bool IsCellValid(int x, int y)
    {
        // If the cell is outside of the map or has already been visited, we consider it not valid.
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || maze[x, y].visited) return false;
        else return true;
    }

    Vector2Int CheckNeighbors()
    {
        List<Direction> rndDir = GetRandomDirections();

        for (int i = 0; i < rndDir.Count; i++)
        {
            // Set neighbor coordinates to current cell for now.
            Vector2Int neighbor = currentCell;

            // Modify neighbor coordinates based on the random directions we're currenlty trying
            switch (rndDir[i])
            {
                case Direction.Up:
                    neighbor.y++;
                    break;
                case Direction.Down:
                    neighbor.y--;
                    break;
                case Direction.Right:
                    neighbor.x++;
                    break;
                case Direction.Left:
                    neighbor.x--;
                    break;
            }

            // If the neighbor we just tried is valid, we can return that neighbor. If not, we go again.
            if (IsCellValid(neighbor.x, neighbor.y)) return neighbor;
        }
        return currentCell;
    }

    // Takes in two maze positions and set the cells accordingly
    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        if (primaryCell.x > secondaryCell.x) //Primary cell's left wall
        {
            maze[primaryCell.x, primaryCell.y].leftWall = false;
        }
        else if (primaryCell.x < secondaryCell.x)
        {
            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        }
        else if (primaryCell.y < secondaryCell.y) //Primary cell's top wall
        {
            maze[primaryCell.x, primaryCell.y].topWall = false;
        }
        else if (primaryCell.y > secondaryCell.y)
        {
            maze[secondaryCell.x, secondaryCell.y].topWall = false;
        }
    }

    // Starting at the x, y passed in, carves a path through the maze unitl it encounters a "dead end"
    void CarvePath(int x, int y)
    {
        //Perform a quick check to make sure our start position is withijn the boundaries of the map
        // If not, set them to a default (I'm using 0) and throw a little warning up 
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            x = y = 0;
            Debug.LogWarning("Starting position is out of bounds, defaulting to 0, 0");
        }

        // Set current cell to the staring position we were passed
        currentCell = new Vector2Int(x, y);


        // A list to keep track of our current path
        List<Vector2Int> path = new List<Vector2Int>();

        // Loop until we encounter a dead end
        bool deadEnd = false;
        while (!deadEnd)
        {

            // Get the next cell we're going to try
            Vector2Int nextCell = CheckNeighbors();

            // If that cell had no valid neighbors, set deadend to true so we break out of the loop
            if (nextCell == currentCell)
            {
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    currentCell = path[i]; // Set currentCell to the next step back along our path
                    path.RemoveAt(i); // remove this step from the path that didnt work
                    nextCell = CheckNeighbors(); // Check that cell to see if any other neighbors are valid

                    //If we find a valid neighbor, break out of the loop
                    if (nextCell != currentCell) break;
                }

                if (nextCell == currentCell)
                    deadEnd = true;
            }
            else
            {
                BreakWalls(currentCell, nextCell); // set wall flags on these two cells.
                maze[currentCell.x, currentCell.y].visited = true; // set to visited before moving on
                currentCell = nextCell; // Set the current cell to the valid neighbor we found
                path.Add(currentCell); // Add this cell to our path
            }

        }
        // Set the exit position (e.g., bottom-right corner)
        exitPosition = new Vector2Int(mazeWidth - 1, mazeHeight - 1);

    }

}


public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MazeCell
{
    public bool visited;
    public int x, y;
    public bool topWall;
    public bool leftWall;

    //Return x and Y as a Vector2Int for convenience sake
    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell(int x, int y)
    {
        // The coordinates of this cell in the maze grid
        this.x = x;
        this.y = y;

        //Whether the algorithm has visited this cell or not - false to start 
        visited = false;

        //All walls are present until the algorithm removes them.
        topWall = leftWall = true;
    }
}