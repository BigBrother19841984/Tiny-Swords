using UnityEngine;
using Pathfinding;
using System.Net.Mime;

public class TorchAI : MonoBehaviour
{
    private GameObject target;
    public float speed = 50f;
    public float nextWayPoint = 3f;


    public Seeker seeker;
    public Rigidbody2D rb;
    private Path path;
    private int currentWayPoint = 0;
    private bool reachedEndOfPath = false;

    public float distanceThreshold = 1.5f;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        target = GameObject.FindGameObjectWithTag("Player");

        InvokeRepeating("UpdatePath", 0f, 0.5f);

    }

    void UpdatePath()
    {
        if (seeker.IsDone() && target != null)
        {
            seeker.StartPath(rb.position, target.GetComponent<Transform>().position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * 20 * Time.fixedDeltaTime;

        rb.AddForce(force);

        if(target != null)
        {
           float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
           float distanceFromTarget = Vector2.Distance(transform.position, target.transform.position);

           if (distance < nextWayPoint)
           {
              currentWayPoint++;
           }
           else if(distanceFromTarget < distanceThreshold)
           {
              speed *= 1.5f;
           }
        }

        
    }
}