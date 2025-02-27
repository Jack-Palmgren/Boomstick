using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class EnemySpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject legs;
    [SerializeField] private GameObject player;
    private GameObject cloneTorso;
    private GameObject cloneLegs;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Triggered by: " + collision.gameObject);
        if (collision.gameObject.tag == "Player")
        {
            //Use a randomizer on two folders, one containing legs and the other containing torsos, then pair them together and spawn them in the same location as the spawner.
            //There'll be multiple spawners for different enemies of higher difficulty

            cloneLegs = Instantiate(legs, legs.transform.localPosition, Quaternion.identity) as GameObject;
            cloneTorso = Instantiate(torso, torso.transform.localPosition, Quaternion.identity) as GameObject;
            EnemyMovementScript movement = cloneLegs.GetComponent<EnemyMovementScript>();
            movement.target = GameObject.Find("Player");
            movement.torso = cloneTorso;
            cloneLegs.transform.position = transform.position;

            EnemyScript attackScript = cloneTorso.GetComponent<EnemyScript>();
            attackScript.legs = cloneLegs;

            Destroy(gameObject);
        }
    }

}
