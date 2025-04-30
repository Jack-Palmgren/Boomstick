using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
//using UnityEngine.WSA;

public class EnemySpawnScript : MonoBehaviour
{
    public GameObject Camera;
    public GameObject[] meleeWepFolder;
    public GameObject[] rangedWepFolder;
    public GameObject[] torsoPool;
    public GameObject[] legsPool;
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject legs;
    [SerializeField] private GameObject player;
    private GameObject cloneTorso;
    private GameObject cloneLegs;

    // Start is called before the first frame update
    void Start()
    {
        torso = torsoPool[Random.Range(0,torsoPool.Length)];
        legs = legsPool[Random.Range(0, legsPool.Length)];

        player = GameObject.FindGameObjectWithTag("Player");
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        GameScript script = Camera.GetComponent<GameScript>();
        script.ENEMIES_REMAINING = script.ENEMIES_REMAINING + 1;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Triggered by: " + collision.gameObject);
        if (collision.gameObject.name == "Player")
        {
            ////////////
            ////////////BELOW IS WEAPON ROLL FUNCTION
            ////////////
            ///
            int randomMeleeWep = Random.Range(0, meleeWepFolder.Length);

            cloneLegs = Instantiate(legs, legs.transform.localPosition, Quaternion.identity) as GameObject;
            cloneTorso = Instantiate(torso, torso.transform.localPosition, Quaternion.identity) as GameObject;
            EnemyMovementScript movement = cloneLegs.GetComponent<EnemyMovementScript>();
            movement.target = GameObject.Find("Player");
            movement.torso = cloneTorso;
            movement.meleeWeapon = meleeWepFolder[randomMeleeWep];
            cloneLegs.transform.position = transform.position;

            //Torso below; adjust damage values to give a random weapon
            EnemyScript attackScript = cloneTorso.GetComponent<EnemyScript>();
            MeleeDropScript weaponToCopy = meleeWepFolder[randomMeleeWep].GetComponent<MeleeDropScript>();
            attackScript.meleeName = weaponToCopy.name;
            attackScript.player = player;
            attackScript.legs = cloneLegs;

            //Roll for any enchantments. change the name accordingly and edit the stats accordingly
            string nameHold = attackScript.meleeName;
            attackScript.meleeName = "";
            int buffCounter = 0;
            for (int i = 0; i < weaponToCopy.MeleeStats.Length; i++)
            {
                float statChange = 0;
                int canEnchant = AdjustStatsRanged(0, 4);
                //Change name and stats below
                if (i == 0 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-8, 8);
                    if ((weaponToCopy.MeleeStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy.MeleeStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.meleeName += "Sharp "; buffCounter++; }
                    if (statChange < 0) { attackScript.meleeName += "Rusted "; buffCounter--; }
                }
                if (i == 1 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-10, 10);
                    if ((weaponToCopy.MeleeStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy.MeleeStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.meleeName += "Fast "; buffCounter++; }
                    if (statChange < 0) { attackScript.meleeName += "Slow "; buffCounter--; }
                }
                if (i == 2 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-10, 10);
                    if ((weaponToCopy.MeleeStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy.MeleeStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.meleeName += "Long "; buffCounter++; }
                    if (statChange < 0) { attackScript.meleeName += "Short "; buffCounter--; }
                }
                if (i == 3 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-3, 3);
                    if ((weaponToCopy.MeleeStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy.MeleeStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.meleeName += "Large "; buffCounter++; }
                    if (statChange < 0) { attackScript.meleeName += "Small "; buffCounter--; }
                }
                if (i == 4 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-10, 10);
                    if ((weaponToCopy.MeleeStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy.MeleeStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.meleeName += "Hasty "; buffCounter++; }
                    if (statChange < 0) { attackScript.meleeName += "Heavy "; buffCounter--; }
                }
                
                attackScript.melee[i] += weaponToCopy.MeleeStats[i] + statChange;
            }
            attackScript.meleeName += nameHold;
            attackScript.meleeType = weaponToCopy.weaponType;

            if (buffCounter == 7)
            {
                attackScript.meleeName = "Perfect " + nameHold;
            }
            if (buffCounter == -7)
            {
                attackScript.meleeName = "Horrible " + nameHold;
            }

            //Use a randomizer on two folders, one containing legs and the other containing torsos, then pair them together and spawn them in the same location as the spawner.
            //There'll be multiple spawners for different enemies of higher difficulty
            int randomRangedWep = Random.Range(0, rangedWepFolder.Length);

            movement.rangedWeapon = rangedWepFolder[randomRangedWep];
            cloneLegs.transform.position = transform.position;

            //Torso below; adjust damage values to give a random weapon
            RangedDrop weaponToCopy2 = rangedWepFolder[randomRangedWep].GetComponent<RangedDrop>();
            attackScript.rangedName = weaponToCopy2.name;
            attackScript.player = player;

            //Roll for any enchantments. change the name accordingly and edit the stats accordingly
            nameHold = attackScript.rangedName;
            attackScript.rangedName = "";
            int buffCounter2 = 0;
            for (int i = 0; i < weaponToCopy2.RangedStats.Length; i++)
            {
                float statChange = 0;
                int canEnchant = AdjustStatsRanged(0, 4);
                //Change name and stats below
                if (i == 0 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-8, 8);
                    if ((weaponToCopy2.RangedStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy2.RangedStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.rangedName += "Strong "; buffCounter2++; }
                    if (statChange < 0) { attackScript.rangedName += "Weak "; buffCounter2--; }
                }
                if (i == 1 && canEnchant == 1)
                {
                    if ((weaponToCopy2.RangedStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy2.RangedStats[i] - 1) * -1;
                    }
                    statChange = AdjustStatsRanged(-10, 10);
                    if (statChange > 0) { attackScript.rangedName += "Fast "; buffCounter2++; }
                    if (statChange < 0) { attackScript.rangedName += "Slow "; buffCounter2--; }
                }
                if (i == 2 && canEnchant == 1)
                {
                    if ((weaponToCopy2.RangedStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy2.RangedStats[i] - 1) * -1;
                    }
                    statChange = AdjustStatsRanged(-3, 3);
                    if (statChange > 0) { attackScript.rangedName += "Large "; buffCounter2++; }
                    if (statChange < 0) { attackScript.rangedName += "Small "; buffCounter2--; }
                }
                if (i == 3 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-10, 10);
                    if ((weaponToCopy2.RangedStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy2.RangedStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.rangedName += "Rapid "; buffCounter2++; }
                    if (statChange < 0) { attackScript.rangedName += "Sluggish "; buffCounter2--; }
                }
                if (i == 4 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-10, 10);
                    if ((weaponToCopy2.RangedStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy2.RangedStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.rangedName += "Explosive "; buffCounter2++; }
                    if (statChange < 0) { attackScript.rangedName += "Non-explosive "; buffCounter2--; }
                }
                if (i == 5 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-10, 10);
                    if ((weaponToCopy2.RangedStats[i] + statChange) < 0)
                    {
                        statChange = (weaponToCopy2.RangedStats[i]) * -1;
                    }
                    statChange = statChange * -1;
                    if (statChange < 0) { attackScript.rangedName += "Accurate "; buffCounter2++; }
                    if (statChange > 0) { attackScript.rangedName += "Inaccurate "; buffCounter2--; }
                }
                if (i == 6 && canEnchant == 1)
                {
                    statChange = AdjustStatsRanged(-10, 10);
                    if ((weaponToCopy2.RangedStats[i] + statChange) < 1)
                    {
                        statChange = (weaponToCopy2.RangedStats[i] - 1) * -1;
                    }
                    if (statChange > 0) { attackScript.rangedName += "Barraging "; buffCounter2++; }
                    if (statChange < 0) { attackScript.rangedName += "Lessened "; buffCounter2--; }
                }
                attackScript.ranged[i] += weaponToCopy2.RangedStats[i] + statChange;
            }
            attackScript.rangedName += nameHold;
            attackScript.rangedType = weaponToCopy2.rangedType;

            if (buffCounter2 == 7)
            {
                attackScript.rangedName = "Perfect " + nameHold;
            }
            if (buffCounter2 == -7)
            {
                attackScript.rangedName = "Horrible " + nameHold;
            }

            Destroy(gameObject);
        }
    }

    private int AdjustStatsRanged(int min, int max)
    {
        int randomStatChange = Random.Range(min, max);
        return randomStatChange;
    }
}
