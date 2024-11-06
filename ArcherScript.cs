using UnityEngine;

public class ArcherScript : MonoBehaviour
{
    public float range;
    public LayerMask enemyLayer;
    public Transform bowPos;
    public GameObject arrowPrefab;

    private float timer = 0f;
    public float timeBtwShot = 2f;



    private void Start()
    {
        timer = 0f;
    }
    private void Update()
    {
        FindClosestEnemy();
    }


    public void Shoot()
    {
        if(timer < timeBtwShot)
        {
            timer += Time.deltaTime;
        }
        else if (timer >= timeBtwShot)
        {
            Instantiate(arrowPrefab,bowPos.position, Quaternion.identity);
            timer = 0f;
        }
    }

    private void FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        
        foreach(Collider2D enemy in enemies)
        {
            Shoot();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
