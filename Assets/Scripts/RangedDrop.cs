using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RangedDrop : MonoBehaviour
{
    //Damage; Bullet Speed, Rate of Fire, Projectile Size, AoE, Spread, Amount
    public float[] RangedStats = { 2f, 10f, 10f, 2f, 3f, 0.5f, 3f };
    public GameObject player;
    public GameObject weapon;
    private bool playerIsColliding = false;
    public string weaponName = "Wooden Wand";
    public string rangedType = "Basic Wand";

    //Anim variables
    public float maxValFS = 1f;
    private float counter = 0f;
    private float frameSkip = 10f / 8f; //(MaxValFrameskip): Every 10 frames the animation switches to the next image
    public SpriteResolver spriteResolver;
    private string currentAnimName;
    private int isTouchingPlayer = 0;
    void Start()
    {
        spriteResolver = transform.GetChild(0).GetComponent<SpriteResolver>();
        frameSkip = maxValFS;
    }
    void getNPlayAnim(string animName, bool canLoop)
    {
        if (currentAnimName != animName)
        {
            counter = 0;
            frameSkip = maxValFS;
        }
        if (frameSkip <= 0f)
        {
            ChangeSprite(animName, counter);
            counter++;
            if (counter >= 4)
            {
                if (canLoop)
                {
                    counter = 0;
                }
            }
            frameSkip = maxValFS;
        }
        frameSkip--;
        currentAnimName = animName;
    }

    void ChangeSprite(string category, float label)
    {
        if (spriteResolver)
        {
            spriteResolver.SetCategoryAndLabel(category, label.ToString()); // Change dynamically
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.name == "Player") && (collision.isTrigger == false))
        {
            isTouchingPlayer = 1;
            /*GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraScript camScript = camera.gameObject.GetComponent<CameraScript>();
            camScript.textBoxText = "Press 'E' To Equip " + weaponName;*/
            playerIsColliding = true;
            transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = weaponName;

            if (transform.GetChild(0))
            {
                Transform childTransform = gameObject.transform.GetChild(0);
                GameObject childGameObject = childTransform.gameObject;
                Movement playerScript = player.GetComponent<Movement>();
                Transform anotherChild = childTransform.GetChild(0);

                for (int i = 0; i < playerScript.ranged.Length; i++)
                {
                    Transform childofchild = anotherChild.GetChild(i);
                    Transform barTrans = childofchild.GetChild(1);

                    barTrans.localScale = new Vector3((playerScript.ranged[i] / 20f) * 4.5f, 0.65f, 0f);
                    barTrans.localPosition = new Vector3(childofchild.GetChild(0).localPosition.x - ((4.5f - barTrans.localScale.x) / 2), 0f, 0f);
                    barTrans.GetComponent<SpriteRenderer>().color = Color.gray;
                }

                for (int i = 0; i < RangedStats.Length; i++)
                {
                    Transform childofchild = anotherChild.GetChild(i);
                    Transform barTrans = childofchild.GetChild(2);

                    barTrans.localScale = new Vector3((RangedStats[i] / 20f) * 4.5f, 0.65f, 0f);
                    barTrans.localPosition = new Vector3(childofchild.GetChild(0).localPosition.x - ((4.5f - barTrans.localScale.x) / 2), 0f, 0f);

                    barTrans.GetComponent<SpriteRenderer>().color = Color.gray;
                    barTrans.transform.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    if (barTrans.localScale.x > childofchild.GetChild(1).localScale.x)
                    {
                        barTrans.GetComponent<SpriteRenderer>().color = Color.green;
                        barTrans.transform.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    }
                    else if (barTrans.localScale.x < childofchild.GetChild(1).localScale.x) 
                    {
                        barTrans.GetComponent<SpriteRenderer>().color = Color.red;
                        barTrans.transform.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    }
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            isTouchingPlayer = 0;
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraScript camScript = camera.gameObject.GetComponent<CameraScript>();
            camScript.textBoxText = " ";
            playerIsColliding = false;
        }
    }

    void FixedUpdate()
    {
        if (transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name == "OpenScroll")
        {
            transform.GetChild(0).GetChild(0).transform.localScale = new Vector3(1f, 1f, 0f);
        }
        else
        {
            transform.GetChild(0).GetChild(0).transform.localScale = new Vector3(1f, 0f, 0f);
        }

        if (transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name == "ClosedScroll")
        {
            transform.GetChild(0).transform.localScale = new Vector3(3f, 0f, 0f);
        }
        else
        {
            transform.GetChild(0).transform.localScale = new Vector3(3f, 3f, 0f);
        }

        if (isTouchingPlayer == 1)
        {
            getNPlayAnim("ScrollUnwind", false);
        }
        else
        {
            getNPlayAnim("ScrollWind", false);
        }
        if (playerIsColliding && Input.GetKey(KeyCode.E))
        {
            Movement rangedScript = player.GetComponent<Movement>();
            rangedScript.rangedName = weaponName;
            rangedScript.rangedType = rangedType;
            for (int i = 0; i < 7; i++)
            {
                rangedScript.ranged[i] = RangedStats[i];
            }
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<SpriteResolver>().SetCategoryAndLabel("Barrels", rangedType);
            Destroy(weapon);
        }
    }
}
