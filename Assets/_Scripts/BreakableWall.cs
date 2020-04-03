using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{

    public List<Sprite> wallHealth;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = wallHealth[4];
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Health>().health < 80)
        {
            GetComponent<SpriteRenderer>().sprite = wallHealth[3];
        }
        if (GetComponent<Health>().health < 60)
        {
            GetComponent<SpriteRenderer>().sprite = wallHealth[2];
        }
        if (GetComponent<Health>().health < 40)
        {
            GetComponent<SpriteRenderer>().sprite = wallHealth[1];
        }
        if (GetComponent<Health>().health < 20)
        {
            GetComponent<SpriteRenderer>().sprite = wallHealth[0];
        }
        if (GetComponent<Health>().health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
