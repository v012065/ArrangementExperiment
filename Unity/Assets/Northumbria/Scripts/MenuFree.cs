using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFree : MonoBehaviour
{
    public GameObject menuList;
    public GameObject modeList;
    public GameObject participant;

    public ScreenOrientation orientation;

    // Start is called before the first frame update
    void Start()
    {
        orientation = Screen.orientation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Screen.orientation != orientation)
        {
            orientation = Screen.orientation;
            Debug.Log("Screen change: " + orientation);
        }
    }

    public void OnToggleMenuList()
    {
        menuList.SetActive(!menuList.activeSelf);
    }

    public void OnToggleModeList()
    {
        participant.SetActive(false);
        modeList.SetActive(!modeList.activeSelf);
    }

    public void OnToggleParticipant()
    {
        modeList.SetActive(false);
        participant.SetActive(!participant.activeSelf);
    }
}
