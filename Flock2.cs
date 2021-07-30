using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock2 : MonoBehaviour
{
    public List<Bot> bots;
    public List<GameObject> botsBody;
    //public int num_of_bots;
    int num_of_bots;
    public float radius;
    public float scale;
    public LayerMask unwalkableMask;
    public LayerMask targetMask;
    bool target = false;
    Vector3 targetV = Vector3.zero;

    public int xmax, ymax;
    void Start()
    {
        num_of_bots = botsBody.Count;
        Init(num_of_bots);
    }

    void FixedUpdate()
    {
        if (target)
        {
            MoveTowardsTarget();
        }
        else
        {
            MoveHead();
            MoveBoids();
        }
        UpdatePosition();
    }

    public void Init(int n)
    {
        bots = new List<Bot>();
        for(int i=0; i<n; i++)
        {
            //GameObject bot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //bot.transform.localScale = Vector3.one * scale;
            //bot.transform.position = new Vector3(Mathf.Cos(2 * Mathf.PI / n * i) * radius, 0, Mathf.Sin(2 * Mathf.PI / n * i) * radius);
            //bot.GetComponent<MeshRenderer>().material.color = Color.red;

            GameObject bot = botsBody[i];
            Vector2 position = new Vector2(Mathf.Cos(2*Mathf.PI / n * i) * radius, Mathf.Sin(2 * Mathf.PI / n * i) * radius);
            botsBody[i].transform.position = new Vector3(position.x, 0, position.y);
            botsBody[i].GetComponent<MeshRenderer>().material.color = Color.red;
            Vector2 speed = Vector2.zero;
            Bot b = new Bot(bot, speed, position);

            bots.Add(b);
        }
    }

    public void UpdatePosition()
    {
        for (int i=0; i< num_of_bots; i++)
        {
            Vector3 v = botsBody[i].transform.position - targetV;
            if (!(target && v.magnitude < 5f))
            {
                bots[i].bot.transform.position = new Vector3(bots[i].position.x, 0, bots[i].position.y);
            }
            else
            {
                bots[i].speed = Vector2.zero;
                bots[i].bot.transform.position = Vector3.MoveTowards(bots[i].bot.transform.position, new Vector3(bots[i].position.x, 0, bots[i].position.y), 10*Time.deltaTime);
            }
            //botsBody[i].transform.position = new Vector3(botsBody[i].transform.position.x, 0, botsBody[i].transform.position.z);
            //boids[i].boid.transform.position = Vector3.MoveTowards(boids[i].boid.transform.position, new Vector3(boids[i].position.x, 0, boids[i].position.y), 10*Time.deltaTime);

        }
    }

    public void MoveHead()
    {
        Vector2 v1, v2, v3, v4, v5;
        v1 = rule1(0);
        v2 = rule2(0);
        v3 = rule3(0);
        //v5 = Vector2.zero;
        v4 = obstacle(0);
        v5 = Random.insideUnitCircle;
        bots[0].speed += (- v1 - v2 - v3 + v4 + v5).normalized / 4;
        obstacle2(0);
        bots[0].position += bots[0].speed * Time.deltaTime / 3;
        Bound(0);
        //bots[0].position += bots[0].speed * Time.deltaTime / 2;
    }

    public void MoveBoids()
    {
        for(int i = 1; i < num_of_bots; i++)
        {
            Vector2 v1, v2, v3, v4, v5;
            v1 = rule1(i);
            v2 = rule2(i);
            v3 = rule3(i);
            //v4 = Vector2.zero;
            v4 = obstacle(i);
            v5 = Random.insideUnitCircle * 2;
            bots[i].speed += (v1 + v2 + v3 + v4 + v5).normalized;
            obstacle2(i);
            bots[i].position += bots[i].speed * Time.deltaTime / 2;
            Bound(i);
            bots[i].position += bots[i].speed * Time.deltaTime / 2;
        }
    }

    public void MoveTowardsTarget()
    {
        for (int i = 0; i < num_of_bots; i++)
        {
            Vector2 v1, v2, v3, v4, v5, v;
            v = new Vector2(targetV.x, targetV.z);
            v1 = rule1(i);

            Vector2 c = Vector2.zero;
            for (int j = 0; j < num_of_bots; j++)
            {
                if ((bots[j].position - bots[i].position).magnitude < 4 * scale)
                {
                    c -= (bots[j].position - bots[i].position);
                }
                if((bots[i].position - v).magnitude > (2 * scale)+2)
                {
                    c -= (bots[i].position - v);
                }
            }

            v2 = c/2;
            v3 = rule3(i);
            //v1 = (((rule1(i)*100 + bots[i].position)* (num_of_bots - 1) + v)/ (num_of_bots - 1) - bots[i].position) / 100;
            //v2 = (rule2(i)*2 - v)/2;
            //v3 = ((rule3(i) * 8 + bots[i].speed) * (num_of_bots - 1) / (num_of_bots) - bots[i].speed) / 8;
            v4 = obstacle(i);
            v5 = Random.insideUnitCircle * 2;
            bots[i].speed += (v1 + v2 + v3 + v4 + v5).normalized / 2;
            obstacle2(i);
            bots[i].position += bots[i].speed * Time.deltaTime / 2;
            Bound(i);
        }
    }

    public Vector2 rule1(int n)
    {
        Vector2 pc = Vector2.zero;
        for (int i = 0; i < num_of_bots; i++)
        {
            if (i != n)
            {
                pc += bots[i].position;
            }
        }
        pc += new Vector2(targetV.x, targetV.z);
        pc /= (num_of_bots - 1);
        return (pc - bots[n].position) / 200;
    }
    public Vector2 rule2(int n)
    {
        Vector2 c = Vector2.zero;
        for (int i = 0; i < num_of_bots; i++)
        {
            if ((bots[i].position - bots[n].position).magnitude < 3*scale)
            {
                c -= (bots[i].position - bots[n].position);
            }
        }
        return c;
    }
    public Vector2 rule3(int n)
    {
        Vector2 pv = Vector2.zero;
        for (int i = 0; i < num_of_bots; i++)
        {
            if (i != n)
            {
                pv += bots[i].speed;
            }
        }
        if(target) pv /= (num_of_bots);
        else pv /= (num_of_bots - 1);
        return (pv - bots[n].speed) / 8;
    }
    public Vector2 obstacle(int n)
    {
        Vector3 v = Vector3.zero;
        int numberofRays = 15;
        float angle = 150;
        for (int i = 0; i < numberofRays; ++i)
        {
            var rotation = botsBody[n].transform.rotation;
            var rotationMod = Quaternion.AngleAxis((float)(2 * angle / (numberofRays - 1) * i), new Vector3(0, 1, 0));
            var direction = rotation * rotationMod * new Vector3(1, 0, 0);

            if (Physics.Raycast(botsBody[n].transform.position, direction, out RaycastHit hit, 2, unwalkableMask))
            {
                //rb.transform.position -= direction / hit.distance * Time.deltaTime;
                //v -= (hit.point - boidsBody[n].transform.position)  * Time.deltaTime;
                v -= direction.normalized / hit.distance * Time.deltaTime;
            }
            else if (Physics.Raycast(botsBody[n].transform.position, direction, out hit, 5, targetMask))
            {
                //rb.transform.position -= direction / hit.distance * Time.deltaTime;
                //v -= (hit.point - boidsBody[n].transform.position)  * Time.deltaTime;
                v -= direction.normalized / (hit.distance + 1) * Time.deltaTime;
                target = true;
                targetV = hit.point;
            }
        }
        v = v.normalized;
        return new Vector2(v.x, v.z) * 10;
    }
    public void obstacle2(int n)
    {
        Vector3 v = Vector3.zero;
        int numberofRays = 15;
        float angle = 150;
        int flag = 0;
        for (int i = 0; i < numberofRays; ++i)
        {
            var rotation = botsBody[n].transform.rotation;
            var rotationMod = Quaternion.AngleAxis((float)(2 * angle / (numberofRays - 1) * i), new Vector3(0, 1, 0));
            var direction = rotation * rotationMod * new Vector3(1, 0, 0);

            if (Physics.Raycast(botsBody[n].transform.position, direction, out RaycastHit hit, 5, unwalkableMask))
            {
                //rb.transform.position -= direction / hit.distance * Time.deltaTime;
                //v -= (hit.point - boidsBody[n].transform.position)  * Time.deltaTime;
                flag++;
                v -= direction.normalized / hit.distance * Time.deltaTime;
            }
            else if (Physics.Raycast(botsBody[n].transform.position, direction, out hit, 10, targetMask))
            {
                //rb.transform.position -= direction / hit.distance * Time.deltaTime;
                //v -= (hit.point - boidsBody[n].transform.position)  * Time.deltaTime;
                //v -= direction.normalized / (hit.distance + 1) * Time.deltaTime;
                target = true;
                if (targetV.magnitude != 0) targetV = (targetV + hit.point) / 2;
                else targetV = hit.point;
            }
        }
        v = v.normalized;
        if(flag>3) bots[n].speed = new Vector2(v.x, v.z) * 10;
        else bots[n].speed += new Vector2(v.x, v.z) * 10;
        //return new Vector2(v.x, v.z) * 10;
    }

    public void Bound(int n)
    {
        int Xmax = xmax, Xmin = -xmax, Ymax = ymax, Ymin = -ymax;
        int c = 4;
        if(bots[n].position.x < Xmin)
        {
            bots[n].speed.x = c;
        }
        else if (bots[n].position.x > Xmax)
        {
            bots[n].speed.x = -c;
        }
        else if (bots[n].position.y < Ymin)
        {
            bots[n].speed.y = c;
        }
        else if (bots[n].position.y > Ymax)
        {
            bots[n].speed.y = -c;
        }
    }
}
