using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public float MasterVolume = 1f;
    public float MainVolume = 0.1f;
    public bool deleteAfterPlay = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<AudioSource>().volume = MasterVolume * MainVolume;
    }
    void Update()
    {
        if (deleteAfterPlay == true && gameObject.GetComponent<AudioSource>().isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
