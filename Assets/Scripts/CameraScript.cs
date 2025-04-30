using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject textBox;
    public string textBoxText = "Hello!";

    Vector3 camPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        textBox.GetComponent<TextMeshProUGUI>().text = textBoxText;
        camPos = _player.transform.position;
        camPos.z = -10;
        _camera.transform.position = camPos;
    }
}
