using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedDrop : MonoBehaviour
{
    //Damage; Bullet Speed, Rate of Fire, Projectile Size, AoE, Spread, Amount
    public float[] RangedStats = { 2f, 10f, 10f, 2f, 3f, 0.5f, 3f };
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject weapon;
    private bool playerIsColliding = false;
    private string weaponName = "Wooden Wand";


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerIsColliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerIsColliding = false;
        }
    }

    void Update()
    {
        if (playerIsColliding && Input.GetKey(KeyCode.E))
        {
            Movement rangedScript = player.GetComponent<Movement>();
            //rangedScript.meleeName = weaponName;
            for (int i = 0; i < 7; i++)
            {
                rangedScript.ranged[i] = RangedStats[i];
            }
            Destroy(weapon);
        }
    }
}
