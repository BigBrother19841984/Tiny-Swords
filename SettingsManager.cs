using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    private bool isInSettingsMenu = false;

    [Header("--- Animations ---")]
    public GameObject settings;
    public float duration = 0.2f;
    public LeanTweenType easeOut;
    private Coroutine closeCoroutine;

    [Header("--- Audio ---")]
    public AudioMixer audioMixer;
    public Slider sliderMusic;
    public Slider sliderSFX;
    public Image soundImage;
    public Image sfxImage;
    public Sprite muted;
    public Sprite regular; 
    public Sprite mutedSFX;
    public Sprite regularSFX;
    private const float minVolume = -80f;
    private const float maxVolume = 20;

    [Header("--- Resolution ---")]
    public Dropdown resDropDown;
    Resolution[] resolutions;
    [SerializeField] private bool isFullScreenEnabled = true;

    [Header("--- References ---")]
    public SoundManager soundManager;


    private void Start()
    {
        //audioMixer.SetFloat("volume", maxVolume);

        resolutions = Screen.resolutions;
        resDropDown.ClearOptions();

        int currentResIndex = 0;
        List<string> resOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++) 
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResIndex = i;
            }
        }

        resDropDown.AddOptions(resOptions);
        resDropDown.value = currentResIndex;
        resDropDown.RefreshShownValue();

        // Retreives audio data
       RetrieveMusicVolume();

       RetrieveSFXVolume();



    }
    private void Update()
    {
        // Check for the Escape key and only start the coroutine if it's not already running
        if (Input.GetKeyDown(KeyCode.Escape) && closeCoroutine == null && GetSceneName() == "MainMenu")
        {
            closeCoroutine = StartCoroutine(CloseCoroutine());
        }
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);

        if(volume <= minVolume)
        {
            soundImage.sprite = muted;
        }
        else
        {
            soundImage.sprite = regular;
        }
        PlayerPrefs.SetFloat("keyVolume", volume); // saves current volume
        Debug.Log("Data was saved");
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

    public void ChangeFullScreen()
    {
        
        if(isFullScreenEnabled)
        {
            isFullScreenEnabled = false;
        }
        else
        {
            isFullScreenEnabled = true;
        }

        Screen.fullScreen = isFullScreenEnabled;
    }

    // Open anims and close animations
    public void OpenSet()
    {
        settings.SetActive(true);
        LeanTween.scale(settings, new Vector3(0.9f, 0.9f, 0.9f), duration).setEase(easeOut);
        if(!isInSettingsMenu)
        {
          soundManager.PopUpSound();
        }
        isInSettingsMenu = true;
    }

    public void CloseSet()
    {
        LeanTween.scale(settings, Vector3.zero, duration).setEase(easeOut).setOnComplete(() => settings.SetActive(false));
        soundManager.PopUpClose();
        isInSettingsMenu = false;
    }

    private void RetrieveMusicVolume()
    {
        float savedMusic = PlayerPrefs.GetFloat("keyVolume", 0);
        audioMixer.SetFloat("MusicVolume", savedMusic);
        sliderMusic.value = savedMusic;
    }

    private void RetrieveSFXVolume()
    {
        float savedSFX = PlayerPrefs.GetFloat("keyVolumeSFX", 0); // retreives saved sfx data
        audioMixer.SetFloat("SFX", savedSFX);
        sliderSFX.value = savedSFX;
    }

    private string GetSceneName()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string currentSceneName = currentScene.name;
        return currentSceneName;
    }

    IEnumerator CloseCoroutine()
    {
        // Wait for the CloseSet animation to complete
        CloseSet();
        yield return new WaitForSeconds(duration);

        // After completion, reset the coroutine reference so it can be called again
        closeCoroutine = null;
    }


}

