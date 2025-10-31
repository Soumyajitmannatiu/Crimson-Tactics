using System.Collections.Generic;
using UnityEngine;
public static class Pathfinding
{
    public static List<Tiles> PathFinder(Tiles start_tile, Tiles target_tile, Tiles[] grid)
    {
        if (start_tile == null || target_tile == null) 
            return null;

        bool[] visited = new bool[100];
        Queue<Tiles> queue = new Queue<Tiles>();
        Tiles curr_tile;

        ClearParentReferences(grid);

        queue.Enqueue(start_tile);
        visited[start_tile.GridPosn.x * 10 + start_tile.GridPosn.y] = true;
       
        while (queue.Count > 0)
        {
            curr_tile = queue.Dequeue();
            if (curr_tile == target_tile)
            {
                return CreatePath(start_tile, target_tile);
            }
            Tiles[] neighbors = FindAdjacentTiles(curr_tile, grid);
            foreach (Tiles t in neighbors)
            {
                if (t != null)
                {
                    if (t.isBlocked||t.isOccupied) continue;
                    if (visited[t.GridPosn.x * 10 + t.GridPosn.y]) continue;
                    visited[t.GridPosn.x * 10 + t.GridPosn.y] = true;
                    queue.Enqueue(t);
                    t.parent = curr_tile;
                }
            }
        }
        return null;
    }
    public static Tiles[] FindAdjacentTiles(Tiles tile, Tiles[] grid)
    {
        Tiles[] neighbors = new Tiles[4];
        Vector2Int min = new Vector2Int(0, 0);
        Vector2Int max = new Vector2Int(9, 9);
        for (int i = 0; i < 4; i++)
        {
            Vector2Int pos = tile.GridPosn;
            switch (i)
            {
                case 0: pos += new Vector2Int(1, 0); break;
                case 1: pos += new Vector2Int(0, 1); break;
                case 2: pos += new Vector2Int(-1, 0); break;
                case 3: pos += new Vector2Int(0, -1); break;
            }
            if (pos.x >= 0 && pos.x < 10 && pos.y >= 0 && pos.y < 10)
            {
                neighbors[i] = grid[pos.x * 10 + pos.y];
            }
        }
        return neighbors;
    }

    public static List<Tiles> CreatePath(Tiles origin, Tiles final)
    {
        List<Tiles> path = new List<Tiles>();
        Tiles curr_tile = final;       
        while (curr_tile != null)
        {
            path.Add(curr_tile);
            curr_tile = curr_tile.parent;            
        }
        path.Reverse();
        return path;
    }
    static void ClearParentReferences(Tiles[] grid)
    {
        foreach (Tiles tile in grid)
        {
            if(tile != null)
            {
                tile.parent = null;
            }
        }
    }
}