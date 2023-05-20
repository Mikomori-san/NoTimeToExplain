using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

public class BreadthFirstSearch
{
    private int width;
    private int height;
    private List<int[]> tiles;

    public BreadthFirstSearch(int width, int height, List<int[]> map)
    {
        this.width = width;
        this.height = height;
        tiles = map;
    }

    public List<Vector2i> FindPath(Vector2i startPos, Vector2i targetPos)
    {
        bool[,] visited = new bool[width, height];

        Vector2i[,] parent = new Vector2i[width, height];

        Queue<Vector2i> queue = new Queue<Vector2i>();

        queue.Enqueue(startPos);
        visited[startPos.X, startPos.Y] = true;

        while (queue.Count > 0)
        {
            Vector2i currentPos = queue.Dequeue();

            if (currentPos == targetPos)
            {
                return ReconstructPath(parent, startPos, targetPos);
            }

            List<Vector2i> neighbors = GetNeighbors(currentPos);

            foreach (Vector2i neighbor in neighbors)
            {
                if (!visited[neighbor.X, neighbor.Y])
                {
                    queue.Enqueue(neighbor);
                    visited[neighbor.X, neighbor.Y] = true;

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
            neighbors.Add(new Vector2i(x, y));
        }
    }
}
