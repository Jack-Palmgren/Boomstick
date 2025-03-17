using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject bullet;
    [SerializeField] private CapsuleCollider2D capCollide;
    public GameObject parent;

    public float[] projStats = { 1f, 1f, 1f, 1f, 1f, 1f, 1f };
    private Vector3 reSize;
    private float rangedDespawnCounter = 1000f; //Flies for 1000 frames, then despawns
    private bool isExploding = false; //is true when the bullet is exploding
    private string opponent;

    public Vector3 destination = Vector3.zero;

    private Vector2 mvel = Vector2.zero;
    void Start()
    {
        if (parent.tag == "Player")
        {
            opponent = "Enemy";
        }
        else
        {
            opponent = "Player";
        }
        Vector2 xVal = new Vector2(UnityEngine.Random.Range(-projStats[5], projStats[5]), 0);
        Vector2 yVal = new Vector2(0, UnityEngine.Random.Range(-projStats[5], projStats[5]));

        transform.right = transform.position - destination; //faces the target location
        transform.right = transform.right + new Vector3(xVal.x, yVal.y, 0f);

        mvel = transform.position - destination; //mvel, AKA movement velocity
        mvel.Normalize();
        mvel = mvel + xVal + yVal;
        mvel.Normalize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isExploding)
        {
            reSize.x = projStats[2] * 0.3f;
            reSize.y = projStats[2] * 0.1f;
            rb.transform.localScale = reSize;
            rb.velocity = -mvel * (projStats[1] * 2);
        }
        if (rangedDespawnCounter <= 0)
        {
            TriggerExplosion();
            Destroy(gameObject);
        }
        rangedDespawnCounter = rangedDespawnCounter - (1f * projStats[1]);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == opponent)
        {
            if (!isExploding)
            {
                TriggerExplosion();
            }
            else
            {
                if (opponent == "Enemy")
                {
                    EnemyMovementScript enemyScript = collision.gameObject.GetComponent<EnemyMovementScript>();
                    enemyScript.HP = enemyScript.HP - projStats[0];
                }
                else if (collision.isTrigger == false)
                {
                    Movement enemyScript = collision.gameObject.GetComponent<Movement>();
                    enemyScript.HP = enemyScript.HP - projStats[0];
                }
                Destroy(gameObject);

            }
        }
    }

    //This function, once called, will have the bullet object explode depending on how large the AoE is.
    private void TriggerExplosion()
    {
        isExploding = true;
        Vector2 explosionRadius = Vector2.zero;
        explosionRadius.x = projStats[4];
        explosionRadius.y = projStats[4];
        capCollide.size = explosionRadius;
        capCollide.enabled = false;
        capCollide.enabled = true; //"reset" the collider, so if an enemy triggers the explosion they won't be immune.

        //Trigger explosion animation before destroying the game object
        
        
    }
}
