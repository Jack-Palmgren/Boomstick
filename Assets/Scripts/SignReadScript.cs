using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.U2D.Animation;

//This is a basic script meant to hold values for the dropped weapon.
public class SignReadScript : MonoBehaviour
{
    //Damage; Swing Speed; Hitbox distance; Hitbox width; Cooldown
    public float[] MeleeStats = { 10f, 5f, 1f, 3f , 2f};
    public GameObject player;
    public GameObject weapon;
    private bool playerIsColliding = false;
    public string textToDisplay = "...";


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.name == "Player") && (collision.isTrigger == false))
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraScript camScript = camera.gameObject.GetComponent<CameraScript>();
            camScript.textBoxText = textToDisplay;
            playerIsColliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraScript camScript = camera.gameObject.GetComponent<CameraScript>();
            camScript.textBoxText = " ";
            playerIsColliding = false;
        }
    }
}
