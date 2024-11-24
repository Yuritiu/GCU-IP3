using UnityEngine;
using TMPro;
using System.IO;
using Unity.Services.Analytics;
using UnityEngine.Analytics;
using JetBrains.Annotations;
using Unity.Services.Core;
using System;
using Unity.Services.Core.Environments;
using System.Collections;

public class TextSaveTest : MonoBehaviour
{
    public void Start()
    {
        UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection(); 
    }

    public string environment = "production";

    async void Awake()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            // An error occurred during services initialization.
        }
    }

 

public void SaveToFile(TMP_InputField inputField)
    {

        MyEvent myEvent = new MyEvent
        {
            EventTest = "hello there",
            
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
    }

public class MyEvent : Unity.Services.Analytics.Event
{
    public MyEvent() : base("myEvent")
    {
    }

    public string EventTest { set { SetParameter("EventTest", value); } }

}
}
