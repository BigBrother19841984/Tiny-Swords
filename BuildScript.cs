using UnityEngine;

public class BuildScript : MonoBehaviour
{
    private SpriteRenderer _rend;
    public Color color;
    public float AddMoneyTime = 3f;
    private float[] timers = {0f,0f};

    [Header("Bool")]
    public bool isBuilt = false;
    private bool isMineBuilt = false;
    public bool isHouseBuilt = false;
    public bool isTowerBuilt = false; 
    private bool isEnough;
    private bool effectIsMade = false;
    public bool isPlayerInTrigger = false;

    [Header("Costs")]
    public int HouseCost = 20;
    public int MineCost = 40;
    public int CastleCost = 100;
    public int TowerCost = 50;
    private int cost;


    [Header("Effects")]
    public LeanTweenType easeType;
    public float duration = 0.5f;
    public Transform effectPos;
    public ParticleSystem effect;
    public AudioSource sound;
   //public AudioClip[] buildSounds;

    [Header ("References")]
    public GameManager gm;
    public MoneyManager moneyManager;
    public Tween tween;
    public GameObject HouseMarker;
    public GameObject TowerMarker;
    public GameObject MineMarker;
   // public Transform tweenOffset;
   //public float delay = .5f;
   // private GameObject buildingToPlace;

    private void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();
        //buildingToPlace = GameObject.FindWithTag(tag);
        if(tag == "House")
        {
            cost = HouseCost;
        }
        else if(tag == "Mine")
        {
            cost = MineCost;
        }
        else if(tag == "Castle")
        {
            cost = CastleCost;
        }
        else if(tag == "Tower")
        {
            cost = TowerCost;
        }

        tween.SidePopUp(tween.tutorialBanners[1], duration * 3f, 37);
    }

    private void Update()
    {
        if(moneyManager.amount >= cost)
        {
            isEnough = true;
        }
        else
        {
            isEnough = false;
        }

        if (isPlayerInTrigger && !isBuilt)
        {
            SetTransparency(0.5f);
            if (gameObject.tag == "House")
            {
                HouseMarker.SetActive(true);
            }
            else if(gameObject.tag == "Tower")
            {
                TowerMarker.SetActive(true);
            }
            else if(gameObject.tag == "Mine")
            {
                MineMarker.SetActive(true);
            }
           // destroyed.enabled = false;
        }
        else if(!isPlayerInTrigger && !isBuilt || isBuilt)
        {
            if(gameObject.tag == "House")
            {
               HouseMarker.SetActive(false);
            }
            else if(gameObject.tag == "Tower")
            {
                TowerMarker.SetActive(false);
            }
            else if(gameObject.tag == "Mine")
            {
                MineMarker.SetActive(false);
            }
        }
        if(!gm.isInWave)
        {
            Build();
        }

        if(Input.GetKeyDown(KeyCode.B) && gm.isInWave && isPlayerInTrigger) //When player tries to place a building during the wave 
        {
            tween.SidePopUp(tween.tutorialBanners[0], duration, 37);
        }

        if(Input.GetKeyDown(KeyCode.B) && !isEnough && isPlayerInTrigger)// Not enough money
        {
            tween.SidePopUp(tween.tutorialBanners[2], duration, 33);
        }
        
        //if(gm.isTutorial && isPlayerInTrigger) // When we teach the player how to place a building(B to build)
        //{
        //    tween.SidePopUp(tween.tutorialBanners[1], duration, 37);
        //}

        
        MineIncome(3);
        HouseIncome(1);
    }

    private void Build()
    {
        if (Input.GetKeyDown(KeyCode.B) && isPlayerInTrigger && !isBuilt && isEnough)
        {
            isBuilt = true;
            Debug.Log("Building done!");
            //Animate(Vector3.up, duration);

            if (tag == "House" && isBuilt)
            {
                SetTransparency(1f);
                BuyHouse();
                isHouseBuilt = true;
            }
            else if (tag == "Mine" && isBuilt)
            {
                SetTransparency(1f);
                BuyGoldMine();
                isMineBuilt = true;
            }
            else if(tag == "Castle" && isBuilt)
            {
                SetTransparency(1f);
                BuyCastle();
            }
            else if(tag == "Tower" && isBuilt)
            {
                SetTransparency(1f);
                BuyTower();
                isTowerBuilt = true;
            }

            if(isBuilt && !effectIsMade)
            {
                sound.Play();
                Instantiate(effect, effectPos.position, Quaternion.identity);
                effectIsMade = true;
            }
        }
    }

    private void SetTransparency(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        color.a = alpha;
        _rend.color = color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBuilt)
        {
            SetTransparency(0f);
            isPlayerInTrigger = false;
            //destroyed.enabled = true;
        }
    }

    public void BuyGoldMine()
    {
        if(moneyManager.amount >= MineCost)
        {
           moneyManager.UpdateMoney(MineCost);
        }
    }

    public void MineIncome(int income)
    {
        if (isMineBuilt && gm.isInWave)
        {
            if (timers[0] < AddMoneyTime)
            {
                timers[0] += Time.deltaTime;
            }
            else if (timers[0] > AddMoneyTime)
            {
                moneyManager.UpdateMoneyPositive(income);
                timers[0] = 0f;
            }
        }
    }

    public void BuyHouse()
    {
        if(moneyManager.amount >= HouseCost)
        {
          moneyManager.UpdateMoney(HouseCost);
        }
    }

    public void HouseIncome(int income)
    {
        if (isHouseBuilt && gm.isInWave)
        {
            if (timers[1] < AddMoneyTime)
            {
                timers[1]+= Time.deltaTime;
            }
            else if (timers[1] > AddMoneyTime)
            {
                moneyManager.UpdateMoneyPositive(income);
                timers[1] = 0f;
            }
        }
    }

    public void BuyCastle()
    {
        if(moneyManager.amount >=  CastleCost)
        {
            moneyManager.UpdateMoney(CastleCost);
        }    
    }

    public void BuyTower()
    {
        if (moneyManager.amount >= TowerCost)
        {
            moneyManager.UpdateMoney(TowerCost);
        }
    }

}
