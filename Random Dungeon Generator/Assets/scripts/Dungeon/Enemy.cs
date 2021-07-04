using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public float chaseSpeed;
    public float alertRange;
    public Vector2 PatrolInterval;
    Vector2 curPos;
    Player player;
    LayerMask obstacleMask, walkableMask;
    bool isMoving;

    List<Vector2> availableMovementList = new List<Vector2>();
    List<Node> nodesList = new List<Node>();

    void Start()
    {
        player = FindObjectOfType<Player>();
        obstacleMask = LayerMask.GetMask("Wall", "Enemy", "Player");
        walkableMask = LayerMask.GetMask("Wall", "Enemy");
        curPos = transform.position;
        StartCoroutine(Movement());
    }

    void Patrol()
    {
        //checking list for potential movements
        availableMovementList.Clear();
        Vector2 size = Vector2.one * 0.8f;
        Collider2D hitUp = Physics2D.OverlapBox(curPos + Vector2.up, size, 0, obstacleMask);
        if (!hitUp)
        {
            availableMovementList.Add(Vector2.up);
        }

        Collider2D hitRight = Physics2D.OverlapBox(curPos + Vector2.right, size, 0, obstacleMask);
        if (!hitRight)
        {
            availableMovementList.Add(Vector2.right);
        }

        Collider2D hitDown = Physics2D.OverlapBox(curPos + Vector2.down, size, 0, obstacleMask);
        if (!hitDown)
        {
            availableMovementList.Add(Vector2.down);
        }

        Collider2D hitLeft = Physics2D.OverlapBox(curPos + Vector2.left, size, 0, obstacleMask);
        if (!hitLeft)
        {
            availableMovementList.Add(Vector2.left);
        }
        if (availableMovementList.Count > 0)
        {
            int randomIndex = Random.Range(0, availableMovementList.Count);
            //adding availablelist to our curPos.
            curPos += availableMovementList[randomIndex];
        }
        StartCoroutine(EnemyMovement(Random.Range(PatrolInterval.x, PatrolInterval.y)));
    }

    void Attack()
    {
        int roll = Random.Range(0, 100);
        if(roll > 50)
        {
            Debug.Log("attacked");
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log(" attacked and missed");
        }
    }
    IEnumerator EnemyMovement(float speed)
    {
        isMoving = true;
        while (Vector2.Distance(transform.position, curPos) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, curPos, 5f * Time.deltaTime);
            yield return null;
        }
        transform.position = curPos;
        yield return new WaitForSeconds(speed);
        isMoving = false;
    }

    void CheckNode(Vector2 chkPoint, Vector2 parent) 
    {
        Vector2 size = Vector2.one * 0.5f;
        Collider2D hit = Physics2D.OverlapBox(chkPoint, size, 0, walkableMask);
        if (!hit)
        {
            nodesList.Add(new Node(chkPoint, parent));
        }
    }

    Vector2 FindNextStep(Vector2 startPos, Vector2 targetPos)
    {
        int listIndex = 0;
        Vector2 myPos = startPos;
        nodesList.Clear();
        nodesList.Add(new Node(startPos, startPos));
        while (myPos != targetPos && listIndex < 1000 && nodesList.Count > 0)
        {
            //checking up,down,left & right (if tiles are walkable add to the list)
            CheckNode(myPos + Vector2.up, myPos);
            CheckNode(myPos + Vector2.right, myPos);
            CheckNode(myPos + Vector2.down, myPos);
            CheckNode(myPos + Vector2.left, myPos);
            //afer check increment the listIndex
            listIndex++;
            if(listIndex < nodesList.Count)
            {
                //update myPos position
                myPos = nodesList[listIndex].position;
            }
        }
        if(myPos == targetPos)
        {
            nodesList.Reverse(); // move backwards through the nodes list
            for(int i = 0; i < nodesList.Count; i++)
            {
                if(myPos == nodesList[i].position)
                {
                    if(nodesList[i].parent == startPos)
                    {
                        return myPos;
                    }
                    myPos = nodesList[i].parent;
                }
            }
        }
        return startPos;
    }
    IEnumerator Movement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (!isMoving)
            {
                //move to player
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance <= alertRange)
                {
                    if (distance <= 1.1f)
                    {
                        Attack();
                        yield return new WaitForSeconds(Random.Range(0.5f, 1.15f));
                    }
                    else
                    {
                        //check to see of there is a clearpath between enemy and player
                        Vector2 newPos = FindNextStep(transform.position, player.transform.position);
                        if(newPos != curPos)
                        {
                            //chase
                            curPos = newPos;
                            StartCoroutine(EnemyMovement(chaseSpeed));
                        }
                        else
                        {
                            Patrol();
                        }
                    }
                }
                else
                {
                    Patrol();
                }
            }
        }
    }
}

