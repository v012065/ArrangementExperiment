using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public Email email;
    public EmailTest emailTest;

    public GameObject worldMessage;
    public GameObject uiMessage;
    public GameObject ui;

    public GameObject aryzon;
    public GameObject arcamera;

    public bool enabled = false;

    bool keyPressedLeft;
    bool keyPressedRight;

    GameObject message;

    // Start is called before the first frame update
    void Start()
    {
        message = uiMessage;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxis("LeftRight"));

        //Debug.Log("joyaxis0: " + Input.GetAxis("joyaxis0"));
        //Debug.Log("joyaxis1: " + Input.GetAxis("joyaxis1"));

        //if(Input.GetAxis("LeftRight") >= 1)
        //{
        //    if (!keyPressedRight)
        //    {
        //        email.NextMessage();
        //        keyPressedRight = true;
        //    }
        //}
        //else
        //{
        //    keyPressedRight = false;
        //}

        //if (Input.GetAxis("LeftRight") <= -1)
        //{
        //    if (!keyPressedLeft)
        //    {
        //        email.PrevMessage();
        //        keyPressedLeft = true;
        //    }
        //}
        //else
        //{
        //    keyPressedLeft = false;
        //}

        //Debug.Log(Input.GetAxis("Vertical"));

        //if (Input.GetAxis("Vertical") >= 1)
        //{
        //    Debug.Log("up");
        //}
        //else if (Input.GetAxis("Vertical") <= -1)
        //{
        //    Debug.Log("down");
        //}

        if (Input.GetButtonDown("Submit"))
        {
            //Debug.Log("Select");

            email.CheckMessage();
            emailTest.UpdateTestFile();
        }

        // B
        if (Input.GetButtonDown("joybutton0"))
        {
            //Debug.Log("Button 0");
            email.NextMessage();
        }

        // D
        if (Input.GetButtonDown("joybutton1"))
        {
        //    Debug.Log("Button 1");
            if(message != null)
            {
                if (message == worldMessage)
                {
                    message.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 4.0f;
                    message.transform.LookAt(Camera.main.transform);
                    message.transform.Rotate(Vector3.up, 180);
                }
                //message.SetActive(!message.activeSelf);
            }
        }

        // C
        if (Input.GetButtonDown("joybutton2"))
        {
            Debug.Log("Button 2");
            if (ui != null)
            {
                ui.SetActive(!ui.activeSelf);
            }
        }

        // A
        if (Input.GetButtonDown("joybutton3"))
        {
            //Debug.Log("Button 3");
            email.PrevMessage();
        }

        // Menu Button
        if (Input.GetButtonDown("joybutton4"))
        {
            //Debug.Log("Button 4");
            if (emailTest.demoStarted)
            {
                email.CheckMessage();
            }
            else if (!emailTest.testStarted)
            {
                emailTest.StartTest();
            }
            else // test started
            {
                email.CheckMessage();
                emailTest.UpdateTestFile();
            }
        }

        // Back Button
        if (Input.GetButtonDown("joybutton5"))
        {
            //if (!emailTest.testStarted)
            //{
            //    emailTest.StartTest();
            //}

            //    //Debug.Log("Button 5");

            //    message.SetActive(false);

            //    if (!aryzon.activeSelf)
            //    {
            //        ui.SetActive(false);
            //        arcamera.GetComponent<Camera>().enabled = false;
            //        aryzon.SetActive(true);
            //        message = worldMessage;
            //    }
            //    else
            //    {
            //        aryzon.SetActive(false);
            //        arcamera.GetComponent<Camera>().enabled = true;
            //        ui.SetActive(true);
            //        message = uiMessage;
            //    }

            //    message.SetActive(true);
            //    email.message = message.GetComponentInChildren<Text>();
            //}

            //if (Input.GetButtonDown("joybutton6"))
            //{
            //    Debug.Log("Button 6");
            //}

            //if (Input.GetButtonDown("joybutton7"))
            //{
            //    Debug.Log("Button 7");
            //}
            //if (Input.GetButtonDown("joybutton8"))
            //{
            //    Debug.Log("Button 8");

            email.ResetPosition();
        }

    }
}
