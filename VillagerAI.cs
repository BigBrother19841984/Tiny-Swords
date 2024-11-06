using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerAI : MonoBehaviour
{
    [Header("--- Audio ---")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stepSound;

    [Header("--- References ---")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject targetPos;
    [SerializeField] private LayerMask shelterLayer;
    private GameManager gameManager;

    [Header("--- Essentials ---")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distanceThreshold = 1.0f;
    [SerializeField] private float radius = 5f;
    [HideInInspector] public Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        gameManager = FindAnyObjectByType<GameManager>();
    }


    private void Update()
    {
        if (gameManager == null)
        {
            Debug.LogError("Game Manager is not found");
            return;
        }

        if (gameManager != null && gameManager.isInWave) 
        {
            MoveTo(targetPos.transform.position);
        }
        else
        {
            MoveTo(startPos);
        }
    }

    private void MoveTo(Vector2 target)
    {
        anim.SetBool("isRunning", true);
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.velocity = force;

        float distance = Vector2.Distance(transform.position, target);

        if (distance <= distanceThreshold)
        {
            //Debug.Log("Target is reached");
            anim.SetBool("isRunning", false);
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Castle") && gameManager.isInWave)
        {
            gameObject.SetActive(false);
        }
    }

    /*private Collider2D Shelter()
    {
        Collider2D shelterCollider = Physics2D.OverlapCircle(transform.position, radius, shelterLayer);
        bool canBeEntered = false;

        if (shelterCollider == null)
            Debug.LogError("Collider is not found");

        if(shelterCollider.CompareTag("Castle"))
        {
            canBeEntered = true;
        }
        else
        {
            canBeEntered = shelterCollider.GetComponent<BuildScript>().isBuilt;
        }

        if (shelterCollider != null && shelterCollider.tag != "Tree" && canBeEntered)
         return shelterCollider;

        else return null;
    }*/




    public void PlayStepSound()
    {
        if(audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = stepSound;
            audioSource.pitch = RandomPitch();
            audioSource.Play();
        }
    }

    private float RandomPitch()
    {
        float randomPitch = Random.Range(0.9f, 1.5f);
        return randomPitch;
    }
}
