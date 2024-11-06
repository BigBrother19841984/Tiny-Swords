using UnityEngine;
using UnityEngine.UI;


public class TopDownMovement : MonoBehaviour
{

    private float moveX;
    private float moveY;


    public bool isAttacking = false;
    public float radius;
    public LayerMask enemyLayer;
    public LayerMask sheepLayer;

    [Header("--- Essentials ---")]
    [SerializeField] private int currentHealth = 10;
    public int maxHealth = 10;
    public int damage;
    public float delay = 1f;
    public float animDuration = 1f;
    public string[] triggers;
    public string[] deathExplanation;
    public int hitCount = 0; // hit count
    public float damage_Delay = 0.5f;

    [HideInInspector]
    public int[] randomDamageAmount = { 28, 30, 32, 34, 36 };


    private Collider2D[] colliders;
    private Rigidbody2D rb;

    [Header("--- Movement ---")]
    public float moveSpeed = 5f;  // Movement speed
    public bool facingRight = true;
    private Vector2 movement;

    [Header("--- References ---")]
    public AudioSource audio;
    public AudioClip[] stepSounds;
    public AudioClip[] swordSounds;
    public Animator anim;
    public Transform swordPoint;
    public CastleScript castleScript;
    public Slider healthSlider;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetStartHealth();
    }

    void Update()
    {
        // Get the horizontal and vertical input
        if(!isAttacking && currentHealth > 0)
        {
          moveX = Input.GetAxisRaw("Horizontal");
          moveY = Input.GetAxisRaw("Vertical");

          movement = new Vector2(moveX, moveY).normalized;
        
          if (moveX != 0 || moveY != 0)
          {
            anim.SetBool("isRunning", true);
          }
          else
          {
            anim.SetBool("isRunning", false);
          }

          if(moveX < 0 && facingRight == true)
          {
             Flip();
          }
          else if(moveX > 0 && facingRight == false) 
          {
             Flip();
          }
  
          if (Input.GetKeyDown(KeyCode.Mouse0))
          {
             SwordHit();
          }
        }
       

        if(currentHealth <= 0)
        {
            //Die anim or sth...
            rb.velocity = Vector3.zero;
            anim.SetTrigger("Dead");
            Invoke("_DeathPanel", 1f);
            Debug.Log("You died!");
            Destroy(gameObject, delay);
        }
    }

    void FixedUpdate()
    {
        if(!isAttacking)
        {
            // Apply movement to the Rigidbody2D
            rb.velocity = movement * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }


    public void SwordHit()
    {
        isAttacking = true;

        colliders = Physics2D.OverlapCircleAll(swordPoint.position, radius, enemyLayer);
        Collider2D[] sheepColliders = Physics2D.OverlapCircleAll(swordPoint.position, radius, sheepLayer);

        int i = Random.Range(0, triggers.Length);
        anim.SetTrigger(triggers[i]);

        foreach (Collider2D collider in colliders)
        {
            if(hitCount == 3) // critical hit
            {
                int critAmount = CritAmount();
                collider.GetComponent<GoblinHealth>().DamageCrit(critAmount, damage_Delay);
                collider.GetComponent<GoblinHealth>().TakeDamage(critAmount);
                hitCount = 0;
            }
            else
            {
                int randomDamage = RandomDamageAmount();
                collider.GetComponent<GoblinHealth>().DamagePopUp(randomDamage, damage_Delay);
                collider.GetComponent<GoblinHealth>().TakeDamage(randomDamage);
                hitCount++;
            }
            CameraShake.instance.ShakeCamera(4f,5f,.15f);
        }

        foreach(Collider2D sheepCollider in sheepColliders)
        {
            sheepCollider.GetComponent<SheepScript>().SheepTakeDamage(RandomDamageAmount());
        }
        Invoke("StopAttack", animDuration);
    }


    private int CritAmount()
    {
        int[] crits = new int[randomDamageAmount.Length];
        for (int i = 0; i < randomDamageAmount.Length; i++)
        {
            crits[i] = (int)(randomDamageAmount[i] * 1.2);
        }
        return crits[Random.Range(0, crits.Length)];
    }
    public int RandomDamageAmount()
    {
       int r = Random.Range(0, randomDamageAmount.Length);   
       return randomDamageAmount[r];
    }

    private void StopAttack()
    {
        isAttacking = false;
    }

    public void PlayerTakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthSlider();
    }

    public void HealPlayer(int maxHealth)
    {
        currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        healthSlider.value = currentHealth;
    }

    private void SetStartHealth()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.minValue = 0;
        UpdateHealthSlider();
    }

    private void _DeathPanel()
    {
        castleScript.gameOverUI.SetActive(true);
        castleScript.GetLoseExplanationComponent().text = deathExplanation[1];
    }

    private void RandomFootStepSound()
    {
        int r = Random.Range(0, stepSounds.Length);
        audio.clip = stepSounds[r];
        audio.Play();

        CameraShake.instance.ShakeCamera(.5f, 1f, .05f);
    }

    public void PlayRandomSound() // This method is used in animation events
    {
        audio.clip = RandomSound(swordSounds);
        audio.Play();
    }

    private AudioClip RandomSound(AudioClip[] clipArray)
    {
        int random = Random.Range(0, clipArray.Length);
        return clipArray[random];
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(swordPoint.position, radius);
    }

}
