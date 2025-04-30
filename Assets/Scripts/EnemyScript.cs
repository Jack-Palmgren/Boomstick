using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //melee weapon stats, such as damage, swing speed, hurtbox distance (this number is higher if weapon is spear), hurtbox width (this number is higher if weapon is sword)
    public float[] melee = { 1f, 1f, 1f, 1f, 1f };
    public string meleeName = "N/A";
    public string meleeType = "N/A";
    public GameObject MeleeRange;

    //ranged weapon stats, such as damage, projectile speed, projectile size, rate of fire, AoE, Spread, Amount (how many bullets fire per shot, like a shotgun)
    public float[] ranged = { 1f, 1f, 1f, 1f, 1f, 1f, 1f };
    public string rangedName = "N/A";
    public string rangedType = "N/A";

    public GameObject slashAnimHandler;
    private bool meleeWeaponIsInRange = false;
    private bool meleeWeaponActive = false; //if true, then the player is currently swinging their main weapon. False if otherwise.
    private float weaponSwingBuffer = -1f; //Time it takes for you to swing again. (=0 means attack finished, =-1 means attack is ready
    private bool weaponCoolDown = false; //If true, then the melee weapon is in cooldown (similar to buffer but the user can move)
    private float weaponCoolDownCounter = 0f;
    private bool meleeWeaponCanBeSwung = false;

    private Vector3 PlayPos;

    private bool rangedWeaponActive = false;
    private float rangedWeaponBuffer = 0f; //Simpler than ranged weapon, 0 = ready to shoot, not 0 = not ready to shoot
    private bool rangedCoolDown = true; //If true, then the ranged weapon is in cooldown
    private float rangedCoolDownCounter = 0f;

    public GameObject bullet;
    public GameObject player;
    public GameObject legs;
    public string torsoName;
    public float Mana = 30f;
    public int stallVar = 0;

    private int attackMode = 0;


    // Start is called before the first frame update
    void Start()
    {
        rangedCoolDownCounter = (200 - ranged[3] * 10); //Cooldown will be proportional to how good the weapon's damage, speed, size, and AoE is.
        CapsuleCollider2D MRange = MeleeRange.transform.GetComponent<CapsuleCollider2D>();
        MRange.size = new Vector2(melee[2]*5, melee[2]*5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (meleeWeaponActive)
            {
                //Damage player
                Movement enemyScript = collision.gameObject.GetComponent<Movement>();
                enemyScript.HP = enemyScript.HP - melee[0];
            }
            else
            {
                meleeWeaponIsInRange = true;
            }
        }
    }

    private IEnumerator StallAttack()
    {
        if (stallVar <= 0)
        {
            stallVar++;
            //Create a red circle with the hitbox dimensions.
            PlayPos = player.transform.position - transform.position;
            PlayPos.Normalize();
            GameObject hitBoxVisual = Instantiate(transform.GetChild(1).gameObject, transform.localPosition, Quaternion.identity) as GameObject;
            hitBoxVisual.transform.SetParent(transform);
            Vector2 wepBoxDimensions = Vector2.zero;
            Vector2 wepBoxLocation = transform.position + (PlayPos * (melee[2] / 2));
            hitBoxVisual.transform.position = wepBoxLocation;
            wepBoxDimensions.x = melee[2]*5;
            wepBoxDimensions.y = melee[3]*5;
            hitBoxVisual.transform.localScale = wepBoxDimensions;
            hitBoxVisual.transform.right = PlayPos;

            yield return new WaitForSeconds(1);

            Destroy(hitBoxVisual);
            meleeWeaponCanBeSwung = true;
            stallVar--;
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        EnemyMovementScript moveScript = legs.GetComponent<EnemyMovementScript>();

        if (torsoName != "Worm" && !meleeWeaponCanBeSwung && meleeWeaponIsInRange)
        {
            StartCoroutine(StallAttack());
        }

        if (torsoName != "Worm" && meleeWeaponCanBeSwung && meleeWeaponIsInRange && !meleeWeaponActive && !weaponCoolDown && !rangedWeaponActive)
        {
            string slashAnimName = "Slash";
            //swingAudio.Play();
            meleeWeaponActive = true;
            weaponCoolDown = true;

            weaponCoolDownCounter = (100 - (melee[4] * 5)); //Cooldown for melee attacks will be proportional to the weapon's damage, speed, and size

            if (melee[2] > melee[3])
            {
                MeleeRange.GetComponent<CapsuleCollider2D>().direction = UnityEngine.CapsuleDirection2D.Horizontal;
                slashAnimName = "Pierce";
            }
            else
            {
                MeleeRange.GetComponent<CapsuleCollider2D>().direction = UnityEngine.CapsuleDirection2D.Vertical;
            }

            //Creates a new collider which acts as a hitbox. The script will get the mouse's position in order to place the hitbox correctly.
            Vector2 wepBoxLocation = transform.position + (PlayPos * (melee[2]/2));
            MeleeRange.transform.position = wepBoxLocation;
            MeleeRange.transform.right = PlayPos;

            MeleeRange.GetComponent<CapsuleCollider2D>().enabled = true;
            CapsuleCollider2D MRange = MeleeRange.transform.GetComponent<CapsuleCollider2D>();
            Vector2 wepBoxDimensions = Vector2.zero;
            wepBoxDimensions.x = melee[2]*5;
            wepBoxDimensions.y = melee[3]*5;
            MRange.size = wepBoxDimensions;

            //Slash visual effect
            GameObject clone = Instantiate(slashAnimHandler, wepBoxLocation, Quaternion.identity) as GameObject;
            clone.transform.parent = MeleeRange.transform;
            clone.transform.position = MeleeRange.transform.position;
            clone.transform.right = MeleeRange.transform.right;
            clone.transform.localScale = new Vector3((0.2f*MRange.size.x * 1.25f), 0.1f * (MRange.size.y * 1.25f), 0f);
            if (PlayPos.x < 0f)
            {
                clone.transform.localScale = new Vector3((0.2f*MRange.size.x * 1.25f), -0.1f * (MRange.size.y * 1.25f), 0f);
            }

            slashTester slashScript = clone.GetComponent<slashTester>();
            slashScript.maxValFS = (25) - melee[1];
            slashScript.spriteName = slashAnimName;
            if (slashAnimName == "Pierce")
            {
                clone.transform.localScale = new Vector3(0.2f * (MRange.size.x * 1.1f), 0.6f * (MRange.size.y * 1.1f), 0f);
                clone.transform.position = MeleeRange.transform.position;// + (PlayPos * MRange.size.x * 0.5f);
            }
        }
        else if (meleeWeaponActive)
        {
            if (weaponSwingBuffer == 0) //We're done swinging, enter cooldown.
            {
                MeleeRange.transform.GetComponent<CapsuleCollider2D>().enabled = false;
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
            if (weaponCoolDownCounter <= 0)
            {
                CapsuleCollider2D MRange = MeleeRange.transform.GetComponent<CapsuleCollider2D>();
                weaponCoolDown = false; //no longer in cooldown
                MRange.enabled = true;
                meleeWeaponIsInRange = false;
                MRange.offset = new Vector2(0f, 0f);
                MRange.size = new Vector2(melee[2]*5, melee[2]*5);
                meleeWeaponCanBeSwung = false;
                MeleeRange.transform.position = transform.position;
            }
            else
            {
                weaponCoolDownCounter--;
            }
        }


        if (torsoName != "Slime" && gameObject && !rangedWeaponActive && !rangedCoolDown)
        {
            if (attackMode == 0)
            {
                rangedCoolDown = true;
                rangedCoolDownCounter = (200 - ranged[3] * 10); //Cooldown will be proportional to how good the weapon's damage, speed, size, and AoE is.

                //Script below will spawn a projectile, which will be its own entity
                for (int i = 0; i < ranged[6]; i++)
                {
                    Mana = (Mana - ranged[0]);
                    GameObject clone = Instantiate(bullet, transform.localPosition, Quaternion.identity) as GameObject;
                    ProjectileScript projectileStats = clone.GetComponent<ProjectileScript>();

                    var mouseWorldPos = player.transform.position;
                    mouseWorldPos.z = 0f; // zero z
                    projectileStats.destination = mouseWorldPos;
                    projectileStats.parent = gameObject;

                    projectileStats.projStats[0] = ranged[0]/3;
                    for (int j = 1; j < 7; j++)
                    {
                        if (j == 5)
                        {
                            projectileStats.projStats[j] = ranged[j] / 25;
                        }
                        else
                        {
                            projectileStats.projStats[j] = ranged[j];
                        }
                    }
                }
                if (Mana <= 0)
                {
                    attackMode = 1;
                }
            }
            else
            {
                Mana = Mana + 0.1f;
                if (Mana >= 30)
                {
                    attackMode = 0;
                }
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
