using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSettings : MonoBehaviour
{
    public enum RealityMode
    {
        VR,
        ARScreen,
        ARHeadset
    }

    public RealityMode realityMode = RealityMode.VR;
    public Email.EmailLayoutModes emailLayout = Email.EmailLayoutModes.Deck;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
