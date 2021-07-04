using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DungeonType { Caverns, Rooms, Winding }

public class DungeonManager : MonoBehaviour
{

    public GameObject[] randomItems, randomEnemies, roundedEdges;
    public GameObject floorPrefab, wallPrefab, tilePrefab, exitPrefab;
    [Range(50, 5000)] public int totalFloorCount;
    [Range(0, 100)] public int itemSpawnPercentage;
    [Range(0, 100)] public int enemySpawnPercentage;
    [Range(0, 100)] public int windingHallPercentage;
    
    public bool useRoundedEdges;
    public DungeonType dungeontype;
    Vector2 hitSize;

    [HideInInspector] public float minX, maxX, minY, maxY;

    List<Vector3> floorList = new List<Vector3>();
    LayerMask floorMask, wallMask;

    void Start()
    {
        floorMask = LayerMask.GetMask("Floor");
        wallMask = LayerMask.GetMask("Wall");
        hitSize = Vector2.one * 0.8f;
        switch (dungeontype)
        {
            case DungeonType.Caverns:
                RandomWalker();
                break;
            case DungeonType.Rooms:
                RoomWalker();
                break;
            case DungeonType.Winding:
                WindingWalker();
                break;
        }
    }

    void Update()
    {
        //reload scene inside unity
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void RandomWalker()
    {
        Vector3 curPos = Vector3.zero;
        //set floor tile at curPos
        floorList.Add(curPos);

        while (floorList.Count < totalFloorCount)
        {
            curPos += RandomDirection();
            if (!InFloorList(curPos))
            {
                floorList.Add(curPos);
            }
        }
        StartCoroutine(DelayProgress());
    }

    void RoomWalker()
    {
        Vector3 curPos = Vector3.zero;
        floorList.Add(curPos);

        while (floorList.Count < totalFloorCount)
        {
            curPos = GeneratePaths(curPos);
            RandomRoom(curPos);
        }
        StartCoroutine(DelayProgress());
    }

    void WindingWalker()
    {
        Vector3 curPos = Vector3.zero;
        floorList.Add(curPos);

        while (floorList.Count < totalFloorCount)
        {
            curPos = GeneratePaths(curPos);
            int roll = UnityEngine.Random.Range(0, 100);
            if(roll > windingHallPercentage)
            {
                RandomRoom(curPos);
            }
        }
        StartCoroutine(DelayProgress());
    }

    Vector3 GeneratePaths(Vector3 myPos)
    {
        //set direction to walk and amount of tiles before  newdirection
        Vector3 walkDir = RandomDirection();
        int walklength = UnityEngine.Random.Range(9, 18);
        for (int i = 0; i < walklength; i++)
        {
            if (!InFloorList(myPos + walkDir))
            {
                floorList.Add(myPos + walkDir);
            }
            myPos += walkDir;
        }
        return myPos;
    }

    void RandomRoom(Vector3 myPos)
    {
        //random room at the end of the a walk
        int width = UnityEngine.Random.Range(1, 5);
        int height = UnityEngine.Random.Range(1, 5);
        for (int w = -width; w <= width; w++)
        {
            for (int h = -height; h <= height; h++)
            {
                Vector3 offset = new Vector3(w, h, 0);
                if (!InFloorList(myPos + offset))
                {
                    floorList.Add(myPos + offset);
                }
            }
        }
    }

    bool InFloorList(Vector3 myPos)
    {
        //choose a new position and add it to the list and check if its already added

        for (int i = 0; i < floorList.Count; ++i)
        {
            if (Vector3.Equals(myPos, floorList[i]))
            {
                return true;
            }
        }
        return false;
    }
    Vector3 RandomDirection()
    {
        switch (UnityEngine.Random.Range(1, 5))
        {
            case 1: return Vector3.up;
            case 2: return Vector3.right;
            case 3: return Vector3.down;
            case 4: return Vector3.left;
        }
        return Vector3.zero;
    }

    IEnumerator DelayProgress()
    {
        for (int i = 0; i < floorList.Count; ++i)
        {
            GameObject goTile = Instantiate(tilePrefab, floorList[i], Quaternion.identity);
            goTile.name = tilePrefab.name;
            goTile.transform.SetParent(transform);
        }
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }
        ExitDoorway();
        Vector2 hitSize = Vector2.one * 0.8f;
        for (int x = (int)minX - 2; x <= (int)maxX + 2; ++x)
        {
            for (int y = (int)minY - 2; y <= (int)maxY + 2; ++y)
            {
                Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(x, y), hitSize, 0, floorMask);
                if (hitFloor)
                {
                    //if not in the same position as lasst item on the floor list
                    if (!Vector2.Equals(hitFloor.transform.position, floorList[floorList.Count - 1]))
                    {
                        Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, wallMask);
                        Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, wallMask);
                        Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, wallMask);
                        Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, wallMask);
                        RandomItems(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                        RandomEnemies(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                    }
                }
                //check for wall collision and add round edges.
                RoundedEdges(x, y);
            }
        }
    }

    void RoundedEdges(int x, int y)
    {
        if (useRoundedEdges)
        {
            Collider2D hitWall = Physics2D.OverlapBox(new Vector2(x, y), hitSize, 0, wallMask);
            if (hitWall)
            {
                Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, wallMask);
                Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, wallMask);
                Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, wallMask);
                Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, wallMask);
                int bitVal = 0;
                if (!hitTop)
                {
                    bitVal += 1;
                }
                if (!hitRight)
                {
                    bitVal += 2;
                }
                if (!hitBottom)
                {
                    bitVal += 4;
                }
                if (!hitLeft)
                {
                    bitVal += 8;
                }
                if (bitVal > 0)
                {
                    GameObject goEdge = Instantiate(roundedEdges[bitVal], new Vector2(x, y), Quaternion.identity);
                    goEdge.name = roundedEdges[bitVal].name;
                    goEdge.transform.SetParent(hitWall.transform);
                }
            }
        }
    }

    void RandomEnemies(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft)
    {
        //makes enemies spawn in open spaces
        if (!hitTop && !hitRight && !hitBottom && !hitLeft)
        {
            int roll = UnityEngine.Random.Range(1, 101);
            if (roll <= enemySpawnPercentage)
            {
                int enemyIndex = UnityEngine.Random.Range(0, randomEnemies.Length);
                GameObject goEnemy = Instantiate(randomEnemies[enemyIndex], hitFloor.transform.position, Quaternion.identity);
                goEnemy.name = randomEnemies[enemyIndex].name;
                goEnemy.transform.SetParent(hitFloor.transform);
            }
        }
    }

    void RandomItems(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft)
    {
        //put items against walls
        if ((hitTop || hitRight || hitBottom || hitLeft) && !(hitTop && hitBottom) && !(hitLeft && hitRight))
        {
            int roll = UnityEngine.Random.Range(1, 101);
            if (roll <= itemSpawnPercentage)
            {
                int itemIndex = UnityEngine.Random.Range(0, randomItems.Length);
                GameObject goItems = Instantiate(randomItems[itemIndex], hitFloor.transform.position, Quaternion.identity);
                goItems.name = randomItems[itemIndex].name;
                goItems.transform.SetParent(hitFloor.transform);
            }
        }
    }

    void ExitDoorway()
    {
        Vector3 doorPosition = floorList[floorList.Count - 1];
        GameObject goDoor = Instantiate(exitPrefab, doorPosition, Quaternion.identity);
        goDoor.name = exitPrefab.name;
        goDoor.transform.SetParent(transform);
    }
}

