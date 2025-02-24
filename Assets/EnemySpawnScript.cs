using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class EnemySpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject torsoArray;
    [SerializeField] private GameObject legsArray;
    
    // Start is called before the first frame update
    void Start()
    {
        //Use a randomizer on two folders, one containing legs and the other containing torsos, then pair them together and spawn them in the same location as the spawner.
        //There'll be multiple spawners for different enemies of higher difficulty
    }
}
