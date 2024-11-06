using UnityEngine;

public class TowerScript : MonoBehaviour
{
    public BuildScript buildScript;
    public bool CanSpawn = false;
    public GameObject archer;

    private void Update()
    {
        if(!CanSpawn && buildScript.isTowerBuilt)
        {
            CanSpawn = true;
        }

        if(CanSpawn)
        {
            archer.SetActive(true);
        }
    }
}
