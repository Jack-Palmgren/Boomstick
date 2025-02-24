using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //Melee stats similar to the player for maximum enemy configuration. There'll probably be preset attacks for each enemy torso
    public float[] melee = {1f, 1f, 1f, 1f};

    //Ranged stats similar to the player. Ranged options will probably only be given to a select few enemy torso's, depending on how many enemies we want.
    public float[] ranged = {1f, 1f, 1f, 1f};


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
