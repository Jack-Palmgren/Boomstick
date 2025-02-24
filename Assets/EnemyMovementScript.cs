using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject torso;
    [SerializeField] private Rigidbody2D rb;

    //Below are general stats given to each enemy. The HP will probably be a combination of torso and leg values, while speed will strictly be leg values.
    public float HP = 20f;
    public float Speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            //Enter dying animation

            //Dies
            Destroy(torso);
            Destroy(gameObject);
        }
        else
        {
            Vector3 additive = Vector3.zero;
            additive.y = 0.5f;
            Vector3 movementVector = target.transform.position - gameObject.transform.position;
            movementVector.Normalize();

            rb.velocity = movementVector * 1f;
            torso.transform.position = gameObject.transform.position;
            torso.transform.position = torso.transform.position + additive;
        }
     
    }
}
