using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameGrab : MonoBehaviour
{
    public GameObject TMP_InputField_Username;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetUsername();
    }

    public void GetUsername()
    {
        string name = TMP_InputField_Username.GetComponent<TMP_InputField>().text;
        Debug.Log("Username: " + name);
    }
}
