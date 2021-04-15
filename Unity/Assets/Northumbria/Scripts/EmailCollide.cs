using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailCollide : MonoBehaviour
{
    public Email email;
    public GameObject reticule;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(email)
       {
            email.Entered(true);
       }

       if(reticule)
       {
            reticule.SetActive(true);
       }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (email)
        {
            email.Entered(false);
        }

        if (reticule)
        {
            reticule.SetActive(false);
        }
    }
}
