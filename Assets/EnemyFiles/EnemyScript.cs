using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //melee weapon stats, such as damage, swing speed, hurtbox distance (this number is higher if weapon is spear), hurtbox width (this number is higher if weapon is sword)
    public float[] melee = { 1f, 1f, 1f, 1f };
    public string meleeName = "N/A";

    //ranged weapon stats, such as damage, projectile speed, projectile size, rate of fire, AoE, Spread, Amount (how many bullets fire per shot, like a shotgun)
    public float[] ranged = { 1f, 1f, 1f, 1f, 1f, 1f, 1f };
    public string rangedName = "N/A";

    /*private bool meleeWeaponActive = false; //if true, then the player is currently swinging their main weapon. False if otherwise.
    private float weaponSwingBuffer = -1f; //Time it takes for you to swing again. (=0 means attack finished, =-1 means attack is ready
    private bool weaponCoolDown = false; //If true, then the melee weapon is in cooldown (similar to buffer but the user can move)
    private float weaponCoolDownCounter = 0f;*/

    private bool rangedWeaponActive = false;
    private float rangedWeaponBuffer = 0f; //Simpler than ranged weapon, 0 = ready to shoot, not 0 = not ready to shoot
    private bool rangedCoolDown = false; //If true, then the ranged weapon is in cooldown
    private float rangedCoolDownCounter = 0f;

    public GameObject bullet;
    public GameObject clone;
    public GameObject player;
    public GameObject legs;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovementScript moveScript = legs.GetComponent<EnemyMovementScript>();
        //Ranged attack script (will eventually be triggered by right click)
        if (gameObject && !rangedWeaponActive && !rangedCoolDown)
        {
            moveScript.canMove = false;

            rangedWeaponBuffer = -1;
            rangedWeaponActive = true;
            rangedCoolDown = true;
            rangedCoolDownCounter = (((ranged[0] + ranged[1] + ranged[2] + ranged[4]) - ranged[3]) * ranged[6])*15; //Cooldown will be proportional to how good the weapon's damage, speed, size, and AoE is.

            //Script below will spawn a projectile, which will be its own "entity"
            for (int i = 0; i < ranged[6]; i++)
            {
                clone = Instantiate(bullet, transform.localPosition, Quaternion.identity) as GameObject;
                ProjectileScript projectileStats = clone.GetComponent<ProjectileScript>();

                var mouseWorldPos = GameObject.FindGameObjectWithTag("Player").transform.position;
                mouseWorldPos.z = 0f; // zero z
                projectileStats.destination = mouseWorldPos;
                projectileStats.parent = gameObject;
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
                moveScript.canMove = true;
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
