using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{

    //Public variables
    public EdgeCollider2D fov;
    public Transform player;
    public Transform eyes;
    public float rotSpeed = 2f;
    public float moveSpeed = 20;

    //Private variables
    bool trackPlayer = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Health>().health <= 0)
        {
            Destroy(gameObject);
        }
        if (trackPlayer)
        {
            Vector3 direction = player.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, direction, 100);
            if (hit)
            {
                if (hit.transform == player)
                {
                    //Move towards player
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90), rotSpeed * Time.deltaTime);

                    rb.AddForce(transform.up * 100 * moveSpeed * Time.deltaTime);
                }
                //if (hit.collider == fov)
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == player)
        {
            trackPlayer = true;
        }
    }
}
