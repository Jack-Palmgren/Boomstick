using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour
{
    public GameObject Camera;
    public GameObject meleeWeapon;
    public GameObject rangedWeapon;
    public int dropChance_1_in_X;

    public GameObject target;
    public GameObject torso;
    public string legType;
    public bool canMove = true; //Since the torso will handle attacks, the torso will tell the legs if they can move or not.
    [SerializeField] private Rigidbody2D rb;

    private float canBabyMove = 100;

    //Below are general stats given to each enemy. The HP will probably be a combination of torso and leg values, while speed will strictly be leg values.
    public float HP = 20f;
    private float MAXHP = 20f;
    public float Speed = 5f;
    private GameObject HPBar;

    // Start is called before the first frame update
    void Start()
    {
        MAXHP = HP;
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        HPBar = transform.GetChild(0).GetChild(1).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HPBar.transform.localScale = new Vector3((4.5f * HP) / MAXHP, HPBar.transform.localScale.y, 0f);
        HPBar.transform.localPosition = new Vector3(transform.GetChild(0).GetChild(0).localPosition.x - ((4.5f - HPBar.transform.localScale.x) / 2), 0f, 0f);
        if (HP <= 0)
        {
            //Enter dying animation

            //Drops their item (1-x chance to do so)
            EnemyScript enemyScript = torso.GetComponent<EnemyScript>();
            if (enemyScript.torsoName == "Worm")
            {
                if (Random.Range(1, (float)dropChance_1_in_X) == 1)
                {
                    GameObject clone = Instantiate(rangedWeapon, transform.position, Quaternion.identity) as GameObject;
                    RangedDrop dropScript = clone.GetComponent<RangedDrop>();
                    for (int i = 0; i < dropScript.RangedStats.Length; i++)
                    {
                        dropScript.RangedStats[i] = enemyScript.ranged[i];
                    }
                    dropScript.weaponName = enemyScript.rangedName;
                    dropScript.rangedType = enemyScript.rangedType;
                    dropScript.player = target;
                }
            }
            if (enemyScript.torsoName == "Slime")
            {
                if (Random.Range(1, (float)dropChance_1_in_X) == 1)
                {
                    GameObject clone = Instantiate(meleeWeapon, transform.position, Quaternion.identity) as GameObject;
                    MeleeDropScript dropScript = clone.GetComponent<MeleeDropScript>();
                    for (int i = 0; i < dropScript.MeleeStats.Length; i++)
                    {
                        dropScript.MeleeStats[i] = enemyScript.melee[i];
                    }
                    dropScript.weaponName = enemyScript.meleeName;
                    dropScript.weaponType = enemyScript.meleeType;
                    dropScript.player = target;
                }
            }
            if (enemyScript.torsoName == "Human")
            {
                if (Random.Range(1, 3) == 1)
                {
                    if (Random.Range(1, (float)dropChance_1_in_X) == 1)
                    {
                        GameObject clone = Instantiate(meleeWeapon, transform.position, Quaternion.identity) as GameObject;
                        MeleeDropScript dropScript = clone.GetComponent<MeleeDropScript>();
                        for (int i = 0; i < dropScript.MeleeStats.Length; i++)
                        {
                            dropScript.MeleeStats[i] = enemyScript.melee[i];
                        }
                        dropScript.weaponName = enemyScript.meleeName;
                        dropScript.weaponType = enemyScript.meleeType;
                        dropScript.player = target;
                    }
                }
                else
                {
                    if (Random.Range(1, (float)dropChance_1_in_X) == 1)
                    {
                        GameObject clone = Instantiate(rangedWeapon, transform.position, Quaternion.identity) as GameObject;
                        RangedDrop dropScript = clone.GetComponent<RangedDrop>();
                        for (int i = 0; i < dropScript.RangedStats.Length; i++)
                        {
                            dropScript.RangedStats[i] = enemyScript.ranged[i];
                        }
                        dropScript.weaponName = enemyScript.rangedName;
                        dropScript.rangedType = enemyScript.rangedType;
                        dropScript.player = target;
                    }
                }
            }
            //Decrement ENEMIES_ALIVE in GameScript
            GameScript script = Camera.GetComponent<GameScript>();
            script.DECREMENT_ENEMIES();

            //Dies
            Destroy(torso);
            Destroy(gameObject);
        }
        else if (canMove == true)
        {
            if (legType == "Spider")
            {
                SpiderMovement();
            }
            else if (legType == "Bee")
            {
                BeeMovement();
            }
            else
            {
                BabyMovement();
            }
        }
    }

    void BabyMovement()
    {
        if (canBabyMove >= 100)
        {
            rb.velocity = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
            canBabyMove = 0;
        }
        else
        {
            canBabyMove = canBabyMove + 0.5f;
        }
        Vector3 additive = Vector3.zero;
        additive.y = 0f;
        torso.transform.position = gameObject.transform.position + additive;
    }
    void SpiderMovement()
    {
        Vector3 movementVector = Vector3.zero;
        Vector3 additive = Vector3.zero;
        additive.y = 0.5f;

        movementVector = target.transform.position - gameObject.transform.position;
        movementVector.Normalize();
        rb.velocity = movementVector * 1f;
        torso.transform.position = gameObject.transform.position + additive;
    }
    void BeeMovement()
    {
        Vector3 movementVector = Vector3.zero;
        Vector3 additive = Vector3.zero;
        additive.y = 0.5f;

        movementVector = target.transform.position - gameObject.transform.position;
        movementVector.Normalize();
        rb.velocity = movementVector * 4f;
        torso.transform.position = gameObject.transform.position + additive;
    }
}
