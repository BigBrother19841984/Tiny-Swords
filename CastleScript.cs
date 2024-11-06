using UnityEngine;
using UnityEngine.UI;

public class CastleScript : MonoBehaviour
{
    public int castleHealth = 3;
    public SpriteRenderer _rend;
    public GameObject destroyedCastle;
    private bool canShake = true;

    [Header("--- References ---")]
    public GameObject gameOverUI;
    public TopDownMovement player;
    [SerializeField] private GameObject goblinSpawner;
    public AudioSource[] audioSources;
    public AudioSource themeMusic;

    private void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(castleHealth <= 0 && canShake)
        {
            _rend.enabled = false;
            destroyedCastle.SetActive(true);
            CameraShake.instance.ShakeCamera(7f, 10f, 2f);
            DisableCharacters();
            Invoke("GameOver", 1f);
            canShake = false;
        }
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        goblinSpawner.SetActive(false);
        GetLoseExplanationComponent().text = player.deathExplanation[0];
        themeMusic.Stop();
        foreach(AudioSource everySource in AllPlayingAudioSources())
        {
            if(everySource.isPlaying)
            {
                everySource.Stop();
            }
        }
        foreach(AudioSource source in audioSources)
        {
            source.Play();
        }
    }

    public Text GetLoseExplanationComponent()
    {
        GameObject explanation = GameObject.FindGameObjectWithTag("Explanation");
        Text WhyLoseText = explanation.GetComponent<Text>();
        return WhyLoseText;
    }

    private void DisableCharacters()
    {
        GameObject[] goblins = GameObject.FindGameObjectsWithTag("Goblin");
        player.enabled = false;
        foreach(GameObject goblin in goblins)
        {
            goblin.SetActive(false);
        }
    }

    private AudioSource[] AllPlayingAudioSources()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        return allAudioSources;
    }

}
