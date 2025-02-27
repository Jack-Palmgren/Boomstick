using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;

//This is a basic script meant to hold values for the dropped weapon.
public class MeleeDropScript : MonoBehaviour
{
    //Damage; Swing Speed; Hitbox distance; Hitbox width
    public float[] MeleeStats = { 10f, 5f, 1f, 3f };
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject weapon;
    private bool playerIsColliding = false;
    private string weaponName = "Iron Pummle";


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraScript camScript = camera.gameObject.GetComponent<CameraScript>();
            camScript.textBoxText = "Press 'E' To Equip " + weaponName;
            playerIsColliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraScript camScript = camera.gameObject.GetComponent<CameraScript>();
            camScript.textBoxText = "";
            playerIsColliding = false;
        }
    }

    void Update()
    {
        if (playerIsColliding && Input.GetKey(KeyCode.E))
        {
            Movement meleeScript = player.GetComponent<Movement>();
            meleeScript.meleeName = weaponName;
            for (int i = 0; i < 4; i++)
            {
                meleeScript.melee[i] = MeleeStats[i];
            }
            Destroy(weapon);
        }
    }
}
