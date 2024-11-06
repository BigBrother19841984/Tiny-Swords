using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
   public Text money;
   public int amount;


    public void Start()
    {
        money.text = amount.ToString();
        amount = Mathf.Clamp(amount ,0, 10000);
    }

    public void UpdateMoney(int cost)
    {
        amount -= cost;
        money.text = amount.ToString();
    }
    public void UpdateMoneyPositive(int cost)
    {
        amount += cost;
        money.text = amount.ToString();
    }
}
