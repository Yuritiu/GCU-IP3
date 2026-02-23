using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DeveloperIdentityManager : MonoBehaviour
{
    private static byte[] key;
    private static byte[] iv;

    //Same Obfuscation as PlayerIdentityManager Key
    private static readonly byte[] obfuscatedKey = new byte[] {0x1F, 0x2C, 0x35, 0x67, 0x28, 0x1B, 0x7D, 0x4E, 0x5A, 0x3B, 0x0F, 0x7A, 0x6C, 0x2D, 0x11, 0x5B };
    private static readonly byte[] obfuscatedIV = new byte[] {0x3A, 0x0B, 0x1D, 0x4C, 0x7A, 0x2E, 0x51, 0x39, 0x6F, 0x0A, 0x4B, 0x7D, 0x2E, 0x11, 0x3F, 0x22 };

    //List of Developer ID's
    private HashSet<string> devIDs = new HashSet<string>();

    static DeveloperIdentityManager()
    {
        key = DeobfuscateKey();
        iv = DeobfuscateIV();
    }

    private static byte[] DeobfuscateKey()
    {
        byte[] realKey = new byte[obfuscatedKey.Length];
        for (int i = 0; i < obfuscatedKey.Length; i++)
            realKey[i] = (byte)(obfuscatedKey[i] ^ 0x5A);
        return realKey;
    }

    private static byte[] DeobfuscateIV()
    {
        byte[] realIV = new byte[obfuscatedIV.Length];
        for (int i = 0; i < obfuscatedIV.Length; i++)
            realIV[i] = (byte)(obfuscatedIV[i] ^ 0x5A);
        return realIV;
    }

    private void Awake()
    {
        LoadDevIDs();
    }

    #region Loading/ Saving Dev ID's
    private void LoadDevIDs()
    {
        //Loads & Decrypts Dev IDs From Encrypted File/ Creates a Default File If Missing
        string path = Path.Combine(Application.persistentDataPath, "devids.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("Dev ID file not found. Creating default empty list.");
            //Creates an Empty Encrypted File
            SaveDevIDs(new List<string>());
            return;
        }

        try
        {
            string encrypted = File.ReadAllText(path);
            string json = Decrypt(encrypted);
            DevIDListWrapper wrapper = JsonUtility.FromJson<DevIDListWrapper>(json);
            if (wrapper != null && wrapper.ids != null)
            {
                devIDs = new HashSet<string>(wrapper.ids);
                Debug.Log($"Loaded {devIDs.Count} developer IDs.");
            }
            else
            {
                Debug.LogWarning("Dev ID file empty or corrupt, initializing empty list.");
                devIDs = new HashSet<string>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load developer IDs: {e}");
            devIDs = new HashSet<string>();
        }
    }

    private void SaveDevIDs(List<string> ids)
    {
        //Saves & Encrypts Dev IDs to File
        string path = Path.Combine(Application.persistentDataPath, "devids.json");
        string json = JsonUtility.ToJson(new DevIDListWrapper { ids = ids });
        string encrypted = Encrypt(json);
        File.WriteAllText(path, encrypted);
    }
    #endregion

    public bool IsDeveloper(string playerID)
    {
        return devIDs.Contains(playerID);
    }

    #region Called From DeveloperIDManagerEditor
    public void AddDeveloperID(string newID)
    {
        //Add New Dev ID & Save File
        if (devIDs.Add(newID))
        {
            SaveDevIDs(new List<string>(devIDs));
            Debug.Log($"Added new developer ID: {newID}");
        }
    }

    public void RemoveDeveloperID(string id)
    {
        //Remove Dev ID & Save File
        if (devIDs.Remove(id))
        {
            SaveDevIDs(new List<string>(devIDs));
            Debug.Log($"Removed developer ID: {id}");
        }
        else
        {
            Debug.LogWarning($"Developer ID not found: {id}");
        }
    }
    #endregion

    private string Encrypt(string plain)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    private string Decrypt(string encrypted)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        byte[] encryptedBytes = Convert.FromBase64String(encrypted);
        byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    [Serializable]
    private class DevIDListWrapper
    {
        public List<string> ids = new List<string>();
    }

    #region Editor Functions
    public List<string> GetDevIDs()
    {
        return new List<string>(devIDs);
    }

    public void SaveDevIDsFromEditor(List<string> ids)
    {
        //Save a Dev ID List From Editor & Encrypt It
        devIDs = new HashSet<string>(ids);
        SaveDevIDs(ids);
    }
    #endregion
}