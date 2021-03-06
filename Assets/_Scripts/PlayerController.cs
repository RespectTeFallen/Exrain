﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }
    #endregion

    //Public Variables
    public GameObject UI;
    public GameObject uiPlayer;
    public Animator anim;
    public Camera cam;
    public float interactRange = 1;
    public float moveSpeed = 1f;
    public GameObject lineRenderer;
    public Transform barrel;
    public Transform eyes;
    public float shootSpeed = 0.5f;
    public AudioSource audioSourceWeapon;
    public AudioSource audioSourcePlayer;
    public List<GameObject> Loadout;
    public List<GameObject> bulletSpark;
    public CircleCollider2D alertRadius;
    public int layerMask = 9;
    public int ignoreMask;
    public TextMeshProUGUI fps;
    public List<AudioClip> soundEffects;
    public GameObject ConsoleWindow;
    public TMP_InputField consoleText;

    //Private Variables
    private Rigidbody2D rb;
    private int openSide;
    private float fpsDelay = 0;
    private Vector2 movement;
    private LineRenderer lineR;
    private int gunShot = 0;
    private bool canShoot = true;
    private bool click = false;
    private float bloom = 0;
    private bool Sprinting = false;
    private bool selected = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //LineRenderer setup
        lineR = lineRenderer.GetComponent<LineRenderer>();
        lineR.enabled = false;
        lineR.useWorldSpace = true;
    }
    
    void Update()
    {
        //Look towards mouse
        faceMouse();

        //Get key input
        Commands();
        if (!ConsoleWindow.activeSelf)
        {
            keyInput();
            Move();
        }
    }

    void LateUpdate()
    {
        //FPS counter
        if (fpsDelay < 0.5f)
        {
            fpsDelay += 1 * Time.deltaTime;
        }
        else
        {
            int frame = (int)(1f / Time.unscaledDeltaTime);
            fps.text = "FPS: " + frame;
            fpsDelay = 0;
        }
    }

    void Commands()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ConsoleWindow.SetActive(!ConsoleWindow.activeSelf);
            Console.instance.consoleText.ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return) && ConsoleWindow.activeSelf)
        {
            string command = consoleText.text;
            Console.instance.SendInput(command);
            consoleText.text = "";
        }
    }

    void keyInput()
    {

        Sprinting = false;

        if (Input.GetButtonDown("Inventory"))
        {
            movement = Vector2.zero;
            UI.SetActive(!UI.activeSelf);
            if (UI.activeSelf)
            {
                Inventory.updateInventory = true;
            }
            else
            {
                Inventory.updateInventory = false;
            }
        }

        if (UI.activeSelf)
        {
            movement = Vector2.zero;
            return;
        }
        
        //Set movement values based on WASD input
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

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

        //Sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            audioSourcePlayer.Stop();
            Sprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            audioSourcePlayer.Stop();
            Sprinting = false;
            moveSpeed = 3.5f;
        }

        //Firing
        if (Input.GetKey(KeyCode.Mouse0) && !click)
        {
            if (UI.activeSelf)
            {
                return;
            }
            else if (anim.GetInteger("weaponStance") != 0 && canShoot)
            {
                if (anim.GetInteger("weaponStance") == 2)
                {
                    click = true;
                }
                canShoot = false;
                Fire();
            }
        }
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            click = false;
            selected = false;
        }
    }

    void resetLoadout()
    {
        for (int i = 0; i < Loadout.Count; i++)
        {
            Loadout[i].SetActive(false);
        }
    }

    void Move() 
    {
        if (rb.velocity != Vector2.zero)
        {
            if (Sprinting)
            {
                moveSpeed = 5.5f;
                if (!audioSourcePlayer.isPlaying)
                {
                    audioSourcePlayer.clip = soundEffects[1];
                    audioSourcePlayer.Play();
                }
            }
            else
            {
                if (!audioSourcePlayer.isPlaying)
                {
                    audioSourcePlayer.clip = soundEffects[0];
                    audioSourcePlayer.Play();
                }
            }
        }
        else
        {
            audioSourcePlayer.Stop();
        }
        rb.velocity = new Vector2(
            Mathf.Lerp(0, movement.x * moveSpeed, 0.8f),
            Mathf.Lerp(0, movement.y * moveSpeed, 0.8f)
        );
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }

    void faceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (UI.activeSelf)
        {
            Vector2 direction = new Vector2(
                mousePosition.x - uiPlayer.transform.position.x,
                mousePosition.y - uiPlayer.transform.position.y
                );
            uiPlayer.transform.up = direction;
        }
        else
        {
            mousePosition = cam.ScreenToWorldPoint(mousePosition);
            Vector2 direction = new Vector2(
                mousePosition.x - transform.position.x,
                mousePosition.y - transform.position.y
                );
            transform.up = direction;
        }
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
            if (hit.transform.GetComponent<Lootable>() != null)
            {
                for (int x = 0; x < hit.transform.GetComponent<Lootable>().loot.Count; x++)
                {
                    int loopCount = hit.transform.GetComponent<Lootable>().loot.Count;
                    Item itemPlace = hit.transform.GetComponent<Lootable>().loot[x];
                    for (int i = 0; i < Inventory.instance.nearby.Count; i++)
                    {
                        if (Inventory.instance.nearby[i].itemID == 0)
                        {
                            Inventory.instance.nearby[i] = new Item(itemPlace.itemName, itemPlace.itemID, itemPlace.itemDesc, itemPlace.itemCount, itemPlace.itemStackable, itemPlace.itemData, itemPlace.itemType);
                            loopCount--;
                            if (loopCount == 0)
                            {
                                UI.SetActive(true);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    void Fire()
    {
        //Shoot lock
        float dist = Vector3.Distance(eyes.transform.position, barrel.transform.position);
        RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, transform.up, dist, ~(1 << ignoreMask));
        if (hit)
        {
            canShoot = true;
            click = false;
        }
        else
        {
            //Bloom
            Vector3 direction = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float change = Loadout[anim.GetInteger("weaponStance") - 1].GetComponent<WeaponStats>().bloom;
            bloom = Random.Range(angle - change, angle + change) * Mathf.Deg2Rad;
            Vector3 shootDir = new Vector3(Mathf.Cos(bloom), Mathf.Sin(bloom), -1f);

            //Hitscan
            hit = Physics2D.Raycast(barrel.transform.position, shootDir, Mathf.Infinity, ~(1 << ignoreMask));
            if (hit)
            {
                if (anim.GetInteger("weaponStance") != 0)
                {
                    int stance = anim.GetInteger("weaponStance") - 1;

                    if (hit.transform.GetComponent<Health>() != null)
                    {
                        hit.transform.GetComponent<Health>().health -= Loadout[stance].GetComponent<WeaponStats>().damage;
                        if (hit.transform.GetComponent<Health>().isAlive)
                        {
                            bulletShot(Loadout[stance].GetComponent<WeaponStats>().sound, Loadout[stance].GetComponent<WeaponStats>().shootSpeed, hit.point, hit.normal, 1);
                        }
                        else
                        {
                            bulletShot(Loadout[stance].GetComponent<WeaponStats>().sound, Loadout[stance].GetComponent<WeaponStats>().shootSpeed, hit.point, hit.normal, 0);
                        }
                    }
                    else
                    {
                        bulletShot(Loadout[stance].GetComponent<WeaponStats>().sound, Loadout[stance].GetComponent<WeaponStats>().shootSpeed, hit.point, hit.normal, 0);
                    }
                }
            }
            else
            {
                canShoot = true;
                click = false;
            }
        }
    }

    void spark(int num, Vector3 hit, Vector3 normal)
    {
        GameObject spark = Instantiate(bulletSpark[num]);
        spark.transform.position = hit;
        spark.transform.rotation = Quaternion.FromToRotation(Vector3.forward, normal);
        spark.SetActive(true);
        Destroy(spark, 0.5f);
    }

    void bulletShot(AudioClip audioClip, float speed, Vector3 hit, Vector3 normal, int num)
    {
        //Spark
        spark(num, hit, normal);

        //Audio
        audioSourceWeapon.PlayOneShot(audioClip);

        //Draw line
        lineR.SetPosition(0, barrel.transform.position);
        lineR.SetPosition(1, hit);
        lineR.enabled = true;

        //Start loops
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
