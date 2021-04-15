using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTest : MonoBehaviour
{
    public GameObject menuList;
    public GameObject tcpList;
    public GameObject participant;
    public GameObject demoList;
    public GameObject controlsList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggleMenuList()
    {
        menuList.SetActive(!menuList.activeSelf);
    }

    public void OnToggleTCPList()
    {
        participant.SetActive(false);
        demoList.SetActive(false);
        controlsList.SetActive(false);
        tcpList.SetActive(!tcpList.activeSelf);
    }

    public void OnToggleParticipant()
    {
        tcpList.SetActive(false);
        demoList.SetActive(false);
        controlsList.SetActive(false);
        participant.SetActive(!participant.activeSelf);
    }

    public void OnToggleDemo()
    {
        tcpList.SetActive(false);
        participant.SetActive(false);
        controlsList.SetActive(false);
        demoList.SetActive(!demoList.activeSelf);
    }

    public void OnToggleControls()
    {
        tcpList.SetActive(false);
        participant.SetActive(false);
        demoList.SetActive(false);
        controlsList.SetActive(!controlsList.activeSelf);
    }
}
