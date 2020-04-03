using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    //Public Variables
    public Animator anim;
    public Camera cam;
    public float interactRange = 1;
    public float moveSpeed = 1f;
    public GameObject lineRenderer;
    public Transform barrel;
    public float shootSpeed = 0.5f;
    public AudioSource audioSource;
    public List<GameObject> Loadout;
    public GameObject bulletSpark;
    public GameObject bulletSplatter;
    public Transform eyes;
    public CircleCollider2D alertRadius;
    public int layerMask = 9;
    public int ignoreMask = 11;

    //Private Variables
    private Rigidbody2D rb;
    private int openSide;
    private Vector2 movement;
    private LineRenderer lineR;
    private int gunShot = 0;
    private bool canShoot = true;
    private bool singleFire = false;
    
    void Start()
    {
        lineR = lineRenderer.GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();

        //LineRenderer setup
        lineR.enabled = false;
        lineR.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Look towards mouse
        faceMouse();

        //Get key input
        keyInput();

        //Set movement values based on WASD input
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    void keyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetInteger("weaponStance", 1);
            gunShot = 1;
            resetLoadout();
            Loadout[0].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetInteger("weaponStance", 2);
            gunShot = 0;
            resetLoadout();
            Loadout[1].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetInteger("weaponStance", 0);
            resetLoadout();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if (Input.GetKey(KeyCode.Mouse0) && canShoot && !singleFire)
        {
            if (anim.GetInteger("weaponStance") == 2)
            {
                singleFire = true;
            }
            if (anim.GetInteger("weaponStance") != 0)
            {
                canShoot = false;
            }
            Fire();
        }
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            singleFire = false;
        }
    }

    void resetLoadout()
    {
        for (int i = 0; i < Loadout.Count; i++)
        {
            Loadout[i].SetActive(false);
        }
    }

    void FixedUpdate() 
    {
        rb.velocity = new Vector2(
            Mathf.Lerp(0, movement.x * moveSpeed, 0.8f),
            Mathf.Lerp(0, movement.y * moveSpeed, 0.8f)
        );
        cam .transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }

    void faceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = cam.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        transform.up = direction;
    }

    void Interact()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, interactRange, 1 << layerMask);
        if (hit)
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform.name == "door")
            {
                //Get door rotation
                if (hit.transform.InverseTransformPoint(transform.position).y > 0)
                {
                    openSide = 1;
                }
                else
                {
                    openSide = 2;
                }
                if (hit.transform.GetComponent<Animator>().GetInteger("open") == 0)
                {
                    hit.transform.GetComponent<BoxCollider2D>().enabled = false;
                    hit.transform.GetComponent<Animator>().SetInteger("open", openSide);
                    StartCoroutine(enableDoor(hit.transform.gameObject));
                }
                else
                {
                    hit.transform.GetComponent<BoxCollider2D>().enabled = false;
                    hit.transform.GetComponent<Animator>().SetInteger("open", 0);
                    StartCoroutine(enableDoor(hit.transform.gameObject));
                }
            }
        }
    }

    void Fire()
    {
        RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, transform.up, 1, ~(1 << ignoreMask));
        if (hit)
        {
            canShoot = true;
            singleFire = false;
            return;
        }
        hit = Physics2D.Raycast(barrel.transform.position, transform.up, Mathf.Infinity, ~(1 << ignoreMask));
        if (hit)
        {
            if (anim.GetInteger("weaponStance") != 0)
            {
                if (anim.GetInteger("weaponStance") == 2)
                {
                    if (hit.transform.GetComponent<Health>() != null)
                    {
                        hit.transform.GetComponent<Health>().health -= 5;
                    }
                    bulletShot(Loadout[1].GetComponent<WeaponStats>().sound, Loadout[1].GetComponent<WeaponStats>().shootSpeed, hit.point);
                    GameObject spark = Instantiate(bulletSpark);
                    spark.transform.position = hit.point;
                    spark.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    spark.SetActive(true);
                    Destroy(spark, 0.5f);
                }
                if (anim.GetInteger("weaponStance") == 1)
                {
                    if (hit.transform.GetComponent<Health>() != null)
                    {
                        hit.transform.GetComponent<Health>().health -= 5;
                    }
                    bulletShot(Loadout[0].GetComponent<WeaponStats>().sound, Loadout[0].GetComponent<WeaponStats>().shootSpeed, hit.point);
                    GameObject spark = Instantiate(bulletSpark);
                    spark.transform.position = hit.point;
                    spark.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    spark.SetActive(true);
                    Destroy(spark, 0.5f);
                }
            }
        }
    }

    void bulletShot(AudioClip audioClip, float speed, Vector3 hit)
    {
        audioSource.PlayOneShot(audioClip);
        lineR.SetPosition(0, barrel.transform.position);
        lineR.SetPosition(1, hit);
        lineR.enabled = true;
        StartCoroutine(bulletTrail());
        StartCoroutine(shootDelay(speed));
    }

    IEnumerator enableDoor(GameObject obj)
    {
        yield return new WaitForSeconds(0.75f);
        obj.GetComponent<BoxCollider2D>().enabled = true;
    }
    IEnumerator bulletTrail()
    {
        yield return new WaitForSeconds(0.05f);
        lineR.enabled = false;
    }
    IEnumerator shootDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canShoot = true;
    }
}
