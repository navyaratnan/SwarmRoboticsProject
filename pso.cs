using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pso : MonoBehaviour
{
    public float maxX = 90;
    public float maxY = 90;
    public float cell = 1;
    public Grid grid;
    public List<Bot> bots;
    public List<GameObject> botsBody;
    int X, Y;
    public LayerMask unwalkableMask;

    void Start()
    {
        grid = new Grid(maxX, maxY, cell);
        
        X = Mathf.RoundToInt(maxX / cell) - 1;
        Y = Mathf.RoundToInt(maxY / cell) - 1;
        grid.grid = new Cell[X,Y];
        for (int i=0; i<X; i++)
        {
            for(int j=0; j<Y; j++)
            {
                grid.grid[i, j] = new Cell(false, false);
            }
        }

        bots = new List<Bot>();
        for (int i = 0; i < botsBody.Count; i++)
        {
            GameObject bot = botsBody[i];
            Vector2 position = grid.Coordinate(botsBody[i].transform.position);
            botsBody[i].transform.position = new Vector3(position.x, 0, position.y);
            botsBody[i].GetComponent<MeshRenderer>().material.color = Color.red;
            Vector2 speed = Vector2.zero;
            Bot b = new Bot(bot, speed, position);

            bots.Add(b);
        }
    }

    void FixedUpdate()
    {
        for(int i=0; i<botsBody.Count; i++)
        {
            Vector3 pos = botsBody[i].transform.position;
            Vector2 coor = grid.Coordinate(pos);
            
            if(grid.grid[Mathf.RoundToInt(coor.x), Mathf.RoundToInt(coor.y)].explored)
            {
                for (int n = -1; n < 2; n++)
                {
                    for (int m = -1; m < 2; m++)
                    {
                        if(Mathf.RoundToInt(coor.x + n) > X || Mathf.RoundToInt(coor.x + n) < 0 || Mathf.RoundToInt(coor.y + m) > Y || Mathf.RoundToInt(coor.y + m) < 0)
                        {
                            continue;
                        }
                        if(grid.grid[Mathf.RoundToInt(coor.x + n), Mathf.RoundToInt(coor.y + m)].explored == false)
                        {
                            botsBody[i].transform.position = Vector3.MoveTowards(botsBody[i].transform.position, new Vector3(botsBody[i].transform.position.x + n, 0, botsBody[i].transform.position.z + m), 10 * Time.deltaTime);
                            break;
                        }
                    }
                }
            }
            pos = botsBody[i].transform.position;
            coor = grid.Coordinate(pos);

            grid.grid[Mathf.RoundToInt(coor.x), Mathf.RoundToInt(coor.y)].explored = true;
            for (int n = -1; n < 2; n++)
            {
                for (int m = -1; m < 2; m++)
                {
                    grid.grid[Mathf.RoundToInt(coor.x + n), Mathf.RoundToInt(coor.y + m)].explored = true;
                    grid.grid[Mathf.RoundToInt(coor.x), Mathf.RoundToInt(coor.y)].occ = false;
                }
            }

            int numberofRays = 8;
            float angle = 150;
            for (int j = 0; j < numberofRays; ++j)
            {
                var rotation = botsBody[i].transform.rotation;
                var rotationMod = Quaternion.AngleAxis((float)(2 * angle / (numberofRays - 1) * j), new Vector3(0, 1, 0));
                var direction = rotation * rotationMod * new Vector3(1, 0, 0);

                if (Physics.Raycast(botsBody[i].transform.position, direction, out RaycastHit hit, 4, unwalkableMask))
                {
                    Vector2 v1 = grid.Coordinate(hit.point);
                    //grid.grid[Mathf.RoundToInt(v1.x), Mathf.RoundToInt(v1.y)].occ = true;
                    for (int n = -1; n < 2; n++)
                    {
                        for (int m = -1; m < 2; m++)
                        {
                            grid.grid[Mathf.RoundToInt(v1.x + n), Mathf.RoundToInt(v1.y + m)].occ = true;
                        }
                    }
                }
                /*else if (Physics.Raycast(botsBody[n].transform.position, direction, out hit, 10, targetMask))
                {
                    target = true;
                    targetV = hit.point;
                }*/
                else
                {
                    //grid.grid[Mathf.RoundToInt(coor.x), Mathf.RoundToInt(coor.y)].occ = false;
                }
            }
        


        }
    }

    public void targetSelection()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(maxX,0,maxY));
        if (bots != null)
        {
            //int i = 0;
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    Debug.Log(i + "," + j);
                    Cell c = grid.grid[i, j];
                    if (c.occ) Gizmos.color = Color.black;
                    else if (c.explored) Gizmos.color = Color.red;
                    else Gizmos.color = Color.white;
                    Vector3 n = new Vector3(i + 0.5f, 0, j + 0.5f)*cell - new Vector3(maxX/2, 0, maxY/2);
                    Gizmos.DrawCube(n, Vector3.one * (1 - .1f)* cell);
                    //Gizmos.DrawWireCube(n, Vector3.one * (1 - .1f));
                }
            }
        }
    }
}
