using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour
{
    public GameObject target;
    public GameObject torso;
    public bool canMove = true; //Since the torso will handle attacks, the torso will tell the legs if they can move or not.
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
        Vector3 movementVector = Vector3.zero;
        Vector3 additive = Vector3.zero;
        additive.y = 0.5f;

        if (HP <= 0)
        {
            //Enter dying animation

            //Drops their item (1-x chance to do so)

            //Dies
            Destroy(torso);
            Destroy(gameObject);
        }
        else if (canMove == true)
        {
            movementVector = target.transform.position - gameObject.transform.position;
            movementVector.Normalize();
        }
        rb.velocity = movementVector * 1f;
        torso.transform.position = gameObject.transform.position;
        torso.transform.position = torso.transform.position + additive;

    }
}
