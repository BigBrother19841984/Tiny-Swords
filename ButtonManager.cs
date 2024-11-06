using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("References")]
    public SoundManager soundManager;

    public void PlayButton()
    {
        soundManager.PlaySelectSound();
        SceneManager.LoadScene("GameLevel");
    }

    public void MenuButton()
    {
        UnPause();
        soundManager.PlaySelectSound();
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        UnPause();
        soundManager.PlaySelectSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitButton() 
    {
        soundManager.PlaySelectSound();
        Application.Quit();
    }

    private void UnPause()
    {
        Time.timeScale = 1.0f;
    }
}
