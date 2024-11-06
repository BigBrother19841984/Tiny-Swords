using UnityEngine;

public class GoblinBarrel : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public GameObject explosion;
    public Transform TargetPos;
    public GoblinHealth health;

    private GameObject castle;
    private GameObject arrow;
    private AudioSource explosionManager;


    public float delay = 0.5f;
    void Start()
    {
        anim = GetComponent<Animator>();
        castle = GameObject.FindGameObjectWithTag("Castle");
        arrow = GameObject.FindGameObjectWithTag("Arrow");

        explosionManager = GameObject.Find("ExplosionManager").GetComponent<AudioSource>();
    }
    void Update()
    {
        anim.SetBool("isRunning", true);
    }

    public void HitTheTarget()
    {
        anim.SetTrigger("Explode");
        Instantiate(explosion, TargetPos.position, Quaternion.identity);
        explosionManager.Play();
        castle.GetComponent<CastleScript>().castleHealth--;
        health.hp = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Arrow"))
        {
            int arrowDamage = health.RandomDamageAmount();
            health.PlayGoblinHitSound(health.arrowHitClips);
            health.TakeDamage(arrowDamage);
            health.DamagePopUp(arrowDamage, .5f);
            Destroy(other.gameObject);
        }
    }

}
