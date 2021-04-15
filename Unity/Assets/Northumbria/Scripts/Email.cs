using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class Email : MonoBehaviour
{
    public enum EmailLayoutModes
    {
        Deck,
        DeckYStepped,
        Circle,
        Spiral,
        Line,
        Wall
    }

    public EmailLayoutModes mode = EmailLayoutModes.Deck;

    public Vector3 startPosition;
    public string emailFolder;
    public GameObject prefab;
    public int currentEmail;
    public bool entered;
    public bool hit = false;
    public bool disabled = true;
    public bool clicked = false;
    public int frontEmail = 0;
    public int lastEmail = 0;
    public bool fixOverlayEmail = false;
    public float scaleLastFrame = 0.0f;

    public Text message;
    public string[] titles;
    public float[] yOffsets;
    public int randomMessage;
    public bool randomiseMessage = true;
    public bool updateMessage = true;
    //public DataOverLoad data;
    public bool endFail = true;
    public float deckZStep = 0.1f;
    public float deckYStep = 0.0f;
    public float lineXStep = 0.0f;
    public float rotInc = 30.0f;
    public Vector3 circleDist = new Vector3(1,0,4.0f);
    public Vector3 spiralDist = new Vector3(1, 0.04f, 4.0f);
    //public RingController controller;
    public GameObject reticule;

    public GameObject nextButton;
    public GameObject backButton;

    public Text current;

    public ScreenOrientation orientation;
    public GameObject emailOverlay;
    public GameObject aryzon;
    public bool zoomEnabled = true;

    //private bool vrbuttonClicked;
    //private string nextButton = "Next";
    //private string backButton = "Back";
    //private string menuButton = "Menu";
    //private string selectButton = "Select";

    // Start is called before the first frame update
    void Start()
    {
        //controller.AddPressCallback(RingController.Buttons.Up, NextMessage);
        //controller.AddPressCallback(RingController.Buttons.Down, PrevMessage);
        //controller.AddPressCallback(RingController.Buttons.Menu, CheckMessage);
        //controller.AddPressCallback(RingController.Buttons.C, OpenMenu);

        TextAsset mytxtData = Resources.Load("Text/emailtitles", typeof(TextAsset)) as TextAsset;
        string txt = mytxtData.text;
        titles = txt.Split('\n');

        CreateEmail();
    }

    void ArrangeEmailDeck(GameObject email, int i)
    {
        email.transform.position = new Vector3(startPosition.x + (lineXStep * i * transform.localScale.x), startPosition.y + (yOffsets[i] * transform.localScale.y) + (deckYStep * i * transform.localScale.y), startPosition.z + (deckZStep * i * transform.localScale.z));
        email.GetComponent<EmailSavePosition>().pos = email.transform.position;
        email.SetActive(true);

        GameObject collide = email.transform.GetChild(0).gameObject;
        collide.transform.position = new Vector3(startPosition.x + (lineXStep * i * transform.localScale.x), startPosition.y + (deckYStep * i * transform.localScale.y), startPosition.z + (deckZStep * i * transform.localScale.z));
        collide.SetActive(false);

        email.GetComponent<SpriteRenderer>().sortingOrder = titles.Length - i;
    }

    void CreateEmail()
    {
        entered = false;
        //vrbuttonClicked = false;

        //mode = data ? data.emailLayout : Email.Deck;

        prefab.SetActive(false);

        yOffsets = new float[titles.Length];

        for (int i = 0; i < titles.Length; ++i)
        {
            string[] strs = titles[i].Split(';');
            yOffsets[i] = float.Parse(strs[0]);
            titles[i] = strs[1];

            GameObject email = Instantiate(prefab, transform);
            email.name = (i + 1).ToString();
            email.SetActive(false);
            email.GetComponent<SpriteRenderer>().sprite = Resources.Load(emailFolder + "/" + (i + 1), typeof(Sprite)) as Sprite;
            email.GetComponent<SpriteRenderer>().sortingOrder = titles.Length - i;

            //Debug.Log(email.GetComponent<SpriteRenderer>().sprite);

            //Debug.Log(email.GetComponent<EmailCollide>());
            //email.GetComponent<EmailCollide>().email = this;

            //switch (mode)
            //{
            //    case EmailLayoutModes.DeckYStepped:
            //    case EmailLayoutModes.Deck: ArrangeEmailDeck(email, i); break;
            //    default: break;
            //}
        }

        //ResetMessages();

        currentEmail = 0;
        frontEmail = 0;
        lastEmail = -1;

        GameObject current = transform.GetChild(currentEmail).gameObject;
        current.SetActive(true);
        GameObject plane = current.transform.GetChild(0).gameObject;
        plane.SetActive(true);

        if (randomiseMessage)
        {
            randomMessage = Random.Range(0, transform.childCount - 1);
            message.text = titles[randomMessage];
        }
    }

    public void NextMessage()
    {
        if (/*entered &&*/ !disabled)
        {
            switch (mode)
            {
                case EmailLayoutModes.Deck:
                case EmailLayoutModes.DeckYStepped:
                case EmailLayoutModes.Line:
                    {
                        if (endFail || (frontEmail != transform.childCount - 1))
                        {
                            GameObject current = transform.GetChild(frontEmail).gameObject;
                            current.SetActive(false);
                            //current.transform.GetChild(0).gameObject.SetActive(false);

                            for (int i = frontEmail; i < transform.childCount; ++i)
                            {
                                Transform child = transform.GetChild(i);
                                Vector3 pos = new Vector3(child.position.x - (lineXStep * transform.localScale.x), child.position.y - (deckYStep * transform.localScale.y), child.position.z - (deckZStep * transform.localScale.z));
                                child.position = pos;
                            }

                            if (++frontEmail < transform.childCount)
                            {
                                current = transform.GetChild(frontEmail).gameObject;
                                //current.transform.GetChild(0).gameObject.SetActive(true);
                            }
                        }

                        if (endFail)
                        {
                            if (frontEmail == transform.childCount)
                            {
                                message.text = "Failed";

                                entered = false;
                                frontEmail = 0;

                                StartCoroutine("DelayReset", 3);
                            }
                        }
                    }
                    break;
                case EmailLayoutModes.Circle:
                case EmailLayoutModes.Spiral:
                    {
                        //frontEmail++;

                        //if(frontEmail >= transform.childCount)
                        //{
                        //    frontEmail = 0;
                        //}

                        for (int i = 0; i < transform.childCount; ++i)
                        {
                            GameObject email = transform.GetChild(i).gameObject;
                            email.transform.RotateAround(startPosition, Vector3.up, -rotInc);
                        }
                    }
                    break;
                default: break;
            }
        }
    }

    public void CheckMessage()
    {
        if (entered && !disabled)
        {
            if (currentEmail == randomMessage)
            {
                message.text = "Correct";
                hit = true;
            }
            // Two possible answers
            else if (((randomMessage == 12) || (randomMessage == 16)) && ((currentEmail == 12) || (currentEmail == 16)))
            {
                message.text = "Correct";
                hit = true;
            }
            else
            {
                message.text = "Incorrect";
                hit = false;
            }

            switch (mode)
            {
                case EmailLayoutModes.Deck:
                case EmailLayoutModes.DeckYStepped:
                case EmailLayoutModes.Line:
                    {
                        //transform.GetChild(currentEmail).GetChild(0).gameObject.SetActive(false);
                    }
                    break;
                case EmailLayoutModes.Spiral:
                case EmailLayoutModes.Circle:
                case EmailLayoutModes.Wall:
                    {
                        if (currentEmail == -1) return;
                    }
                    break;
                default: break;
            }

            entered = false;
            disabled = true;
            clicked = true;

            StartCoroutine("DelayReset", 3);
        }
    }

    public void PrevMessage()
    {
        if (/*entered &&*/ !disabled)
        {
            switch (mode)
            {
                case EmailLayoutModes.Deck:
                case EmailLayoutModes.DeckYStepped:
                case EmailLayoutModes.Line:
                    {
                        GameObject current = transform.GetChild(frontEmail).gameObject;
                        //current.SetActive(false);
                        GameObject plane = current.transform.GetChild(0).gameObject;
                        //plane.SetActive(false);

                        if (--frontEmail != -1)
                        {
                            for (int i = frontEmail; i < transform.childCount; ++i)
                            {
                                Transform child = transform.GetChild(i);
                                Vector3 pos = new Vector3(child.position.x + (lineXStep * transform.localScale.x), child.position.y + (deckYStep * transform.localScale.y), child.position.z + (deckZStep * transform.localScale.z));
                                child.position = pos;
                            }
                        }
                        else
                        {
                            frontEmail = 0;
                        }

                        current = transform.GetChild(frontEmail).gameObject;
                        current.SetActive(true);
                        plane = current.transform.GetChild(0).gameObject;
                        //plane.SetActive(true);
                    }
                    break;
                case EmailLayoutModes.Circle:
                case EmailLayoutModes.Spiral:
                    {
                        //frontEmail--;

                        //if (frontEmail <= 0)
                        //{
                        //    frontEmail = transform.childCount-1;
                        //}

                        for (int i = 0; i < transform.childCount; ++i)
                        {
                            GameObject email = transform.GetChild(i).gameObject;
                            email.transform.RotateAround(startPosition, Vector3.up, rotInc);
                        }
                    }
                    break;
                default: break;
            }
        }
    }

    IEnumerator DelayReset(float count)
    {
        yield return new WaitForSeconds(count); //Count is the amount of time in seconds that you want to wait.
                                                //And here goes your method of resetting the game...
        yield return ResetMessages();
    }

    public IEnumerator ResetMessages()
    {
        switch (mode)
        {
            case EmailLayoutModes.Deck:
            case EmailLayoutModes.DeckYStepped:
            case EmailLayoutModes.Line:
                {
                    //currentEmail = 0;
                    currentEmail = -1;
                    frontEmail = 0;
                    //lastEmail = -1;

                    //Debug.Log("Child Count: " + transform.childCount);
                    for (int i = 0; i < transform.childCount; ++i)
                    {
                        //Debug.Log("Log£jhk: " + i);
                        Transform child = transform.GetChild(i);

                        Vector3 record = child.GetComponent<EmailSavePosition>().pos;
                        Vector3 pos = new Vector3(record.x, record.y, record.z);
                        //Vector3 pos = new Vector3(0, 0, 0);
                        child.position = new Vector3(pos.x, pos.y /*+ (deckYStep * i)*/, pos.z /*+ (deckZStep * i)*/);
                        //child.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        child.gameObject.SetActive(true);
                        //child.GetComponent<SpriteRenderer>().sortingOrder = transform.childCount - i;

                        GameObject current = child.gameObject;
                        current.SetActive(true);
                        GameObject plane = current.transform.GetChild(0).gameObject;
                        plane.SetActive(true);
                    }

                    //GameObject current = transform.GetChild(currentEmail).gameObject;
                    //current.SetActive(true);
                    //GameObject plane = current.transform.GetChild(0).gameObject;
                    //plane.SetActive(true);
                }
                break;
            case EmailLayoutModes.Wall:
            case EmailLayoutModes.Spiral:
            case EmailLayoutModes.Circle: currentEmail = -1; break;
            default: break;
        }

        if (randomiseMessage)
        {
            randomMessage = Random.Range(0, transform.childCount - 1);
        }

        if (updateMessage)
        {
            message.text = titles[randomMessage];
        }

        hit = false;
        disabled = false;
        clicked = false;

        lastEmail = -1;

        return null;
    }

    public void Entered(bool set)
    {
        entered = set;
    }

    public void Triggered(BaseEventData eventData)
    {
        // Only trigger on left input button, which maps to
        // Daydream controller TouchPadButton and Trigger buttons.
        //PointerEventData ped = eventData as PointerEventData;
        //if (ped != null)
        //{
        //    if (ped.button == PointerEventData.InputButton.Left)
        //    {
        //        if (vrbuttonClicked == false)
        //        {
        //            NextMessage();
        //            vrbuttonClicked = true;
        //        }
        //    }
        //    else
        //    {
        //        vrbuttonClicked = false;
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if (NextButtonPressed())
        //{
        //    NextMessage();
        //}
        //else if (BackButtonPressed())
        //{
        //    PrevMessage();
        //}
        //else if (SelectButtonPressed())
        //{
        //    CheckMessage();
        //}
        //else if (MenuButtonPressed())
        //{
        //    OpenMenu();
        //}

        current.text = currentEmail.ToString();

        if ((currentEmail > -1) && (currentEmail < titles.Length))
        {
            prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load(emailFolder + "/" + (currentEmail + 1), typeof(Sprite)) as Sprite;

            //prefab.transform.position = new Vector3(prefab.transform.position.x, prefab.transform.position.y + (yOffsets[currentEmail] * transform.localScale.y), prefab.transform.position.z);

            if (prefab.transform.parent != null)
            {
                prefab.SetActive(true);
            }
        }
        else
        {
            prefab.SetActive(false);
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            //Debug.DrawLine(ray.origin, hit.point);
            //Debug.Log("You selected the " + hit.transform.name);

            Entered(true);

            if (reticule)
            {
                //reticule.SetActive(true);
                reticule.transform.localScale = new Vector2(2, 2);
            }

            if (hit.collider.transform.parent.GetComponent<EmailSavePosition>())
            {
                //if (mode == EmailLayoutModes.Circle || mode == EmailLayoutModes.Spiral || mode == EmailLayoutModes.Wall)
                //{
                //currentEmail = hit.collider.gameObject.transform.parent.GetSiblingIndex();// int.Parse(hit.collider.gameObject.transform.parent.name)-1;
                currentEmail = int.Parse(hit.collider.gameObject.transform.parent.name) - 1;
                //}
            }
        }
        else
        {
            Entered(false);

            if (reticule)
            {
                //reticule.SetActive(false);
                reticule.transform.localScale = new Vector2(1, 1);
            }

            //if (mode == EmailLayoutModes.Circle || mode == EmailLayoutModes.Spiral || mode == EmailLayoutModes.Wall)
            //{
            currentEmail = -1;
            //}
        }

        if (emailOverlay != null)
        {
            if (!aryzon.activeSelf && zoomEnabled)
            {
                if (Screen.orientation != orientation)
                {
                    orientation = Screen.orientation;

                    //Debug.Log("Screen change: " + orientation);
                }

                switch (orientation)
                {
                    case ScreenOrientation.LandscapeLeft:
                    case ScreenOrientation.LandscapeRight:
                        {
                            emailOverlay.SetActive(false);
                        }
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                    case ScreenOrientation.Portrait:
                        {
                            Image image = emailOverlay.GetComponentInChildren<Image>();

                            if (image != null)
                            {
                                if (currentEmail == -1)
                                {
                                    if (!fixOverlayEmail)
                                    {
                                        emailOverlay.SetActive(false);
                                    }
                                }
                                else if (currentEmail == lastEmail)
                                {
                                    emailOverlay.SetActive(true);
                                }
                                else if (currentEmail != lastEmail)
                                {
                                    lastEmail = currentEmail;

                                    emailOverlay.SetActive(true);

                                    //Debug.Log("Email Orient Happened");

                                    //Image image = emailOverlay.GetComponentInChildren<Image>();
                                    if (image)
                                    {
                                        //Debug.Log("Email Orient Image Happened");

                                        Sprite sprite = Resources.Load(emailFolder + "/" + (currentEmail + 1), typeof(Sprite)) as Sprite;
                                        image.sprite = sprite;
                                        image.SetNativeSize();
                                        //image.sprite = sprite;
                                        //Vector2 size = image.GetComponent<RectTransform>().sizeDelta;
                                        //size *= image.GetComponent<Image>().pixelsPerUnit;
                                        //Vector2 pixelPivot = image.GetComponent<Image>().sprite.pivot;
                                        //Vector2 percentPivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
                                        //image.GetComponent<RectTransform>().pivot = percentPivot;

                                        image.transform.localPosition = new Vector2(0, 550);
                                        image.transform.localScale = new Vector2(1, 1);
                                    }
                                }
                                //else
                                //{
                                //    emailOverlay.GetComponentInChildren<Image>().transform.localPosition = new Vector2(0, 550);
                                //}

                                //if (currentEmail != -1)
                                //{
                                //Image image = emailOverlay.GetComponentInChildren<Image>();

                                if (Input.touchCount != 0)
                                {
                                    var touch = Input.GetTouch(0);

                                    GraphicRaycaster raycaster = emailOverlay.GetComponent<GraphicRaycaster>();

                                    //Set up the new Pointer Event
                                    var pointerEventData = new PointerEventData(EventSystem.current);
                                    //Set the Pointer Event Position to that of the mouse position
                                    pointerEventData.position = touch.position;

                                    //Create a list of Raycast Results
                                    List<RaycastResult> results = new List<RaycastResult>();

                                    //Raycast using the Graphics Raycaster and mouse click position
                                    raycaster.Raycast(pointerEventData, results);

                                    //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                                    foreach (RaycastResult result in results)
                                    {
                                        if (result.gameObject.name == "EmailImage")
                                        {
                                            if (Input.touchCount == 2)
                                            {
                                                float scale = Vector2.Distance(Input.touches[0].position, Input.touches[1].position) / Screen.dpi;
                                                float dscale = scale - scaleLastFrame;

                                                float newscale = image.transform.localScale.x + dscale;
                                                if (newscale < 1)
                                                {
                                                    newscale = 1;
                                                }

                                                image.transform.localScale = new Vector2(newscale, newscale);
                                                scaleLastFrame = scale;
                                            }
                                            else if (Input.touchCount == 1)
                                            {
                                                var move = new Vector2(touch.deltaPosition.x, touch.deltaPosition.y);
                                                image.transform.Translate(move);
                                            }
                                        }
                                    }
                                }
                                else if (Input.GetMouseButton(2))
                                {
                                    //Debug.Log("Email Click");

                                    GraphicRaycaster raycaster = emailOverlay.GetComponent<GraphicRaycaster>();

                                    //Set up the new Pointer Event
                                    var pointerEventData = new PointerEventData(EventSystem.current);
                                    //Set the Pointer Event Position to that of the mouse position
                                    pointerEventData.position = Input.mousePosition;

                                    //Create a list of Raycast Results
                                    List<RaycastResult> results = new List<RaycastResult>();

                                    //Raycast using the Graphics Raycaster and mouse click position
                                    raycaster.Raycast(pointerEventData, results);

                                    //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                                    foreach (RaycastResult result in results)
                                    {
                                        if (result.gameObject.name == "EmailImage")
                                        {
                                            image.transform.Translate(Input.mouseScrollDelta * 50);
                                        }
                                    }
                                }
                                else
                                {
                                    image.transform.Translate(new Vector2(0, Input.GetAxis("Vertical") * 50));
                                }
                            }
                        }
                        break;
                    default: break;
                }
            }
            else
            {
                emailOverlay.SetActive(false);
            }
        }
    }

    public void FixOverlayEmail()
    {
        fixOverlayEmail = !fixOverlayEmail;
    }

    private float lerpTime = 0;

    private void FixedUpdate()
    {
        if (prefab.activeSelf)
        {
            prefab.transform.position = Vector3.Lerp(prefab.GetComponent<EmailSavePosition>().pos, prefab.transform.position, lerpTime);

            prefab.GetComponent<EmailSavePosition>().pos = prefab.transform.position;

            lerpTime += Time.deltaTime / Time.fixedDeltaTime;
        }
    }

    //private bool NextButtonPressed()
    //{
    //    if (entered)
    //    {
    //        if (Input.GetButtonDown(nextButton))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //private bool BackButtonPressed()
    //{
    //    if (entered)
    //    {
    //        if (Input.GetButtonDown(backButton))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //private bool MenuButtonPressed()
    //{
    //    if (Input.GetButtonDown(menuButton))
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    //private bool SelectButtonPressed()
    //{
    //    if (entered)
    //    {
    //        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown(selectButton))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //private void OpenMenu()
    //{
    //    //XRSettings.enabled = false;

    //    //DataOverLoad data = FindObjectOfType<DataOverLoad>();
    //    //if (data != null)
    //    //{
    //    //    data.aryzonEnabled = false;
    //    //}

    //    SceneManager.LoadScene("StartScreen", LoadSceneMode.Single);
    //}

    //public void MoveBack()
    //{
    //    transform.Translate(new Vector3(0, 0, 0.1f));
    //}

    //public void MoveTowards()
    //{
    //    transform.Translate(new Vector3(0, 0, -0.1f));
    //}

    //public void MoveLeft()
    //{
    //    transform.Translate(new Vector3(-0.1f, 0, 0));
    //}

    //public void MoveRight()
    //{
    //    transform.Translate(new Vector3(0.1f, 0, 0));
    //}

    //public void MoveUp()
    //{
    //    transform.Translate(new Vector3(0, 0.1f, 0));
    //}

    //public void MoveDown()
    //{
    //    transform.Translate(new Vector3(0, -0.1f, 0));
    //}

    //void DestroyChildren()
    //{
    //    List<GameObject> children = new List<GameObject>();
    //    foreach (Transform tran in transform)
    //    {
    //        children.Add(tran.gameObject);
    //    }
    //    children.ForEach(child => GameObject.Destroy(child));
    //}

    public void SetDeckMode(bool reset = true)
    {
        EventSystem.current.SetSelectedGameObject(null);

        mode = EmailLayoutModes.Deck;

        deckZStep = 0.3f;
        deckYStep = 0;
        lineXStep = 0;

        StartDeckModes(reset);
    }

    public void SetDeckYStepMode(bool reset = true)
    {
        EventSystem.current.SetSelectedGameObject(null);

        mode = EmailLayoutModes.DeckYStepped;

        deckZStep = 0.3f;
        deckYStep = 0.2f;
        lineXStep = 0;

        StartDeckModes(reset);
    }

    public void SetLineMode(bool reset = true)
    {
        EventSystem.current.SetSelectedGameObject(null);

        mode = EmailLayoutModes.Line;

        deckZStep = 0;
        deckYStep = 0;
        lineXStep = 1.0f;

        StartDeckModes(reset);
    }

    void StartDeckModes(bool reset)
    {
        if (reset)
        {
            ReorderEmail();
            ResetMessages();
        }

        //startPosition = Camera.main.transform.position;
        //startPosition = Vector3.zero;

        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject email = transform.GetChild(i).gameObject;

            email.transform.position = new Vector3(startPosition.x + (lineXStep * i * transform.localScale.x), startPosition.y /*+ (yOffsets[i] * transform.localScale.y)*/ + (deckYStep * i * transform.localScale.y), startPosition.z + (deckZStep * i * transform.localScale.z) + 0.3f);
            //email.transform.LookAt(startPosition);
            //email.transform.Rotate(new Vector3(0, 1, 0), 180);
            email.transform.localEulerAngles = new Vector3(0, 0, 0);
            email.GetComponent<EmailSavePosition>().pos = email.transform.position;
            email.SetActive(true);

            GameObject collide = email.transform.GetChild(0).gameObject;
            //collide.transform.position = new Vector3(startPosition.x + (lineXStep * i * transform.localScale.x), startPosition.y + (deckYStep * i * transform.localScale.y), startPosition.z + (deckZStep * i * transform.localScale.z) + 0.3f);
            collide.transform.position = new Vector3(email.transform.position.x, email.transform.position.y - 0.0556f /* (yOffsets[i] * transform.localScale.y)*/, email.transform.position.z);
            //collide.SetActive(false);
            collide.SetActive(true);
        }

        //transform.position = startPosition + Camera.main.transform.forward;

        //transform.LookAt(startPosition);
        //transform.RotateAround(startPosition, Vector3.up, 180);
        //transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        //GameObject current = transform.GetChild(currentEmail).gameObject;
        //current.SetActive(true);
        //GameObject plane = current.transform.GetChild(0).gameObject;
        //plane.SetActive(true);

        nextButton.SetActive(true);
        backButton.SetActive(true);
    }

    public void SetCircleMode(bool reset = true)
    {
        EventSystem.current.SetSelectedGameObject(null);

        //float circleDist = 0.4f;

        mode = EmailLayoutModes.Circle;

        startCircleMode(circleDist, reset);

        //ResetMessages();

        //startPosition = Camera.main.transform.position;

        //for (int i = 0; i < transform.childCount; ++i)
        //{
        //    GameObject email = transform.GetChild(i).gameObject;

        //    email.transform.position = new Vector3(startPosition.x, startPosition.y + (yOffsets[i] * transform.localScale.y), startPosition.z + circleDist);
        //    email.transform.LookAt(startPosition);
        //    email.transform.Rotate(new Vector3(0, 1, 0), 180);
        //    email.transform.RotateAround(new Vector3(startPosition.x, startPosition.y, startPosition.z), new Vector3(0, 1, 0), (360 / (transform.childCount)) * i);
        //    email.transform.localEulerAngles = new Vector3(0, email.transform.localEulerAngles.y, email.transform.localEulerAngles.z);
        //    email.GetComponent<EmailSavePosition>().pos = email.transform.position;
        //    email.SetActive(true);

        //    GameObject collide = email.transform.GetChild(0).gameObject;
        //    collide.transform.position = new Vector3(email.transform.position.x, email.transform.position.y - (yOffsets[i] * transform.localScale.y), email.transform.position.z);
        //    //collide.transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z + 0.5f);
        //    //email.transform.RotateAround(new Vector3(startPosition.x, startPosition.y, startPosition.z), new Vector3(0, 1, 0), (360 / (transform.childCount)) * i);
        //    //collide.transform.LookAt(startPosition);
        //    //collide.transform.Rotate(new Vector3(0, 1, 0), 180);
        //    collide.SetActive(true);
        //}

        //currentEmail = -1;

        //nextButton.SetActive(false);
        //backButton.SetActive(false);
    }

    public void SetSpiralMode(bool reset = true)
    {
        EventSystem.current.SetSelectedGameObject(null);

        //float spiralDist = 0.8f;
        //float spiralHeightStep = 0.02f;

        mode = EmailLayoutModes.Spiral;

        startCircleMode(spiralDist, reset);

        //ResetMessages();

        //startPosition = Camera.main.transform.position;

        //for (int i = 0; i < transform.childCount; ++i)
        //{
        //    GameObject email = transform.GetChild(i).gameObject;

        //    email.transform.position = new Vector3(startPosition.x, startPosition.y + (yOffsets[i] * transform.localScale.y) + (-0.1f + (spiralHeightStep * transform.localScale.y * i)), startPosition.z + spiralDist);
        //    email.transform.LookAt(startPosition);
        //    email.transform.Rotate(new Vector3(0, 1, 0), 180);
        //    email.transform.RotateAround(new Vector3(startPosition.x, startPosition.y, startPosition.z), new Vector3(0, 1, 0), (360 / (transform.childCount)) * i);
        //    email.transform.localEulerAngles = new Vector3(0, email.transform.localEulerAngles.y, email.transform.localEulerAngles.z);
        //    email.GetComponent<EmailSavePosition>().pos = email.transform.position;
        //    email.SetActive(true);

        //    GameObject collide = email.transform.GetChild(0).gameObject;
        //    collide.transform.position = new Vector3(email.transform.position.x, email.transform.position.y - (yOffsets[i] * transform.localScale.y), email.transform.position.z);
        //    //collide.transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z + 0.5f);
        //    //email.transform.RotateAround(new Vector3(startPosition.x, startPosition.y, startPosition.z), new Vector3(0, 1, 0), (360 / (transform.childCount)) * i);
        //    //collide.transform.LookAt(startPosition);
        //    //collide.transform.Rotate(new Vector3(0, 1, 0), 180);
        //    collide.SetActive(true);
        //}

        //currentEmail = -1;

        //nextButton.SetActive(false);
        //backButton.SetActive(false);
    }

    void startCircleMode(Vector3 offset, bool reset)
    {
        if (reset)
        {
            ReorderEmail();
            ResetMessages();
        }

        rotInc = (360 / transform.childCount) * offset.x;

        //startPosition = Camera.main.transform.position;
        //startPosition = Vector3.zero;

        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject email = transform.GetChild(i).gameObject;

            email.transform.position = new Vector3(startPosition.x/* + (offset.x * transform.localScale.x)*/, startPosition.y /*+ (yOffsets[i] * transform.localScale.y)*/ + (/*-0.1f +*/ (offset.y * transform.localScale.y * i)), startPosition.z + (offset.z * transform.localScale.z));
            email.transform.LookAt(startPosition);
            email.transform.Rotate(new Vector3(0, 1, 0), 180);
            email.transform.RotateAround(startPosition, Vector3.up, ((360 / transform.childCount)* offset.x ) * i);
            email.transform.localEulerAngles = new Vector3(0, email.transform.localEulerAngles.y, email.transform.localEulerAngles.z);
            email.GetComponent<EmailSavePosition>().pos = email.transform.position;
            email.SetActive(true);

            GameObject collide = email.transform.GetChild(0).gameObject;
            collide.transform.position = new Vector3(email.transform.position.x, email.transform.position.y - 0.0556f /* (yOffsets[i] * transform.localScale.y)*/, email.transform.position.z);
            collide.SetActive(true);
        }

        currentEmail = -1;

        //nextButton.SetActive(false);
        //backButton.SetActive(false);
        nextButton.SetActive(true);
        backButton.SetActive(true);
    }

    public void SetWallMode(bool reset = true)
    {
        EventSystem.current.SetSelectedGameObject(null);

        float xSpacing = 1.0f;
        float ySpacing = 1.0f;
        float zSpacing = 0.2f;

        mode = EmailLayoutModes.Wall;

        if (reset)
        {
            ReorderEmail();
            ResetMessages();
        }

        //startPosition = Camera.main.transform.position;
        //startPosition = Vector3.zero;

        int j = 0;

        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject email = transform.GetChild(i).gameObject;

            email.transform.position = new Vector3(startPosition.x + ((xSpacing * j) * transform.localScale.x), startPosition.y + /*(yOffsets[i] * transform.localScale.y)*/ + (ySpacing * transform.localScale.y * (i/4)), startPosition.z/* + 0.3f*/ + (i * zSpacing * transform.localScale.z));
            email.transform.localEulerAngles = new Vector3(0, 0, 0);
            email.GetComponent<EmailSavePosition>().pos = email.transform.position;
            email.SetActive(true);

            GameObject collide = email.transform.GetChild(0).gameObject;
            collide.transform.position = new Vector3(email.transform.position.x, email.transform.position.y - 0.0556f /* (yOffsets[i] * transform.localScale.y)*/, email.transform.position.z);
            collide.SetActive(true);

            if (j++ > 7) j = 0;
        }

        currentEmail = -1;

        nextButton.SetActive(false);
        backButton.SetActive(false);
    }

    public static string GetModeString(EmailLayoutModes _mode)
    {
        string mode = "";

        switch (_mode)
        {
            case Email.EmailLayoutModes.Circle: mode = "Circle"; break;
            case Email.EmailLayoutModes.Deck: mode = "Deck"; break;
            case Email.EmailLayoutModes.DeckYStepped: mode = "DeckUp"; break;
            case Email.EmailLayoutModes.Line: mode = "Line"; break;
            case Email.EmailLayoutModes.Spiral: mode = "Spiral"; break;
            case Email.EmailLayoutModes.Wall: mode = "Wall"; break;
        }

        return mode;
    }

    public static int GetModeNum(EmailLayoutModes _mode)
    {
        int mode = 0;

        switch (_mode)
        {
            case Email.EmailLayoutModes.Circle: mode = 0; break;
            case Email.EmailLayoutModes.Deck: mode = 1; break;
            case Email.EmailLayoutModes.DeckYStepped: mode = 2; break;
            case Email.EmailLayoutModes.Line: mode = 3; break;
            case Email.EmailLayoutModes.Spiral: mode = 4; break;
            case Email.EmailLayoutModes.Wall: mode = 5; break;
        }

        return mode;
    }

    public static EmailLayoutModes GetModeNum(int _num)
    {
        EmailLayoutModes mode = 0;

        switch (_num)
        {
            case 0: mode = Email.EmailLayoutModes.Circle; break;
            case 1: mode = Email.EmailLayoutModes.Deck; break;
            case 2: mode = Email.EmailLayoutModes.DeckYStepped; break;
            case 3: mode = Email.EmailLayoutModes.Line; break;
            case 4: mode = Email.EmailLayoutModes.Spiral; break;
            case 5: mode = Email.EmailLayoutModes.Wall; break;
        }

        return mode;
    }

    public void ResetPosition()
    {
        startPosition = Camera.main.transform.position;

        //switch (mode)
        //{
        //    //case EmailLayoutModes.Circle:
        //    //case EmailLayoutModes.Spiral:
        //    //    {
        //    //        transform.position = startPosition;
        //    //    }
        //    //    break;
        //    default:
        //        {
        //            transform.position = startPosition + Camera.main.transform.forward * 0.1f;
        //        }
        //        break;
        //}

        //transform.LookAt(startPosition);
        //transform.RotateAround(startPosition, Vector3.up, 180);
        //transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        switch (mode)
        {
            case EmailLayoutModes.Circle:
                {
                    SetCircleMode(false);
                }
                break;
            case EmailLayoutModes.Spiral:
                {
                    SetSpiralMode(false);
                }
                break;
            case EmailLayoutModes.Deck:
                {
                    SetDeckMode(false);
                }
                break;
            case EmailLayoutModes.DeckYStepped:
                {
                    SetDeckYStepMode(false);
                }
                break;
            case EmailLayoutModes.Line:
                {
                    SetLineMode(false);
                }
                break;
            case EmailLayoutModes.Wall:
                {
                    SetWallMode(false);
                }
                break;
            default:
                break;
        }
    }

    void ReorderEmail()
    {
        var uniqueList = new List<int>();

        for (int i = 0; i < transform.childCount; ++i)
        {
            uniqueList.Add(i);
        }

        var numList = new List<int>();

        for (int i = 0; i < transform.childCount; ++i)
        {
            int ranNum = uniqueList[Random.Range(0, uniqueList.Count)];
            numList.Add(ranNum);
            //Debug.Log(ranNum);
            uniqueList.Remove(ranNum);
        }

        var emailList = new List<Transform>();

        for (int i = 0; i < transform.childCount; ++i)
        {
            emailList.Add(transform.GetChild(i));
        }

        for (int i = 0; i < emailList.Count; ++i)
        {
            emailList[i].SetSiblingIndex(numList[i]);
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = titles.Length - i;
        }
    }
}
