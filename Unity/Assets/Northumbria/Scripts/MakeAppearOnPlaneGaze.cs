using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Moves the ARSessionOrigin in such a way that it makes the given content appear to be
/// at a given location acquired via a raycast.
/// </summary>
[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARRaycastManager))]
public class MakeAppearOnPlaneGaze : MonoBehaviour
{
    [SerializeField]
    [Tooltip("A transform which should be made to appear to be at the point.")]
    Transform m_Content;

    /// <summary>
    /// A transform which should be made to appear to be at the touch point.
    /// </summary>
    public Transform content
    {
        get { return m_Content; }
        set { m_Content = value; }
    }

    public GameObject emailObject;

    [SerializeField]
    [Tooltip("The rotation the content should appear to have.")]
    Quaternion m_Rotation;

    /// <summary>
    /// The rotation the content should appear to have.
    /// </summary>
    public Quaternion rotation
    {
        get { return m_Rotation; }
        set
        {
            m_Rotation = value;
            if (m_SessionOrigin != null)
                m_SessionOrigin.MakeContentAppearAt(content, content.transform.position, m_Rotation);
        }
    }

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        //if (Input.touchCount == 0 || m_Content == null)
        //    return;

        //var touch = Input.GetTouch(0);

        if (Input.GetButtonDown("joybutton5"))
        {
            if (m_Content == null)
                return;

            var centre = new Vector2(Screen.width / 2, Screen.height / 2);

            if (m_RaycastManager.Raycast(centre, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                // This does not move the content; instead, it moves and orients the ARSessionOrigin
                // such that the content appears to be at the raycast hit position.
                m_SessionOrigin.MakeContentAppearAt(content, hitPose.position, m_Rotation);

                //var pos = Camera.main.transform.position;

                //content.LookAt(pos);
                //content.RotateAround(pos, Vector3.up, 180);
                //content.localEulerAngles = new Vector3(0, content.transform.localEulerAngles.y, 0);

                content.gameObject.SetActive(true);

                emailObject.transform.localPosition = Vector3.zero;
                emailObject.transform.localEulerAngles = Vector3.zero;
                emailObject.transform.localScale = new Vector3(1, 1, 1);
                emailObject.transform.parent = content.transform;
                //emailObject.transform.Rotate(new Vector3(1, 0, 0), 90);

                Debug.Log(hitPose.position);
            }
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARSessionOrigin m_SessionOrigin;

    ARRaycastManager m_RaycastManager;
}
