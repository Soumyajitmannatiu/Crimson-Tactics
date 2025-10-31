using System.Collections.Generic;
using UnityEngine;

public interface AI
{
    bool isTurnToMove();
    Tiles GetTarget();
}

public class EnemyAI : MonoBehaviour, AI
{
    public PlayerMovement player;
    public GridManager gridManager;
    public bool isturn;
    public Tiles target, curr_tile;
    public float moveSpeed;
    public Rigidbody rb;

    Tiles[] Grid = new Tiles[100];
    List<Tiles> path= new List<Tiles>();
    bool isMoving;
    int move_count = 0;
    Animator animator;    
    public bool isTurnToMove()
    {
        if (player != null)
        {
            isturn = !player.isMoving;
            return isturn;
        }
        else
            return false;       
    }
    public Tiles GetTarget()
    {
        if (player != null)
            target = player.curr_tile;
        return target;
    }
    public void GetGrid()
    {
        if (gridManager.grid==null)
        {
            Debug.Log("NULL grid");
            return;
        }
            Grid = gridManager.grid;
    }
    public void Enabler()
    {
        GetGrid();
        FindSpawn();        
        gameObject.SetActive(player.isActive);     
        animator=gameObject.GetComponent<Animator>();
    }

    void FindSpawn()
    {        
        for (int i = 0; i <Grid.Length; i++)
        {
            if (Grid[i] == null)
            {
                Debug.Log("null grid");
                return;
            }
            if (!Grid[i].isBlocked && !Grid[i] == target)
            {
                if (player != null)
                {
                    transform.position = Grid[i].WorldPosition + new Vector3(0, transform.position.y + 1, 0);
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
                return;
            }
        }
    }
    private void Update()
    {
        if (isTurnToMove() && !isMoving)
        {
            target = GetTarget();
            move_count = 0;            
            path = Pathfinding.PathFinder(curr_tile, target, Grid);
            if (path != null)
                path.Remove(path[path.Count - 1]);
        }
        AnimationControl();
    }

    private void FixedUpdate()
    {                
        if (path != null && move_count < path.Count)
        {            
            MoveAlong(path);
        }                                                  
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.CompareTag("Tiles"))
            {
                Tiles newTile = other.transform.parent.GetComponent<Tiles>();
                if (curr_tile != null)
                {
                    curr_tile.isOccupied = false;                    
                }
                curr_tile = newTile;
                curr_tile.isOccupied = true;
            }
        }
    }
    void AnimationControl()
    {
        animator.SetBool("isMoving", isMoving);
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.CompareTag("Tiles"))
            {
                curr_tile.isOccupied = true;
            }
        }
    }   
    public void MoveAlong(List<Tiles> Path)
    {
        isMoving = true;
        Tiles targetNode = Path[move_count];
        Vector3 targetPosition = targetNode.WorldPosition;
        targetPosition.y = transform.position.y;

        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        rotate(targetPosition);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.1f)
        {
            transform.position = targetPosition;
            move_count++;            

            if (move_count >= Path.Count)
            {
                Vector3 final_direc = player.gameObject.transform.position;
                rotate(final_direc);
                isMoving = false;
                Path.Clear();                
                Debug.Log("Enemy's Destination reached!");
            }
        }
    }

    void rotate(Vector3 targetPosition)
    {
        if (isMoving)
        {
            Vector3 dir = targetPosition - transform.position;
            if (dir == Vector3.zero) return;

            Quaternion new_rotation = Quaternion.LookRotation(dir, transform.up);

            Quaternion smoothedRotation = Quaternion.Lerp(rb.rotation, new_rotation, Time.deltaTime * 20);
            rb.MoveRotation(smoothedRotation); 
        }
    }
}
