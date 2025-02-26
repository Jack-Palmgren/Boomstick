using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using Random = Unity.Mathematics.Random;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CapsuleCollider2D hbx;
    [SerializeField] private CapsuleCollider2D whbx;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject sprite; //The sprite, which will be under a different game object.

    public GameObject clone;
    public float HP = 20f;

    //melee weapon stats, such as damage, swing speed, hurtbox distance (this number is higher if weapon is spear), hurtbox width (this number is higher if weapon is sword)
    public float[] melee = { 1f, 1f, 1f, 1f };
    public string meleeName = "N/A";

    //ranged weapon stats, such as damage, projectile speed, projectile size, rate of fire, AoE, Spread, Amount (how many bullets fire per shot, like a shotgun)
    public float[] ranged = {1f, 1f, 1f, 1f, 1f, 1f, 1f};
    public string rangedName = "N/A";

    private float speed = 10f;
    private float statShowerDebounce = 20f; //To make sure the user isn't spammed with stats after pressing space

    private bool meleeWeaponActive = false; //if true, then the player is currently swinging their main weapon. False if otherwise.
    private float weaponSwingBuffer = -1f; //Time it takes for you to swing again. (=0 means attack finished, =-1 means attack is ready
    private bool weaponCoolDown = false; //If true, then the melee weapon is in cooldown (similar to buffer but the user can move)
    private float weaponCoolDownCounter = 0f;

    private bool rangedWeaponActive = false;
    private float rangedWeaponBuffer = 0f; //Simpler than ranged weapon, 0 = ready to shoot, not 0 = not ready to shoot
    private bool rangedCoolDown = false; //If true, then the ranged weapon is in cooldown
    private float rangedCoolDownCounter = 0f;

    //Dodge variables
    private bool isDodgeActive = false;
    private float dodgeCooldownStat = 60f;
    private float dodgeCooldown;
    private bool isDodgeInCooldown = false;
    private float dodgeDistanceStat = 10f;
    private float dodgeDistance;

    // Start is called before the first frame update
    void Start()
    {
        whbx.enabled = false;
        dodgeCooldown = dodgeCooldownStat;
        dodgeDistance = dodgeDistanceStat;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collided with: " + collision.rigidbody);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Triggered by: " + collision.gameObject);
        if (collision.gameObject.tag == "Enemy" && meleeWeaponActive)
        {
            EnemyMovementScript enemyScript = collision.gameObject.GetComponent<EnemyMovementScript>();
            enemyScript.HP = enemyScript.HP - melee[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //All "update" code is subject to change (I don't want a bunch of if/else statements)

        sprite.transform.position = transform.position;

        //Basic movement script below
        Vector2 velHold = Vector2.zero;
        if (!meleeWeaponActive && !rangedWeaponActive)
        {
            if (Input.GetKey(KeyCode.W))
            {
                velHold = velHold + new Vector2(0f, 1f);
            }
            if (Input.GetKey(KeyCode.S))
            {
                velHold = velHold + new Vector2(0f, -1f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                velHold = velHold + new Vector2(-1f, 0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                velHold = velHold + new Vector2(1f, 0f);
            }
            velHold.Normalize();

            //Dodge right before the rest of the movement script, as dodge will technically manipulate the movement
            if (((Input.GetKey(KeyCode.C)) || isDodgeActive == true) && !isDodgeInCooldown)
            {
                isDodgeActive = true;
                hbx.enabled = false;
                velHold = velHold * dodgeDistance;
                dodgeDistance--;
                if (dodgeDistance == 0)
                {
                    hbx.enabled = true;
                    isDodgeInCooldown = true;
                    isDodgeActive = false;
                }
            }
            if (isDodgeInCooldown)
            {
                dodgeCooldown--;
                if (dodgeCooldown == 0)
                {
                    isDodgeInCooldown = false;
                    dodgeCooldown = dodgeCooldownStat;
                    dodgeDistance = dodgeDistanceStat;
                }
            }

            //Script below to constaly face the mouse. It should disable when the player is attacking.
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // zero z
            transform.right = mouseWorldPos - transform.position;
        }
        rb.velocity = velHold * speed;

        //below is meant to show the user their weapon stats when they press space
        if ((Input.GetKey(KeyCode.Space)) && statShowerDebounce == 20f)
        {
            statShowerDebounce = 0f;
            Debug.Log("[MELEE]::: Damage: "+ melee[0]+ " ||| Swing Speed: "+ melee[1]+ " ||| Hurtbox Length: "+ melee[2]+ " ||| Hurtbox Width: "+ melee[3]);
            Debug.Log("[RANGED]::: Damage: " + ranged[0] + " ||| Speed: " + ranged[1] + " ||| Size: " + ranged[2] + " ||| Rate of Fire: " + ranged[3] + " ||| AoE: "+ ranged[4] + " ||| Spread: " + ranged[5] + " ||| Amount: " + ranged[6]);
        } else if (statShowerDebounce != 20f)
        {
            statShowerDebounce++;
        }

        //Below will be some basic attack scripts, again subject to change once I find a better way to do this.

        //Melee attack (will eventually be triggered by left click)
        if ((Input.GetKey(KeyCode.Z)) && !meleeWeaponActive && !weaponCoolDown)
        {
            meleeWeaponActive = true;
            weaponCoolDown = true;
            weaponCoolDownCounter = melee[0] + melee[1] + melee[2] + melee[3]; //Cooldown for melee attacks will be proportional to the weapon's damage, speed, and size

            //Creates a new collider which acts as a hitbox. The script will get the mouse's position in order to place the hitbox correctly.
            Vector2 wepBoxDimensions = Vector2.zero;
            Vector2 wepBoxOffset = Vector2.zero; wepBoxOffset.x = 0.5f + ((melee[2])/2); wepBoxOffset.y = 0f;

            wepBoxDimensions.x = melee[2];
            wepBoxDimensions.y = melee[3];
            whbx.size = wepBoxDimensions;
            whbx.offset = wepBoxOffset;
            whbx.enabled = true;
        }
        else if (meleeWeaponActive)
        {
            if (weaponSwingBuffer == 0) //We're done swinging, enter cooldown.
            {
                whbx.enabled = false;
                meleeWeaponActive = false;
                weaponSwingBuffer = -1;
            }
            else if (weaponSwingBuffer == -1) //Weapon is ready to be swung
            {
                weaponSwingBuffer = 25 - melee[1];
            }
            else //Weapon is currently swinging
            { 
                weaponSwingBuffer--; 
            }
        }
        else if (weaponCoolDown)
        {
            if (weaponCoolDownCounter == 0)
            {
                weaponCoolDown = false; //no longer in cooldown
            }
            else
            {
                weaponCoolDownCounter--;
            }
        }

        //Ranged attack script (will eventually be triggered by right click)
        if (Input.GetKey(KeyCode.X) && !rangedWeaponActive && !rangedCoolDown)
        {
            rangedWeaponBuffer = -1;
            rangedWeaponActive = true;
            rangedCoolDown = true;
            rangedCoolDownCounter = ((ranged[0] + ranged[1] + ranged[2] + ranged[4]) - ranged[3]) * ranged[6]; //Cooldown will be proportional to how good the weapon's damage, speed, size, and AoE is.
            
            //Script below will spawn a projectile, which will be its own "entity"
            for(int i=0; i < ranged[6]; i++)
            {
                clone = Instantiate(bullet, rb.transform.localPosition, Quaternion.identity) as GameObject;
                ProjectileScript projectileStats = clone.GetComponent<ProjectileScript>();

                var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f; // zero z
                projectileStats.destination = mouseWorldPos;
                for (int j = 0; j < 7; j++)
                {
                    projectileStats.projStats[j] = ranged[j];
                }
            }
            
        }
        else if (rangedWeaponActive)
        {
            if (rangedWeaponBuffer == -1)
            {
                rangedWeaponBuffer = 100;
            }
            else if (rangedWeaponBuffer <= 0)
            {
                rangedWeaponBuffer = 0;
                rangedWeaponActive = false;
            }
            else
            {
                rangedWeaponBuffer = rangedWeaponBuffer - ranged[3];
            }

        }
        else if (rangedCoolDown) //Ranged weapon has entered cooldown
        {
            if (rangedCoolDownCounter <= 0)
            {
                rangedCoolDown = false; //no longer in cooldown
            }
            else
            {
                rangedCoolDownCounter--;
            }
        }
    }
}
