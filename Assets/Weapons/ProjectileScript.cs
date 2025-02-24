using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject bullet;
    [SerializeField] private CapsuleCollider2D capCollide;
    public float[] projStats = { 1f, 1f, 1f, 1f, 1f };
    private Vector3 reSize;
    private float rangedDespawnCounter = 1000f; //Flies for 1000 frames, then despawns
    private bool isExploding = false; //is true when the bullet is exploding

    public Vector3 destination = Vector3.zero;

    private Vector2 mvel = Vector2.zero;
    void Start()
    {
        transform.right = transform.position - destination; //faces the target location
        mvel = transform.position - destination; //mvel, AKA movement velocity
        mvel.Normalize();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isExploding)
        {
            reSize.x = projStats[2] * 0.3f;
            reSize.y = projStats[2] * 0.1f;
            rb.transform.localScale = reSize;
            rb.velocity = -mvel * (projStats[1] * 2);
        }
        if (rangedDespawnCounter == 0)
        {
            TriggerExplosion();
            Destroy(gameObject);
        }
        rangedDespawnCounter = rangedDespawnCounter - (1f * projStats[1]);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (!isExploding)
            {
                TriggerExplosion();
            }
            else
            {
                EnemyMovementScript enemyScript = collision.gameObject.GetComponent<EnemyMovementScript>();
                enemyScript.HP = enemyScript.HP - projStats[0];
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
