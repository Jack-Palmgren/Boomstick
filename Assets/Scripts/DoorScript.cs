using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject OTHER_SIDE;
    public bool KEY = false; //If key is true, the player can use the door.
    public bool CAN_WALK = true; //Set to false when the player just used a door to prevent no debounce
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (KEY && CAN_WALK && collision.tag == "Player")
        {
            //Teleport player to other door and set other door's CAN_WALK to false
            DoorScript script = OTHER_SIDE.GetComponent<DoorScript>();
            script.CAN_WALK = false;
            collision.transform.position = OTHER_SIDE.transform.position;
        }
        CAN_WALK = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        CAN_WALK = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
