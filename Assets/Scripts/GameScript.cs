using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public int ROOM_NUMBER = 0;
    public int ENEMIES_REMAINING = 0;
    public int MAX_ROOM_COUNT = 10;
    public GameObject[] ROOMS = { };
    public GameObject CURRENT_ROOM = null;

    //Called by the enemy script when the enemy dies.
    public void DECREMENT_ENEMIES()
    {
        ENEMIES_REMAINING--;
        if (ENEMIES_REMAINING <= 0)
        {
            LEVEL_LOADER();
        }
    }
    public void LEVEL_LOADER()
    {
        //Grab a random room from the array and load it a decent distance to the right of the previous room.
        int randomRoom = UnityEngine.Random.Range(0, ROOMS.Length);
        GameObject cloneRoom = Instantiate(ROOMS[randomRoom], CURRENT_ROOM.transform.localPosition + new Vector3(200,0,0), Quaternion.identity) as GameObject;

        //Unlock the door in the current room and give the player a certain amount of HP
        Transform currentExitDoor = CURRENT_ROOM.transform.GetChild(1);
        DoorScript currentExitScript = currentExitDoor.GetComponent<DoorScript>();
        currentExitScript.KEY = true;

        Transform newEnterDoor = cloneRoom.transform.GetChild(0);
        DoorScript newEnterScript = newEnterDoor.GetComponent<DoorScript>();
        newEnterScript.KEY = true;

        //Connect the exit door with the enterance door of the next room
        currentExitScript.OTHER_SIDE = newEnterScript.GameObject();
        newEnterScript.OTHER_SIDE = currentExitScript.GameObject();

        //Put down a gameobject with an onentered event listener, so when the player walks near the door they teleport to the next room
        ROOM_NUMBER++;
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        Movement movement = Player.GetComponent<Movement>();
        movement.HP = 20;

        CURRENT_ROOM = cloneRoom;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
