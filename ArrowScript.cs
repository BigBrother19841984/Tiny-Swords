using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float arrowSpeed;
    private float range = 7f;

    private GameObject archer;
    public float delay;
    public LayerMask enemyLayer;
    private GameObject barrel;

    private void Start()
    {
        StartCoroutine(Timer());
        rb = GetComponent<Rigidbody2D>();
        archer = GameObject.FindGameObjectWithTag("Archer");

        ShootEnemyCollider();

    }

    private void ShootEnemyCollider()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(archer.transform.position, range, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            rb.velocity = direction * arrowSpeed;
            float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot);
        }
    }

    public void DestroyArrow()
    {
        Destroy(gameObject);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3f);
        DestroyArrow();
        yield break;
    }
}

