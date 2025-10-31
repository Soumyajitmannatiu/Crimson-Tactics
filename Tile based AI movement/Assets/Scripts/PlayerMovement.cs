using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    public TextMeshProUGUI tileInfo;
    public Button start_but;
    public Tiles hover_tile, target_tile, curr_tile;
    public GridManager gridManager;
    public Tiles[] grid = new Tiles[100];
    public float moveSpeed;
    public int move_count;
    public bool isMoving;
    public bool isActive;
    public Rigidbody rb;
    public int RotationSmoothness;

    List<Tiles> Path = new List<Tiles>();
    Vector3 spawn_position, moveTowards;
    Animator animator;
    public void Enabler()
    {
        isActive = true;
        start_but.gameObject.SetActive(false);        
        gameObject.SetActive(isActive);
        grid = gridManager.grid;
        FindValidSpawnLocation();
        transform.position = spawn_position;
        animator = GetComponent<Animator>();
    }

    void FindValidSpawnLocation()
    {
        for (int i = 1; i < grid.Count(); i++)
        {
            if (grid[i] == null) Debug.Log("null grid");
            if (!grid[i].isBlocked)
            {
                spawn_position = grid[i].WorldPosition + new Vector3(0, transform.position.y+2, 0);
            }
        }
    }

    private void Update()
    {
        HandleRaycast();
        AnimationControl();
    }
    void HandleRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.parent == null)
            {
                return;
            }
            else if (hit.transform.parent.CompareTag("Tiles"))
            {
                int tile_name = int.Parse(hit.transform.parent.name);
                Vector2Int Tile_coor = new Vector2Int((tile_name + 10) / 10, (tile_name + 1) % 10 == 0 ? 10 : (tile_name + 1) % 10);
                hover_tile = hit.transform.parent.GetComponent<Tiles>();
                tileInfo.text = "Tile Coordinates \n" + Tile_coor;
                if (Input.GetMouseButtonDown(0) && !isMoving)
                {
                    move_count = 0;
                    target_tile = hover_tile;
                    Path = Pathfinding.PathFinder(curr_tile, target_tile, grid);
                }
            }
        }
    }
    void AnimationControl()
    {
        if (isActive)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.CompareTag("Tiles"))
            {
                curr_tile = other.transform.parent.GetComponent<Tiles>();
            }
        }        
    }
    private void FixedUpdate()
    {
        if (Path == null)
            Debug.Log("Null Path!!");
        if (Path != null && move_count < Path.Count)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        isMoving = true;
        Tiles targetNode = Path[move_count];
        Vector3 targetPosition = targetNode.WorldPosition;

        targetPosition.y = transform.position.y;
        moveTowards = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(moveTowards);

        rotate(targetPosition);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.1f)
        {
            transform.position = targetPosition;
            move_count++;

            if (move_count >= Path.Count)
            {
                isMoving = false;
                Path.Clear();
                Debug.Log("Destination reached!");
            }
        }
    }    
    void rotate(Vector3 targetPosition)
    {
        if (isMoving)
        {
            Vector3 dir = targetPosition-transform.position;
            Quaternion new_rotation = Quaternion.LookRotation(dir,Vector3.up);
            Quaternion delta_rotation = Quaternion.Lerp(transform.rotation,new_rotation,Time.deltaTime*RotationSmoothness);

            rb.MoveRotation(delta_rotation);
        }
    }
}
