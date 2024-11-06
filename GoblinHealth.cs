using System.Collections;
using UnityEngine;
using TMPro;



public class GoblinHealth : MonoBehaviour
{

    [Header("--- Essentials ---")]
    public int hp = 1;
    public float delay = 3f;
    public float damageDelay = 1.0f;
    public float time = .5f;
    public float offset = 5f;
    public float fadeDuration = 0.1f;
    private float fadeTimer = 0f;
    public int[] arrowDamage = { 28, 30, 32, 34, 36 };

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    private Coroutine audioCoroutine;
    private bool isDead = false;
    private bool isFadingOut = false;


    [Header("References")]
    public GameObject pfDamagePopUp;
    public GameObject pfDamageCritical;
    public ParticleSystem pfBloodEffect;
    

    [Header("ScriptReferences")]
    public GoblinBarrel barrelScript;
    public EnemyAI aiScript;
    public TorchGoblinScript torchScript;
    public TorchAI torchAIscript;
    public TopDownMovement playerScript;

    [Header("--- Audio ---")]
    public AudioSource audioSource;
    public AudioClip[] arrowHitClips;
    public AudioClip[] swordHitClips;
    public AudioClip EnemyExplode;
    public float audioDuration = .5f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    // Update is called once per frame
    private void Start()
    {
        isDead = false;
        col.isTrigger = false;
    }
    void Update()
    {
        if (hp <= 0)
        {
            //StartCoroutine(PlayDeathSound());
            //StopAllCoroutines();
            if (gameObject.name == "GoblinBarrel" && barrelScript != null && aiScript != null)
            {
                if (!aiScript.canExplode) {
                    delay = 0f;
                }
                barrelScript.enabled = false;
                aiScript.enabled = false;
                Destroy(gameObject, delay);
            }
            else if (gameObject.name == "TorchGoblin" && torchScript != null && torchAIscript != null)
            {
                torchScript.enabled = false;
                torchAIscript.enabled = false;
                Destroy(gameObject, delay);
            }

            ExecuteDeathLogic();
        }

        FadeOut();
    }

    public void TakeDamage(int damageAmount)
    {
        hp -= damageAmount;
        SpawnEffects();
        PlayGoblinHitSound(swordHitClips);
    }

    private void ExecuteDeathLogic()
    {
        gameObject.layer = LayerIndex();

        anim.SetTrigger("Death"); //play dying animation
        col.isTrigger = true;

        //CameraShake.instance.ShakeCamera(5f, 0.1f);

        if (audioCoroutine == null && !isDead)
        {
            audioCoroutine = StartCoroutine(Audio());
            isDead = true;
        }
        DisableRigidBody();
        Destroy(gameObject, delay);
    }

    private GameObject popUp;
    public void DamagePopUp(int damageAmount, float delay)
    {
        pfDamagePopUp.GetComponent<TextMeshPro>().SetText(damageAmount.ToString());
        popUp = Instantiate(pfDamagePopUp, gameObject.transform.position, Quaternion.identity);
        isFadingOut = true;
        LeanTween.moveY(popUp, transform.position.y + offset , time);
        Destroy(popUp, delay);
    }

    public void DamageCrit(int damageAmount, float delay)
    {
        pfDamageCritical.GetComponent<TextMeshPro>().SetText(damageAmount.ToString());
        popUp = Instantiate(pfDamageCritical, gameObject.transform.position, Quaternion.identity);
        isFadingOut = true;
        LeanTween.moveY(popUp, transform.position.y + offset, time);
        Destroy(popUp, delay);
    }

    private void FadeOut()
    {
        if (isFadingOut && popUp != null)
        {
            fadeTimer += Time.deltaTime; // Increment timer by the time passed in each frame
            float fadeTimeNormalized = fadeTimer / fadeDuration; // Normalize the time (between 0 and 1)
            // Ensure fadeTimeNormalized stays between 0 and 1
            //fadeTimeNormalized = Mathf.Clamp01(fadeTimeNormalized);

            // Get current color of the TextMeshPro component
            Color currentColor = popUp.GetComponent<TextMeshPro>().color;

            // Lerp the alpha value from 1 (opaque) to 0 (transparent) over time
            currentColor.a = Mathf.Lerp(1f, 0.5f, fadeTimeNormalized);

            // Apply the new color with modified alpha
            popUp.GetComponent<TextMeshPro>().color = currentColor;

            Invoke("ResetFadeState", 0.1f);

            ////float minAlpha = 0.1f;
            //// Stop fading once the fade is complete
            //if (fadeTimeNormalized == 0f || popUp.GetComponent<TextMeshPro>().color.a <= minAlpha || popUp == null)
            //{
            //    fadeTimer = 0f;
            //}
        }
    }

    private void ResetFadeState() { isFadingOut = false; } // Reset fade state

    public void SpawnEffects()
    {
        ParticleSystem bloodEffect = Instantiate(pfBloodEffect, gameObject.transform.position , Quaternion.identity);
        bloodEffect.Play();
    }

    public void PlayGoblinHitSound(AudioClip[] clipArray)
    {
        audioSource.clip = SelectRandomSound(clipArray);
        audioSource.Play();
    }

    private AudioClip SelectRandomSound(AudioClip[] clipArray)
    {
        int r = Random.Range(0, clipArray.Length);
        return clipArray[r];
    }

    private void DisableRigidBody()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    private int LayerIndex()
    {
        int layerIndex = LayerMask.NameToLayer("Default");
        return layerIndex;
    }

    private void PlayDyeSound()
    {
        audioSource.clip = EnemyExplode;
        audioSource.Play();
    }

    public int RandomDamageAmount()
    {
        int r = Random.Range(0, arrowDamage.Length);
        return arrowDamage[r];
    }


    IEnumerator Audio()
    {
        PlayDyeSound();
        yield return new WaitForSeconds(audioDuration);
        audioSource.Stop();

        audioCoroutine = null;
    }
}

