using UnityEngine;
public class Tiles : MonoBehaviour
{
    public Tiles parent;
    public Vector2Int GridPosn;
    public bool isBlocked = false;
    public bool isOccupied = false;
    public Vector3 WorldPosition => transform.position;
    public obstaclePositions op;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent.CompareTag("Player"))
        {
            this.isOccupied = true;
        }
    }
    private void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var index = GridPosn.x * 10 + GridPosn.y;
            if (!op.obstacle[index] && child.name == "Obstacle(Clone)")
            {
                this.isBlocked = false;
                Destroy(child.gameObject);
            }
        }
    }
}
