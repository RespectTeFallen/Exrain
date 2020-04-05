using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawning : MonoBehaviour
{

    //Public variables
    public int roomLevel = 1;
    public List<Transform> spawnPositions;
    public GameObject render;
    public List<Collider2D> boundingBox;

    //Private variables
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("player");
        render.SetActive(false);
    }

    bool checkContain()
    {
        bool contains = false;
        foreach (Collider2D col in boundingBox)
        {
            if (col.bounds.Contains(player.transform.position))
            {
                render.SetActive(true);
                contains = true;
            }
        }
        if (!contains)
        {
            render.SetActive(false);
        }
        return contains;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "RenderDistance")
        {
            render.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "RenderDistance")
        {
            render.SetActive(false);
        }
    }
}
