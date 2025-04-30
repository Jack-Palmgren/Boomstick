using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject menuBody;
    [SerializeField] GameObject player;
    private bool isMenuOpen = false;
    private bool isStatsOpen = false;
    private bool isSettingsOpen = false;
    private float xPosForOpenMenu = -10.7f;
    private float xPosForClosedMenu = -17.917f;
    private bool isMenuMoving = false;
    private float goalPos;
    private float menuSpeed = 50;
    private float incrementer;
    private float debounce = 120f;

    private bool isChangingKeys = false;
    private KeyCode meleeInput = KeyCode.Mouse0;
    private KeyCode rangedInput = KeyCode.Mouse1;
    private KeyCode dodgeInput = KeyCode.Space;
    private string MovesetToApply = "Preset1";

    //Audio variables
    private float masterVolume = 1f;
    private float playerVolume = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        goalPos = xPosForClosedMenu;

        //Below is for changing custom keys
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

    public void ExitButton()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public GameObject[] wep;
    public void DEBUG()
    {
        int melee = 0, ranged = 0;
        for (int i=0; i < wep.Length; i++)
        {
            GameObject clone_1 = Instantiate(wep[i], new Vector3(-13f, 2f, 0f), Quaternion.identity);
            if (clone_1.GetComponent<MeleeDropScript>() == null) //Weapon is a ranged drop
            {
                clone_1.transform.position = new Vector3(-13f + (ranged * 2.5f), -1f, 0f);
                RangedDrop rangedDrop = clone_1.GetComponent<RangedDrop>();
                rangedDrop.player = player;
                ranged++;
            }
            else
            {
                clone_1.transform.position = new Vector3(-13f + (melee * 2.5f), 1f, 0f);
                MeleeDropScript rangedDrop = clone_1.GetComponent<MeleeDropScript>();
                rangedDrop.player = player;
                melee++;
            }
        }
    }
    public void ToggleStats()
    {
        if (gameObject.transform.GetChild(2).transform.localScale.y == 0)
        {
            isStatsOpen = true;
            gameObject.transform.GetChild(2).transform.localScale = new Vector3(1f,1f,0f);
            gameObject.transform.GetChild(1).transform.localScale = new Vector3(1f, 0f, 0f);
        }
        else
        {
            isStatsOpen = false;
            gameObject.transform.GetChild(2).transform.localScale = new Vector3(1f, 0f, 0f);
            gameObject.transform.GetChild(1).transform.localScale = new Vector3(1f, 1f, 0f);
        }
        Transform childTransform = gameObject.transform.GetChild(2);
        GameObject childGameObject = childTransform.gameObject;
        Movement playerScript = player.GetComponent<Movement>();
        Transform anotherChild = childTransform.GetChild(0);

        for (int i = 0; i < playerScript.melee.Length; i++)
        {
            Transform childofchild = anotherChild.GetChild(i);
            Transform barTrans = childofchild.GetChild(1);

            barTrans.localScale = new Vector3((playerScript.melee[i]/20f)*4.5f, 0.65f, 0f);
            barTrans.localPosition = new Vector3(childofchild.GetChild(0).localPosition.x - ((4.5f - barTrans.localScale.x) / 2), 0f, 0f);
        }
        anotherChild = childTransform.GetChild(1);
        for (int i = 0; i < playerScript.ranged.Length; i++)
        {
            Transform childofchild = anotherChild.GetChild(i);
            Transform barTrans = childofchild.GetChild(1);

            barTrans.localScale = new Vector3((playerScript.ranged[i] / 20f) * 4.5f, 0.65f, 0f);
            barTrans.localPosition = new Vector3(childofchild.GetChild(0).localPosition.x - ((4.5f - barTrans.localScale.x) / 2), 0f, 0f);
        }

    }

    public void apply()
    {
        if (!isSettingsOpen)
        {
            return;
        }
        Movement playerScript = player.GetComponent<Movement>();
        if (MovesetToApply == "Preset1")
        {
            playerScript.meleeInput = KeyCode.Mouse0;
            playerScript.rangedInput = KeyCode.Mouse1;
            playerScript.dodgeInput = KeyCode.Space;
        }
        else
        {
            playerScript.meleeInput = meleeInput;
            playerScript.rangedInput = rangedInput;
            playerScript.dodgeInput = dodgeInput;
        }
    }
    public void applyPreset1()
    {
        if (!isSettingsOpen)
        {
            return;
        }
        MovesetToApply = "Preset1";
        //Changes color of selected button
        Transform repo = transform.GetChild(3);
        UnityEngine.Color newColor = new Vector4(1, 1, 1, 22 / 255);
        UnityEngine.Color oldColor = new Vector4(1, 1, 1, 0.086f);
        repo.GetChild(1).GetComponent<Image>().color = newColor;
        repo.GetChild(0).GetComponent<Image>().color = oldColor;
    }
    public void applyCustomPreset()
    {
        if (!isSettingsOpen)
        {
            return;
        }
        MovesetToApply = "PresetCustom";
        Transform repo = transform.GetChild(3);
        UnityEngine.Color newColor = new Vector4(1, 1, 1, 22 / 255);
        UnityEngine.Color oldColor = new Vector4(1, 1, 1, 0.086f);
        repo.GetChild(0).GetComponent<Image>().color = newColor;
        repo.GetChild(1).GetComponent<Image>().color = oldColor;
    }

    public void ChangeInput(TextMeshProUGUI button)
    {
        if (!isSettingsOpen)
        {
            return;
        }
        if (button.name == "Melee")
        {
            meleeInput = KeyCode.None;
        }
        if (button.name == "Ranged")
        {
            rangedInput = KeyCode.None;
        }
        if (button.name == "Dodge")
        {
            dodgeInput = KeyCode.None;
        }
        button.text = "Press any key";
        isChangingKeys = true;
    }

    public void SlidercChanged(GameObject slider)
    {
        if (!isSettingsOpen)
        {
            return;
        }
        float volume = slider.GetComponent<Slider>().value;
        if (slider.name == "SliderMaster")
        {
            masterVolume = volume;
            GameObject[] audioArr = GameObject.FindGameObjectsWithTag("Audio");
            for (int i = 0; i < audioArr.Length; i++)
            {
                AudioScript AudioScr = audioArr[i].GetComponent<AudioScript>();
                AudioScr.MasterVolume = volume;
                audioArr[i].GetComponent<AudioSource>().volume = (AudioScr.MasterVolume * 0.08f) * AudioScr.MainVolume;
            }

            AudioSource swingAudio = GameObject.Find("Player").GetComponent<AudioSource>();
            swingAudio.volume = volume * masterVolume;
        } 
        else if (slider.name == "SliderMusic")
        {
            AudioSource audio = GameObject.Find("MainMusic").GetComponent<AudioSource>();
            AudioScript AudioScr = audio.GetComponent<AudioScript>();
            AudioScr.MainVolume = volume;
            audio.GetComponent<AudioSource>().volume = (AudioScr.MasterVolume * 0.08f) * AudioScr.MainVolume;
        } 
        else if (slider.name == "SliderPlayer")
        {
            AudioSource audio = GameObject.Find("footsteps").GetComponent<AudioSource>();
            AudioScript AudioScr = audio.GetComponent<AudioScript>();
            AudioScr.MainVolume = volume;
            audio.GetComponent<AudioSource>().volume = AudioScr.MasterVolume * AudioScr.MainVolume;

            Component[] components = GameObject.Find("Player").GetComponents(typeof(Component));

            foreach (Component component in components)
            {
                if (component.GetType() == typeof(AudioSource))
                {
                    AudioSource swingAudio = (AudioSource)component;
                    swingAudio.volume = volume * masterVolume;
                }
            }
        }
    }
    public void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;
        if (gameObject.transform.GetChild(3).transform.localScale.y == 0)
        {
            gameObject.transform.GetChild(3).transform.localScale = new Vector3(1f, 1f, 0f);
            gameObject.transform.GetChild(1).transform.localScale = new Vector3(1f, 0f, 0f);
        }
        else
        {
            gameObject.transform.GetChild(3).transform.localScale = new Vector3(1f, 0f, 0f);
            gameObject.transform.GetChild(1).transform.localScale = new Vector3(1f, 1f, 0f);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isChangingKeys == true)
        {
            foreach (KeyCode vkey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vkey) == true)
                {
                    Transform childe = transform.GetChild(3).GetChild(1);
                    if (meleeInput == KeyCode.None)
                    {
                        meleeInput = vkey;
                        childe.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = meleeInput.ToString();
                    }
                    if (rangedInput == KeyCode.None)
                    {
                        rangedInput = vkey;
                        childe.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = rangedInput.ToString();
                    }
                    if (dodgeInput == KeyCode.None)
                    {
                        dodgeInput = vkey;
                        childe.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = dodgeInput.ToString();
                    }

                    isChangingKeys = false;
                    break;
                }
            }
        }


        //string inputText = inputField.text;
        //print(inputText);
        if (Input.GetKey(KeyCode.Escape))
        {
            ToggleMenu();
        }
        if (Input.GetKey(KeyCode.Q) && debounce == 120)
        {
            debounce = 119f;
            if (!isMenuOpen)
            {
                ToggleMenu();
            }
            if (!isStatsOpen)
            {
                ToggleStats();
            }
            else
            {
                ToggleStats();
                ToggleStats();
            }
            if (isSettingsOpen) //Close settings
            {
                ToggleSettings();
            }
            gameObject.transform.GetChild(1).transform.localScale = new Vector3(1f, 0f, 0f);
        }
        else if (debounce != 120)
        {
            if(debounce <= 0)
            {
                debounce = 120f;
            }
            else
            {
                debounce--;
            }
        }
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
