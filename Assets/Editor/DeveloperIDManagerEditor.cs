using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeveloperIDManagerEditor : EditorWindow
{
    private DeveloperIdentityManager devManager;

    //Local Copy of Developer ID's Loaded From The Encrypted File
    private List<string> devIDs = new List<string>();
    private Vector2 scrollPos;
    private string newID = "";

    [MenuItem("Tools/Developer ID Manager")]
    public static void ShowWindow()
    {
        GetWindow<DeveloperIDManagerEditor>("Developer ID Manager");
    }

    private void OnEnable()
    {
        devManager = FindObjectOfType<DeveloperIdentityManager>();
        if (devManager == null)
        {
            Debug.LogError("No DeveloperIdentityManager found in the scene. Please add it to a GameObject.");
            return;
        }

        //Load & Decrypt Dev ID's From File Into Local List
        LoadDevIDs();
    }

    private void LoadDevIDs()
    {
        devIDs = new List<string>(devManager.GetDevIDs());
    }

    private void SaveDevIDs()
    {
        //Save The Current List of Dev ID's Back to Encrypted File Via DeveloperIdentityManager
        devManager.SaveDevIDsFromEditor(devIDs);
        EditorUtility.DisplayDialog("Save Successful", "Developer IDs saved and encrypted.", "OK");
    }

    private void OnGUI()
    {
        if (devManager == null)
        {
            EditorGUILayout.HelpBox("Add DeveloperIdentityManager component to a GameObject in your scene.", MessageType.Error);
            if (GUILayout.Button("Reload"))
            {
                OnEnable();
            }
            return;
        }

        EditorGUILayout.LabelField("Developer IDs", EditorStyles.boldLabel);

        //Scrollable List of Current Dev ID's
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        for (int i = 0; i < devIDs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            //Editable Text Field For Each Dev ID
            devIDs[i] = EditorGUILayout.TextField(devIDs[i]);

            //Remove Button to Delete This Dev ID
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                devIDs.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        //Input Field & Button to Add a New Dev ID
        EditorGUILayout.LabelField("Add New Developer ID", EditorStyles.boldLabel);
        newID = EditorGUILayout.TextField(newID);

        EditorGUILayout.BeginHorizontal();

        //Add Button -> Add New ID If Not Empty/ Duplicate
        if (GUILayout.Button("Add") && !string.IsNullOrEmpty(newID))
        {
            if (!devIDs.Contains(newID))
            {
                devIDs.Add(newID);
                newID = "";
            }
            else
            {
                EditorUtility.DisplayDialog("Duplicate ID", "This ID already exists.", "OK");
            }
        }

        //Save Button to Encrypt & Write IDs to File
        if (GUILayout.Button("Save"))
        {
            SaveDevIDs();
        }
        EditorGUILayout.EndHorizontal();
    }
}