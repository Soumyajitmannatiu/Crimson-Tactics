using UnityEngine;
public class GridManager : MonoBehaviour
{
    public obstaclePositions obstacleData;
    public MapGeneration mp;
    public Tiles[] grid = new Tiles[100];
    private void Start()
    {
        ApplyObstacleData();
    }
    public void ApplyObstacleData()
    {
        grid = mp.Grid;
        for (int i = 0; i < 10; i++)
        {
            grid[i].isBlocked = obstacleData.obstacle[i];
        }
    }
}
