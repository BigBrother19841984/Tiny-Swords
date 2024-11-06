using System.Collections;
using UnityEngine;

public class TorchGoblinScript : MonoBehaviour
{

    public LayerMask playerLayer;
    private GameObject player;
    private Coroutine _attackCoroutine;

    [Header("--- Damage/Time ---")]
    [SerializeField] int damage = 1;
    [SerializeField] private float range = 0.5f;
    [SerializeField] private float _timeBtwHit = 1f;
    [SerializeField] private float _timeBtwDamage = 1f;

    [Header("--- References ---")]
    public Animator anim;
    public GoblinHealth health;
    public AudioSource audioSource;
    public AudioClip[] torchSwings;
    
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();    
    }

    private void Update()
    {
        anim.SetBool("isRunning", true);
        if (_attackCoroutine == null && PlayerCollider() != null && health.hp > 0) {
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    private Collider2D PlayerCollider()
    {
       Collider2D _playerCollider = Physics2D.OverlapCircle(transform.position, range, playerLayer);
       return _playerCollider;
    }

    public void PlayTorchSound()
    {
        if(!audioSource.isPlaying)
        {
          audioSource.clip = RandomClip();
          audioSource.Play();
        }
    }

    private AudioClip RandomClip()
    {
        int r = Random.Range(0, torchSwings.Length);
        return torchSwings[r];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Arrow"))
        {
            int arrowDamage = health.RandomDamageAmount();
            health.PlayGoblinHitSound(health.arrowHitClips);
            health.TakeDamage(arrowDamage);
            health.DamagePopUp(arrowDamage, .5f);
            Destroy(other.gameObject);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        anim.SetBool("isRunning", false);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(_timeBtwDamage);
        player.GetComponent<TopDownMovement>().PlayerTakeDamage(damage);
        yield return new WaitForSeconds(_timeBtwHit);
        
        _attackCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
