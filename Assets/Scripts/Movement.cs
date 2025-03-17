using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;
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
    [SerializeField] private GameObject slashAnimHandler;
    public string[] inventory; //A dynamic array that will contain every melee and ranged weapon the player accumulates

    public GameObject clone;
    public float HP = 20f;
    public GameObject HPBar;
    private float maxHPBarScaleX = 4.5f;
    public GameObject meleeCDBar;
    public GameObject rangedCDBar;
    public GameObject dodgeCDBar;

    //Variables for player inputs:
    public KeyCode meleeInput = KeyCode.Mouse0;
    public KeyCode rangedInput = KeyCode.Mouse1;
    public KeyCode dodgeInput = KeyCode.Space;

    //melee weapon stats, such as damage, swing speed, hurtbox distance (this number is higher if weapon is spear), hurtbox width (this number is higher if weapon is sword), cooldown
    public float[] melee = { 1f, 1f, 1f, 1f, 1f};
    public string meleeName = "N/A";

    //ranged weapon stats, such as damage, speed, size, rate of fire, AoE, Spread, Amount (how many bullets fire per shot, like a shotgun)
    public float[] ranged = {1f, 1f, 1f, 1f, 1f, 1f, 1f};
    public string rangedName = "N/A";

    private float speed = 10f;
    private float statShowerDebounce = 20f; //To make sure the user isn't spammed with stats after pressing space

    private bool meleeWeaponActive = false; //if true, then the player is currently swinging their main weapon. False if otherwise.
    private float weaponSwingBuffer = -1f; //Time it takes for you to swing again. (=0 means attack finished, =-1 means attack is ready
    private bool weaponCoolDown = false; //If true, then the melee weapon is in cooldown (similar to buffer but the user can move)
    private float weaponCoolDownCounter = 0f;

    private bool rangedWeaponActive = false;
    private float rangedWeaponBuffer = 30f; //Simpler than ranged weapon, 0 = ready to shoot, not 0 = not ready to shoot
    private bool rangedCoolDown = false; //If true, then the ranged weapon is in cooldown
    private float rangedCoolDownCounter = 0f;

    //Dodge variables
    private bool isDodgeActive = false;
    private bool isDodgeInCooldown = false;
    private float dodgeCooldownStat = 50f;
    private float dodgeDistanceStat = 10f;
    private float dodgeCooldown;
    private float dodgeDistance;

    //Anim variables
    public float maxValFS = 1f;
    private float counter = 0f;
    private float frameSkip = 10f / 8f; //(MaxValFrameskip): Every 10 frames the animation switches to the next image
    public SpriteResolver spriteResolver;
    private GameObject playerSprite;
    public AudioSource walkAudio;
    public AudioSource swingAudio;
    private string directional = "Down"; //Used for animating movement and still player anims

    // Start is called before the first frame update
    void Start()
    {
        spriteResolver = GameObject.Find("PlayerSprite").GetComponent<SpriteResolver>();
        playerSprite = GameObject.Find("PlayerSprite");
        frameSkip = maxValFS;
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

    void getNPlayAnim(string animName)
    {
        if (frameSkip <= 0f)
        {
            ChangeSprite(animName, counter);
            counter++;
            if (counter >= 8)
            {
                counter = 0;
            }
            frameSkip = maxValFS;
        }
        frameSkip--;
    }

    void ChangeSprite(string category, float label)
    {
        if (spriteResolver)
        {
            spriteResolver.SetCategoryAndLabel(category, label.ToString()); // Change dynamically
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Adjusts the HP bar every frame
        HPBar.transform.localScale = new Vector3((maxHPBarScaleX*HP)/20, HPBar.transform.localScale.y, 0f);
        HPBar.transform.localPosition = new Vector3((HP - 20)/8.7f, 0f, 0f);

        //All "update" code is subject to change (I don't want a bunch of if/else statements)

        sprite.transform.position = transform.position;

        bool isMoving = false; //Is the player moving?
        string movement = "Idle";
        //Basic movement script below
        Vector2 velHold = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            velHold = velHold + new Vector2(0f, 1f);
            directional = "Up";
            movement = "Run";
            isMoving = true;
            maxValFS = 5f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velHold = velHold + new Vector2(0f, -1f);
            directional = "Down";
            movement = "Run";
            isMoving = true;
            maxValFS = 5f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velHold = velHold + new Vector2(-1f, 0f);
            playerSprite.transform.localScale = new Vector3(0.1732944f, 0.1732944f, 0.1732944f);
            directional = "Left";
            movement = "Run";
            isMoving = true;
            maxValFS = 2.5f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velHold = velHold + new Vector2(1f, 0f);
            playerSprite.transform.localScale = new Vector3(-0.1732944f, 0.1732944f, 0.1732944f);
            directional = "Left";
            movement = "Run";
            isMoving = true;
            maxValFS = 2.5f;
        }
        velHold.Normalize();
        getNPlayAnim(movement + directional);
        if (isMoving && !walkAudio.isPlaying)
        {
            walkAudio.Play();
        }
        else if (!isMoving)
        {
            walkAudio.Pause();
        }

        //Dodge right before the rest of the movement script, as dodge will technically manipulate the movement
        if (((Input.GetKey(dodgeInput)) || isDodgeActive == true) && !isDodgeInCooldown)
        {
            dodgeCDBar.transform.localScale = new Vector3(1f, 1f, 0f);
            dodgeCDBar.transform.localPosition = new Vector3(0f, 0f, 0f);

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
            float func = dodgeCooldown / dodgeCooldownStat;
            dodgeCDBar.transform.localScale = new Vector3(1f, func, 0f);
            float sizeDiff = func - 1;
            dodgeCDBar.transform.localPosition = new Vector3(0f, sizeDiff / 3f, 0f);
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
        if (!meleeWeaponActive)
        {
            transform.right = mouseWorldPos - transform.position;
        }

        rb.velocity = velHold * speed;

        //below is meant to show the user their weapon stats when they press space
        if ((Input.GetKey(KeyCode.B)) && statShowerDebounce == 20f)
        {
            statShowerDebounce = 0f;
            Debug.Log("[MELEE]::: Damage: "+ melee[0]+ " ||| Swing Speed: "+ melee[1]+ " ||| Hurtbox Length: "+ melee[2]+ " ||| Hurtbox Width: "+ melee[3]);
            Debug.Log("[RANGED]::: Damage: " + ranged[0] + " ||| Speed: " + ranged[1] + " ||| Size: " + ranged[2] + " ||| Rate of Fire: " + ranged[3] + " ||| AoE: "+ ranged[4] + " ||| Spread: " + ranged[5] + " ||| Amount: " + ranged[6]);
        } else if (statShowerDebounce != 20f)
        {
            statShowerDebounce++;
        }

        ///////////////////
        //Below will be some basic attack scripts, again subject to change once I find a better way to do this.
        ///////////////////
        //Melee attack (will eventually be triggered by left click)
        if ((Input.GetKey(meleeInput)) && !meleeWeaponActive && !weaponCoolDown)
        {
            swingAudio.Play();
            meleeWeaponActive = true;
            weaponCoolDown = true;
            weaponCoolDownCounter = (100 - (melee[4] * 5)); //Cooldown for melee attacks will be proportional to the weapon's damage, speed, and size

            if (melee[2] > melee[3])
            {
                whbx.direction = UnityEngine.CapsuleDirection2D.Horizontal;
            }
            else
            {
                whbx.direction = UnityEngine.CapsuleDirection2D.Vertical;
            }

            //Creates a new collider which acts as a hitbox. The script will get the mouse's position in order to place the hitbox correctly.
            Vector2 wepBoxDimensions = Vector2.zero;
            Vector2 wepBoxOffset = Vector2.zero; wepBoxOffset.x = 0.5f + ((melee[2])/2); wepBoxOffset.y = 0f;

            wepBoxDimensions.x = melee[2];
            wepBoxDimensions.y = melee[3];
            whbx.size = wepBoxDimensions;
            whbx.offset = wepBoxOffset;
            whbx.enabled = true;

            clone = Instantiate(slashAnimHandler, transform.localPosition, Quaternion.identity) as GameObject;
            clone.transform.right = transform.right;
            var lookVector = mouseWorldPos - hbx.transform.position;
            lookVector.Normalize();
            clone.transform.parent = transform;
            clone.transform.position = transform.position + (lookVector*0.4f);
            clone.transform.localScale = new Vector3(0.2f*(whbx.size.x*1.25f), 0.2f, 0f);
            if (lookVector.x < 0f)
            {
                clone.transform.localScale = new Vector3(0.2f * (whbx.size.x * 1.25f), -0.2f, 0f);
            }
            
            slashTester slashScript = clone.GetComponent<slashTester>();
            slashScript.maxValFS = (25) - melee[1];
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
                meleeCDBar.transform.localScale = new Vector3(1f, 1f, 0f);
                meleeCDBar.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            else //Weapon is currently swinging
            { 
                weaponSwingBuffer--;
            }
        }
        else if (weaponCoolDown)
        {
            if (weaponCoolDownCounter <= 0)
            {
                weaponCoolDown = false; //no longer in cooldown
            }
            else
            {
                weaponCoolDownCounter--;
                float maxCD = (100 - (melee[4]*5));
                float func = weaponCoolDownCounter / maxCD;
                meleeCDBar.transform.localScale = new Vector3(1f, func, 0f);
                float sizeDiff = func-1;
                meleeCDBar.transform.localPosition = new Vector3(0f, sizeDiff/3f, 0f);
            }
        }

        //Ranged attack script (will eventually be triggered by right click)
        if (Input.GetKey(rangedInput) && !rangedCoolDown)
        {
            rangedCoolDown = true; 
            rangedWeaponActive = true;
            rangedCoolDownCounter = (200 - ranged[3]*10); //Cooldown will be proportional to how good the weapon's damage, speed, size, and AoE is.

            //resize the gun image so it becomes visible
            var lookVector = mouseWorldPos - hbx.transform.position;
            lookVector.Normalize();
            if (lookVector.x < 0f)
            {
                gameObject.transform.GetChild(0).transform.localScale = new Vector3(0.2f, -0.2f, 0.2f);
            }
            else
            {

                gameObject.transform.GetChild(0).transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }


            //Script below will spawn a projectile, which will be its own "entity"
            for (int i=0; i < ranged[6]; i++)
            {
                clone = Instantiate(bullet, transform.GetChild(0).GetChild(0).position, Quaternion.identity) as GameObject;
                ProjectileScript projectileStats = clone.GetComponent<ProjectileScript>();

                mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f; // zero z
                projectileStats.destination = mouseWorldPos;
                projectileStats.parent = gameObject;
                for (int j = 0; j < 7; j++)
                {
                    if (j == 5)
                    {
                        projectileStats.projStats[j] = ranged[j]/25;
                    }
                    else
                    {
                        projectileStats.projStats[j] = ranged[j];
                    }
                }
            }
            rangedCDBar.transform.localScale = new Vector3(1f, 1f, 0f);
            rangedCDBar.transform.localPosition = new Vector3(0f, 0f, 0f);

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
                float maxCD = (200 - ranged[3] * 10);
                float func = rangedCoolDownCounter / maxCD;
                rangedCDBar.transform.localScale = new Vector3(1f, func, 0f);
                float sizeDiff = func - 1;
                rangedCDBar.transform.localPosition = new Vector3(0f, sizeDiff / 3f, 0f);
            }
        }
        if (rangedWeaponActive) //Buffer a bit for the player to show their gun, shoot, then put their gun away (no more than a half a second)
        {
            if (rangedWeaponBuffer <= 0)
            {
                rangedWeaponBuffer = 30;
                gameObject.transform.GetChild(0).transform.localScale = new Vector3(0.2f, 0f, 0.2f);
                rangedWeaponActive = false;
            }
            else
            {
                rangedWeaponBuffer--;
            }
        }

    }
}
