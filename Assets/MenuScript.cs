using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject menuBody;
    [SerializeField] GameObject player;
    private bool isMenuOpen = false;
    private float xPosForOpenMenu = -10.7f;
    private float xPosForClosedMenu = -17.917f;
    private bool isMenuMoving = false;
    private float goalPos;
    private float menuSpeed = 50;
    private float incrementer;

    private bool isChangingKeys = false;
    
    // Start is called before the first frame update
    void Start()
    {
        goalPos = xPosForClosedMenu;
    }

    public void ToggleMenu()
    {
        if (!isMenuMoving)
        {
            isMenuMoving = true;
            if (isMenuOpen) //If the menu is open, 
            {
                isMenuOpen = false;
                goalPos = xPosForClosedMenu - xPosForOpenMenu;
            }
            else
            {
                isMenuOpen = true;
                goalPos = xPosForOpenMenu - xPosForClosedMenu;
            }
            goalPos = goalPos / menuSpeed;
            incrementer = menuSpeed;
        }
    }

    public void exitButton()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ToggleStats()
    {
        if (gameObject.transform.GetChild(6).transform.localScale.y == 0)
        {
            gameObject.transform.GetChild(6).transform.localScale = new Vector3(1f,1f,0f);
        }
        else
        {
            gameObject.transform.GetChild(6).transform.localScale = new Vector3(1f, 0f, 0f);
        }
        Transform childTransform = gameObject.transform.GetChild(6);
        GameObject childGameObject = childTransform.gameObject;
        Movement playerScript = player.GetComponent<Movement>();
        Transform anotherChild = childTransform.GetChild(0);

        for (int i = 0; i < playerScript.melee.Length; i++)
        {
            Transform childofchild = anotherChild.GetChild(i);
            Transform barTrans = childofchild.GetChild(1);

            barTrans.localScale = new Vector3((playerScript.melee[i]/25f)*4.5f, 0.65f, 0f);
        }
        anotherChild = childTransform.GetChild(1);
        for (int i = 0; i < playerScript.ranged.Length; i++)
        {
            Transform childofchild = anotherChild.GetChild(i);
            Transform barTrans = childofchild.GetChild(1);

            barTrans.localScale = new Vector3((playerScript.ranged[i] / 25f) * 4.5f, 0.65f, 0f);
        }

    }

    public void ChangeKeyCode(TextMeshProUGUI textToChange) //this function will be called whenever the player sets their own keys.
    {
        textToChange.text = "...";
    }


    public void ChangeMelee()
    {
        Transform meleeButtonTransform = gameObject.transform.GetChild(7).GetChild(0).GetChild(0).GetChild(0);
        TextMeshProUGUI meleeButtonText = meleeButtonTransform.GetComponent<TextMeshProUGUI>();
        ChangeKeyCode(meleeButtonText);
    }

    public void ToggleSettings()
    {
        if (gameObject.transform.GetChild(7).transform.localScale.y == 0)
        {
            gameObject.transform.GetChild(7).transform.localScale = new Vector3(1f, 1f, 0f);
        }
        else
        {
            gameObject.transform.GetChild(7).transform.localScale = new Vector3(1f, 0f, 0f);
        }
    }

    public InputField inputField;

    // Update is called once per frame
    void Update()
    {
        //string inputText = inputField.text;
        //print(inputText);
        if (isMenuMoving)
        {
            menuBody.transform.position = menuBody.transform.position + new Vector3(goalPos,0f,0f);
            incrementer--;
            if (incrementer <= 0)
            {
                isMenuMoving = false;
            }
        }
    }
}
