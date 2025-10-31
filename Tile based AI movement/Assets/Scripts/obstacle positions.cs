using UnityEngine;

[CreateAssetMenu(fileName = "ObstaclePositions", menuName = "ScriptableObjects/ObstaclePositions", order = 1)]
public class obstaclePositions : ScriptableObject
{
    public bool[] obstacle=new bool[100];    
}
