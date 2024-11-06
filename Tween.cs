using UnityEngine;
using UnityEngine.UI;

public class Tween : MonoBehaviour
{
    public LeanTweenType easeIn;
    public LeanTweenType easeOut;
    public float duration = 1f;
    public float closeDelay = 2f;
    public float offsetX;
    public float offsetY;

    public string[] tutorialBanners = { "Not during the wave", "B to build" };

    [Header("References")]
    public Text tutorialText;
    public GameManager gm;
    public GameObject moneyPanel;
    //[HideInInspector] public Vector3 startPos;

    private void Start()
    {
        // Initialize LeanTween with a max tween limit (optional)
        LeanTween.init(10000);
    }

    public void SidePopUp(string tutorial, float duration, int fontSize)
    {
        if (gameObject.CompareTag("Tutorial"))
        {
            gameObject.SetActive(true);

            // Check if tutorialText is assigned
            if (tutorialText != null)
            {
                tutorialText.text = tutorial;
                tutorialText.fontSize = fontSize;

                // Ensure moneyPanel is assigned
                if (moneyPanel != null)
                {
                    LeanTween.moveX(gameObject, moneyPanel.transform.position.x, duration)
                        .setEase(easeIn)
                        .setDelay(0.3f)
                        .setOnComplete(CloseSidePopUp);
                }
                else
                {
                    Debug.LogWarning("Money panel reference is missing.");
                }
            }
            else
            {
                Debug.LogWarning("TutorialText component is not assigned.");
            }
        }
    }

    public void OnClose()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(easeOut).setDelay(2f);
    }

    public void CloseSidePopUp()
    {
        if (moneyPanel != null)
        {
            LeanTween.moveX(gameObject, moneyPanel.transform.position.x - offsetX, 0.8f)
                .setEase(easeOut)
                .setDelay(closeDelay);
        }
        else
        {
            Debug.LogWarning("Money panel reference is missing in CloseSidePopUp.");
        }
    }

    public void LiftWaveRibbon()
    {
        if (moneyPanel != null)
        {
            LeanTween.moveY(gameObject, moneyPanel.transform.position.y + offsetY, duration)
                .setEase(easeOut)
                .setDelay(0.5f);
        }
        else
        {
            Debug.LogWarning("Money panel reference is missing in LiftWaveRibbon.");
        }
    }

    public void ReturnRibbonToStartPos()
    {
        LeanTween.moveY(gameObject, moneyPanel.transform.position.y, duration)
                .setEase(easeOut)
                .setDelay(0.5f);
    }

    private void DeActivatePopUp()
    {
        gameObject.SetActive(false);
    }
}
