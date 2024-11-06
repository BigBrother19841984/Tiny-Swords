using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{
    public GameObject[] goblins;
    public Transform[] spawnPos;

    private float timer = 0f;
    public float spawnTime = 2f;
    public int goblinSpawnAmount = 3;

    private void Update()
    {

        if(timer < spawnTime)
        {
            timer += Time.deltaTime;
        }
        else if (timer > spawnTime)
        {
            int randomSpawnIndex = Random.Range(0, spawnPos.Length);
            for(int i = 0; i < goblinSpawnAmount; i++)
            {
                SpawnEnemy(randomSpawnIndex);
            }
            timer = 0;
        }
    }

    private void SpawnEnemy(int randomSpawnIndex)
    {
        int i = Random.Range(0, goblins.Length);
        Instantiate(goblins[i], spawnPos[randomSpawnIndex].position, Quaternion.identity);
    }
}
