using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int waveIndex = 0;
    public bool isInWave = false;
    public bool isTutorial = false;
    public bool isGamePaused = false;
    private GoblinSpawner g;
    private Tween tween;
    private TopDownMovement player;


    [Header("--- Time ---")]
    public float timer = 0f;
    public float waveDuration = 30f;
    public float increaseIndex = 2f;
    public float decreaseSpawnTime = 0.1f;
    public float duration = 3f;


    [Header("--- References ---")]
    public Text waveUI;
    public GameObject goblinSpawner;
    public GameObject pauseMenu;
    public SoundManager soundManager;
    private Coroutine pauseCoroutine;
    private VillagerAI[] villagers;

    [Header("--- Audio ---")]
    public AudioMixer audioMixer;
    public AudioSource audioSource;
    public AudioClip pyatkaBobra;
    public AudioClip hornSound;
    public Image sourceImage;
    public Image sfxImage;
    public Sprite regular;
    public Sprite muted;
    public Sprite regularSFX;
    public Sprite mutedSFX;
    public Slider sliderUI;
    public Slider sliderSFX;
    private const float minVolume = -80f;

    private void Start()
    {
         audioSource = GetComponent<AudioSource>();
         tween = GameObject.FindGameObjectWithTag("WaveRibbon").GetComponent<Tween>();

         player = GameObject.FindGameObjectWithTag("Player").GetComponent<TopDownMovement>();

         villagers = Resources.FindObjectsOfTypeAll<VillagerAI>();

        float savedVolume = PlayerPrefs.GetFloat("keyVolume", 0); // retrieves saved music data
        audioMixer.SetFloat("MusicVolume", savedVolume);
        sliderUI.value = savedVolume;

        float savedSFX = PlayerPrefs.GetFloat("keyVolumeSFX", 0); // retreives saved sfx data
        audioMixer.SetFloat("SFX", savedSFX);
        sliderSFX.value = savedSFX;

    }

    private void Update()
    {
        if(isInWave)
        {
           if(timer < waveDuration)
           {
              timer += Time.deltaTime;
           }
           else if(timer >= waveDuration)
           {
               EndWave();
           }
        }

        if (waveIndex == 0)
        {
            isTutorial = true;
        }
        else
        {
            isTutorial = false;
        }


        if (Input.GetKeyDown(KeyCode.Space) && !isInWave)
        {
            StartNextWave();
        }

        if(pauseCoroutine == null)
        {
            pauseCoroutine = StartCoroutine(PauseCoroutine());
        }

    }

    private void StartNextWave()
    {
        g = goblinSpawner.GetComponent<GoblinSpawner>();
        isInWave = true;

        if (!audioSource.isPlaying)
        {
            PlaySound(hornSound, 1f);
        }

        waveIndex++;
        waveUI.text = "Wave: " + waveIndex;
        goblinSpawner.SetActive(true);
        waveDuration += increaseIndex;

        if(tween != null)
        {
          tween.LiftWaveRibbon();
        }
        player.HealPlayer(player.maxHealth);
        
        if(g != null )
        {
           g.spawnTime -= decreaseSpawnTime;
        }

        if(waveIndex == 5 || waveIndex == 15 || waveIndex == 20)
        {
            IncreaseSpawnRate();
            decreaseSpawnTime -= 0.025f;
            decreaseSpawnTime = Mathf.Clamp(decreaseSpawnTime, 0f, 1f);
        }

    }

    private void EndWave()
    {
        if(!audioSource.isPlaying)
        {
            PlaySound(pyatkaBobra, 1f);
        }

        tween.ReturnRibbonToStartPos();
        goblinSpawner.SetActive(false);
        isInWave = false;
        timer = 0f;

        EnableVillagers();
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.Play();
    }

    private void EnableVillagers()
    {
        foreach (VillagerAI villager in villagers)
        {
            if (!villager.gameObject.activeSelf)
            {
                villager.gameObject.SetActive(true);
            }
        }
    }

    private void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        LeanTween.scale(pauseMenu, Vector3.one, duration).setEase(LeanTweenType.easeInOutExpo);
        soundManager.PopUpSound();
    }

    private void CloseMenu()
    {
        LeanTween.scale(pauseMenu, Vector3.zero, duration).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() => pauseMenu.SetActive(false));
        soundManager.PopUpClose();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);

        if (volume <= minVolume)
        {
            sourceImage.sprite = muted;
        }
        else
        {
            sourceImage.sprite = regular;
        }
        PlayerPrefs.SetFloat("keyVolume", volume);
    }

    public void SetSFX(float SFXvolume)
    {
        audioMixer.SetFloat("SFX", SFXvolume);

        if (SFXvolume <= minVolume)
        {
            sfxImage.sprite = mutedSFX;
        }
        else
        {
            sfxImage.sprite = regularSFX;
        }
        PlayerPrefs.SetFloat("keyVolumeSFX", SFXvolume); // saves current volume
    }

    private void IncreaseSpawnRate()
    {
        g.goblinSpawnAmount++;
    }

    IEnumerator PauseCoroutine()
    {
        
            if (Input.GetKeyDown(KeyCode.Escape)) 
            { 
                if(!isGamePaused)
                {
                    OpenPauseMenu();
                    yield return new WaitForSeconds(duration + 0.1f);
                    Time.timeScale = 0f;
                    isGamePaused = true;
                }
                else
                {
                    Time.timeScale = 1f;
                    isGamePaused = false;
                    CloseMenu();
                }
            }
        pauseCoroutine = null;
    }

}
