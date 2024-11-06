using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 50f;
    public float nextWayPoint = 3f;


    private Seeker seeker;
    public Rigidbody2D rb;
    private Path path;
    private int currentWayPoint = 0;
    private bool reachedEndOfPath = false;

    public float distanceThreshold = 1.5f;
    public float thresholdBeforeSpeedUp;

    public GoblinBarrel goblinScript;
    public bool canExplode = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
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
        Vector2 force = direction * speed * Time.fixedDeltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPoint)
        {
            currentWayPoint++;
        }

        float distanceBtwCastle = Vector2.Distance(rb.position, target.position);

        if (distanceBtwCastle < distanceThreshold && canExplode)
        {
            goblinScript.HitTheTarget();
            canExplode = false;
        }
        else if(distanceBtwCastle < thresholdBeforeSpeedUp)
        {
            speed *= 1.2f;
        }

    }
}

