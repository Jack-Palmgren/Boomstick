using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class slashTester : MonoBehaviour
{
    public float speed = 5f;
    public float maxValFS = 10f;
    private float counter = 0f;
    private float frameSkip; //(MaxValFrameskip): Every 10 frames the animation switches to the next image
    private SpriteResolver spriteResolver;
    private Rigidbody2D rb;

    void Start()
    {
        spriteResolver = GetComponent<SpriteResolver>();
        rb = GetComponent<Rigidbody2D>();
        frameSkip = maxValFS/8;
        //Debug.Log(frameSkip);
    }

    void FixedUpdate()
    {
        if (frameSkip <= 0f)
        {
            ChangeSprite("Slash", counter);
            counter++;
            if (counter > 7)
            {
                counter = 0;
                Destroy(gameObject); //Destroy it after the animation plays
            }
            frameSkip = maxValFS/8;
        }
        frameSkip--;
    }

    void ChangeSprite(string category, float label)
    {
        if (spriteResolver)
        {
            spriteResolver.SetCategoryAndLabel(category, label.ToString()); // Change dynamically
        }
    }
}
