using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

public class BreadthFirstSearch
{
    private int width;
    private int height;
    private List<int[]> tiles;
    private Room currentRoom;
    private Vector2i? blockedEnemyTileIndex; 
    private int maxDepth;

    public BreadthFirstSearch(int width, int height, Room currentRoom, Vector2i? blockedEnemyTileIndex, int maxDepth)
    {
        this.width = width;
        this.height = height;
        this.currentRoom = currentRoom;
        tiles = currentRoom.Map;
        this.blockedEnemyTileIndex = blockedEnemyTileIndex;
        this.maxDepth = maxDepth;
    }

    public List<Vector2i> FindPath(Vector2i startPos, Vector2i targetPos)
    {
        bool[,] visited = new bool[width, height];
        Vector2i[,] parent = new Vector2i[width, height];
        int[,] depth = new int[width, height];
        Queue<Vector2i> queue = new Queue<Vector2i>();

        queue.Enqueue(startPos);
        visited[startPos.X, startPos.Y] = true;
        depth[startPos.X, startPos.Y] = 0;

        while (queue.Count > 0)
        {
            Vector2i currentPos = queue.Dequeue();
            int currentDepth = depth[currentPos.X, currentPos.Y];

            if (currentPos == targetPos)
            {
                return ReconstructPath(parent, startPos, targetPos);
            }

            if (currentDepth >= maxDepth)
            {
                continue; 
            }

            List<Vector2i> neighbors = GetNeighbors(currentPos);

            foreach (Vector2i neighbor in neighbors)
            {
                if (!visited[neighbor.X, neighbor.Y])
                {
                    queue.Enqueue(neighbor);
                    visited[neighbor.X, neighbor.Y] = true;
                    depth[neighbor.X, neighbor.Y] = currentDepth + 1;
                    parent[neighbor.X, neighbor.Y] = currentPos;
                }
            }
        }

        return new List<Vector2i>(); 
    }


    private List<Vector2i> ReconstructPath(Vector2i[,] parent, Vector2i start, Vector2i target)
    {
        List<Vector2i> path = new List<Vector2i>();

        Vector2i current = target;
        while (current != start)
        {
            path.Add(current);
            current = parent[current.X, current.Y];
        }
        path.Reverse();

        return path;
    }

    private List<Vector2i> GetNeighbors(Vector2i position)
    {
        List<Vector2i> neighbors = new List<Vector2i>();

        AddNeighbor(position.X, position.Y - 1, neighbors); 
        AddNeighbor(position.X, position.Y + 1, neighbors);
        AddNeighbor(position.X - 1, position.Y, neighbors);
        AddNeighbor(position.X + 1, position.Y, neighbors);

        return neighbors;
    }

    private void AddNeighbor(int x, int y, List<Vector2i> neighbors)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && tiles[y][x] < Utils.OBSTACLE_TILE_INDEX)
        {
            if(blockedEnemyTileIndex != null)
            {
                if(new Vector2i(x, y) != blockedEnemyTileIndex)
                {
                    neighbors.Add(new Vector2i(x, y));
                }
            } 
            else
            {
                neighbors.Add(new Vector2i(x, y));
            }
        }
    }
}
