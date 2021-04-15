using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class EmailTest : MonoBehaviour
{
    public int currentParticipant = 0;
    public string dataFile = "data.txt";
    public string testFile = "test.txt";
    public bool logPosData = false;

    public Email email;
    public GameObject startButton;
    public GameObject startTestButton;
    public GameObject selectButton;
    public GameObject demoSelectButton;
    public Controller controller;
    public Text updateText;
    public GameObject arcamera;
    public GameObject ARSession;
    public GameObject ui;
    public GameObject buttons;

    StreamWriter writer;
    StreamWriter dataWriter;
    public float timer;
    float dataTimer;
    float dataUpdateNextSec = 0;

    public int[] currentBlockList;
    string[] blocks;

    public int trialPerBlock = 10;
    public int currentTrial = 0;
    public int currentBlock = 0;
    public bool testStarted = false;
    public bool demoStarted = false;

    public int[] randomMessageList;

    // Start is called before the first frame update
    void Start()
    {
        if (email == null) email = gameObject.GetComponent<Email>();

        OpenLogFiles();

        TextAsset mytxtData = Resources.Load("Text/eblocks", typeof(TextAsset)) as TextAsset;
        string txt = mytxtData.text;
        blocks = txt.Split('\n');

        currentBlockList = new int[6];

        randomMessageList = new int[trialPerBlock];
    }

    void OpenLogFiles()
    {
        string path = Application.persistentDataPath + "/data/" + currentParticipant;

        Debug.Log(path);

        if (!System.IO.File.Exists(path + "/" + testFile))
        {
            Directory.CreateDirectory(path);
            writer = new StreamWriter(path + "/" + testFile, true);
            writer.WriteLine("ID,Mode,Email,Time,Hit,Headset");
            writer.Flush();
        }
        else
        {
            writer = new StreamWriter(path + "/" + testFile, true);
        }

        if (logPosData)
        {
            if (!System.IO.File.Exists(path + "/" + dataFile))
            {
                dataWriter = new StreamWriter(path + "/" + dataFile, true);
                dataWriter.WriteLine("ID,PosX,PosY,PosZ,RotX,RotY,RotZ,Time");
                dataWriter.Flush();
            }
            else
            {
                dataWriter = new StreamWriter(path + "/" + dataFile, true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!email.disabled) timer += Time.deltaTime;
        dataTimer += Time.deltaTime;

        if ((dataTimer >= dataUpdateNextSec) && logPosData)
        {
            Transform camera = Camera.main.transform;
            dataWriter.WriteLine("" + currentParticipant + "," + camera.position.x + "," + camera.position.y + "," + camera.position.z + "," + camera.localEulerAngles.x + "," + camera.localEulerAngles.y + "," + camera.localEulerAngles.z + "," + dataTimer);
            dataWriter.Flush();

            dataUpdateNextSec = dataTimer + 1.0f;
        }
    }

    public void StartTestSetup()
    {
        var selectedBlock = blocks[Random.Range(0, blocks.Length)];
        var splitBlock = selectedBlock.Split(' ');

        for (int i = 0; i < 6; ++i)
        {
            currentBlockList[i] = int.Parse(splitBlock[i]);
        }

        //updateText.text = "Started - Block: " + currentBlock + " - Trial: " + currentTrial;

        startTestButton.SetActive(true);
        selectButton.SetActive(false);
        demoSelectButton.SetActive(false);

        StartNextBlock();
    }

    void StartNextBlock()
    {
        //updateText.text = "End Block";

        currentTrial = 0;

        for (var i = 0; i < trialPerBlock; ++i)
        {
            randomMessageList[i] = -1;
        }

        testStarted = false;
        email.updateMessage = false;

        if (currentBlock > 5)
        {
            EndBlock();
            return;
        }

        switch (currentBlockList[currentBlock])
        {
            case 0: email.SetCircleMode(); break;
            case 1: email.SetDeckMode(); break;
            case 2: email.SetDeckYStepMode(); break;
            case 3: email.SetLineMode(); break;
            case 4: email.SetSpiralMode(); break;
            case 5: email.SetWallMode(); break;
            default: break;
        }

        updateText.text = "Click to start";
        email.message.text = "Click to start";

        ResetTest();
    }

    void EndBlock()
    {
        updateText.text = "End";
        email.message.text = "End";

        email.disabled = true;
        timer = 0;

        startButton.SetActive(true);
        selectButton.SetActive(false);

        currentBlock = 0;
    }

    public void StartTest()
    {
        email.disabled = false;
        timer = 0;

        startButton.SetActive(false);
        selectButton.SetActive(true);
        demoSelectButton.SetActive(false);

        updateText.text = "Started - Block: " + currentBlock + " - Trial: " + currentTrial;

        //Debug.Log("Started");
        //Debug.Log(StackTraceUtility.ExtractStackTrace());

        testStarted = true;
        demoStarted = false;
        email.randomiseMessage = false;

        for (var i = 0; i < trialPerBlock; ++i)
        {
            randomMessageList[i] = -1;
        }

        GenerateRandomMessage();
        email.message.text = email.titles[email.randomMessage];
        email.updateMessage = true;
    }

    public void StartDemo()
    {
        email.disabled = false;
        timer = 0;

        startButton.SetActive(false);
        selectButton.SetActive(false);
        demoSelectButton.SetActive(true);
        controller.enabled = true;

        updateText.text = "Demo";
        testStarted = false;
        demoStarted = true;
        email.randomiseMessage = true;
        email.updateMessage = true;
    }

    public void UpdateTestFile()
    {
        if (email.currentEmail > -1 && email.clicked)
        {
            email.clicked = false;

            float clickTime = timer;

            string mode = Email.GetModeString(email.mode);

            if (writer != null)
            {
                writer.WriteLine("" + currentParticipant + "," + mode + "," + email.currentEmail + "," + clickTime + "," + (email.hit ? 1 : 0) + "," + (email.aryzon.activeSelf ? 1 : 0));
                writer.Flush();
            }

            timer = 0;

            updateText.text = "Next - Block: " + currentBlock + " - Trial: " + ++currentTrial;

            if (currentTrial >= trialPerBlock)
            {
                ++currentBlock;
                StartNextBlock();
                return;
            }

            // Last email in block repeat previous
            if (currentTrial == (trialPerBlock - 1))
            {
                email.randomMessage = randomMessageList[Random.Range(0, trialPerBlock - 2)];
                //email.message.text = email.titles[email.randomMessage];
            }
            else
            {
                GenerateRandomMessage();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }

        if ((dataWriter != null) && logPosData)
        {
            dataWriter.Flush();
            dataWriter.Close();
        }
    }

    public void ResetTest()
    {
        email.disabled = true;
        timer = 0;

        if (!demoStarted)
        {
            startTestButton.SetActive(false);
            startButton.SetActive(true);
            selectButton.SetActive(false);
        }

        controller.enabled = true;

        updateText.text = "Click to start";

        demoStarted = false;
    }

    public void UpdateParticipantID(string val)
    {
        if (int.TryParse(val, out currentParticipant))
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
            }

            if ((dataWriter != null) && logPosData)
            {
                dataWriter.Flush();
                dataWriter.Close();
            }

            OpenLogFiles();
        }
    }

    public bool IsMessageInList()
    {
        for (var i = 0; i < currentTrial; ++i)
        {
            if (email.randomMessage == randomMessageList[i])
            {
                return true;
            }
        }

        return false;
    }

    public void GenerateRandomMessage()
    {
        email.randomMessage = Random.Range(0, transform.childCount - 1);

        // Check no repeat email
        while (IsMessageInList() || (email.randomMessage == 12))
        {
            email.randomMessage = Random.Range(0, transform.childCount - 1);
        }

        //email.message.text = email.titles[email.randomMessage];
        randomMessageList[currentTrial] = email.randomMessage;

        //Debug.Log(email.randomMessage);
    }

    public void ToggleAryzon()
    {
        if (!email.aryzon.activeSelf)
        {
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            ui.SetActive(false);
            arcamera.GetComponent<Camera>().enabled = false;
            email.aryzon.SetActive(true);
        }
        else
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            email.aryzon.SetActive(false);
            arcamera.GetComponent<Camera>().enabled = true;
            ui.SetActive(true);
        }
    }

    public void ToggleButtons()
    {
        buttons.SetActive(!buttons.activeSelf);
    }

    public void TogglePlanes()
    {
        if (!ARSession.GetComponent<ARPlaneManager>().enabled)
        {
            ARSession.GetComponent<ARPlaneManager>().enabled = true;
            ARSession.GetComponent<ARPointCloudManager>().enabled = true;
            ARSession.GetComponent<ARRaycastManager>().enabled = true;
            ARSession.GetComponent<MakeAppearOnPlaneGaze>().enabled = true;
        }
        else
        {
            ARSession.GetComponent<ARPlaneManager>().enabled = false;
            ARSession.GetComponent<ARPointCloudManager>().enabled = false;
            ARSession.GetComponent<ARRaycastManager>().enabled = false;
            ARSession.GetComponent<MakeAppearOnPlaneGaze>().enabled = false;
        }
    }

    public void ToggleQR()
    {
        if (!ARSession.GetComponent<ARTrackedImageManager>().enabled)
        {
            ARSession.GetComponent<ARTrackedImageManager>().enabled = true;
            ARSession.GetComponent<TrackedImageInfoManagerEmail>().enabled = true;
        }
        else
        {
            ARSession.GetComponent<ARTrackedImageManager>().enabled = false;
            ARSession.GetComponent<TrackedImageInfoManagerEmail>().enabled = false;
        }
    }

    public void ToggleZoom()
    {
        email.zoomEnabled = !email.zoomEnabled;
    }

    public void SoftReset()
    {
        bool randomBackup = email.randomiseMessage;
        bool messageBackup = email.updateMessage;
        email.randomiseMessage = false;
        email.updateMessage = false;

        email.entered = false;
        email.disabled = true;
        email.clicked = true;

        email.ResetMessages();
        timer = 0;
        email.clicked = false;

        email.randomiseMessage = randomBackup;
        email.updateMessage = messageBackup;
    }
}
