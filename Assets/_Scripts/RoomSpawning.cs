using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawning : MonoBehaviour
{

    //Public variables
    public int roomLevel = 1;
    public List<Transform> spawnPositions;
    public GameObject Zombie;

    //Private variables
    private int spawnLoop;
    private int passes = 0;

    // Start is called before the first frame update
    void Start()
    {
        int spawnCount = Random.Range(1, 7);
        Debug.Log(spawnCount);
        for (int i = 0; i < spawnCount; i++)
        {
            for (int x = 0; x < spawnPositions.Count; x++)
            {
                if (spawnCount > 0)
                {
                    GameObject zombo = Instantiate(Zombie);
                    zombo.transform.position = spawnPositions[x].transform.position;
                    zombo.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                    zombo.SetActive(true);
                    zombo.transform.position = new Vector3(zombo.transform.position.x + Random.Range(0.1f, 1), zombo.transform.position.y + Random.Range(0.1f, 1), zombo.transform.position.z);
                    spawnCount--;
                }
            }
        }
    }
}
