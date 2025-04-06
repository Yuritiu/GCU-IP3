using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Xml.Linq;
using System;

public class Capturing : MonoBehaviour
{

    //public string triggerString;
    public BarTenderAnimation opAnimControl;
    public UnityEvent m_MyEvent = new UnityEvent();
    public TMP_InputField inField;
    public BVHRecorder recorder;
    public Transform rootBoney;

    // Start is called before the first frame update
    void Start()
    {
        opAnimControl = FindAnyObjectByType<BarTenderAnimation>();
        
        //m_MyEvent.AddListener(MyAction);
    }


    public void CaptureStart()
    {
        //This Section is, primarilly, from the BVH Tools Addon ///
        recorder = gameObject.AddComponent<BVHRecorder>();
        recorder.targetAvatar = opAnimControl.animator;
        recorder.scripted = true;
        recorder.getBones();
        recorder.buildSkeleton();
        recorder.genHierarchy();
        recorder.rootBone = rootBoney;
        recorder.capturing = true;
        //This Section is, primarilly, from the BVH Tools Addon ///

        opAnimControl.BottleTr();
    }

    public void CaputureEnding()
    {
        //This Section is, primarilly, from the BVH Tools Addon ///
        recorder.capturing = false;
        recorder.filename = inField.text;
        recorder.directory = "C:\\Users\\kylem\\OneDrive\\Documents\\GitHub\\GCU-IP3\\Assets";
        recorder.saveBVH();
        recorder.clearCapture();
        //This Section is, primarilly, from the BVH Tools Addon ///
    }


}
