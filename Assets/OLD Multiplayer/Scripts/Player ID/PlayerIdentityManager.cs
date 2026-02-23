using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class PlayerIdentityManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI playerIdText;

    //The Real AES Key & IV Bytes Will be Stored Here After Deobfuscation at Runtime
    private static byte[] key;
    //The IV (Initilization Vector) Randomizes The Start of The Encryption Process so It Looks Different Every Time
    private static byte[] iv;

    //Store Loaded/ Generated Player ID
    private string playerID;
    bool isShowingFullID = false;

    //Static Constructor Runs Once When The Class is First Used
    //Deobfuscates The Key & IV Arrays so They Can be Used For Encryption/ Decryption
    static PlayerIdentityManager()
    {
        key = DeobfuscateKey();
        iv = DeobfuscateIV();
    }

    //These Are The Obfuscated AES Key & IV bytes
    //Each Byte Here is XORed (Exclusive OR) With 0x5A (Mask Byte) to Hide The Real Key in The Source Code
    //If The Bits Are Different (0 & 1) -> XOR Returns 1
    //If The Bits Are The Same (0 & 0 or 1 & 1) -> XOR Returns 0
    private static readonly byte[] obfuscatedKey = new byte[] { 0x1F, 0x2C, 0x35, 0x67, 0x28, 0x1B, 0x7D, 0x4E, 0x5A, 0x3B, 0x0F, 0x7A, 0x6C, 0x2D, 0x11, 0x5B };
    private static readonly byte[] obfuscatedIV = new byte[] { 0x3A, 0x0B, 0x1D, 0x4C, 0x7A, 0x2E, 0x51, 0x39, 0x6F, 0x0A, 0x4B, 0x7D, 0x2E, 0x11, 0x3F, 0x22 };

    private static byte[] DeobfuscateKey()
    {
        //Reconstruct The Original AES Key by Reversing The XOR Operation
        byte[] realKey = new byte[obfuscatedKey.Length];
        for (int i = 0; i < obfuscatedKey.Length; i++)
        {
            //XOR Each Byte With 0x5A (Mask Byte) to Get Original Byte
            realKey[i] = (byte)(obfuscatedKey[i] ^ 0x5A);
        }
        return realKey;
    }

    private static byte[] DeobfuscateIV()
    {
        //Reconstruct The Original AES IV by Reversing The XOR Operation
        byte[] realIV = new byte[obfuscatedIV.Length];
        for (int i = 0; i < obfuscatedIV.Length; i++)
        {
            //XOR Each Byte With 0x5A (Mask Byte) to Get Original Byte
            realIV[i] = (byte)(obfuscatedIV[i] ^ 0x5A);
        }
        return realIV;
    }

    #region ID Creation/ Loading
    void Awake()
    {
        LoadOrGenerateID();

        //Start With Player ID Hidden
        playerIdText.text = "Show ID";

        var button = playerIdText.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(TogglePlayerIDDisplay);
        }
        else
        {
            //Add Button If One Doesn't Exist
            var btn = playerIdText.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(TogglePlayerIDDisplay);
        }
    }

    public void LoadOrGenerateID()
    {
        //Loads The Player ID From Disk If It Exists -> Otherwise Generates A New One & Saves It Encrypted
        //Path Where The Player ID JSON File Will be Stored Locally
        string idFilePath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "playerid.json");

        if (System.IO.File.Exists(idFilePath))
        {
            //Read The Encrypted ID From File & Decrypt It
            string encrypted = System.IO.File.ReadAllText(idFilePath);
            playerID = Decrypt(encrypted);
            UnityEngine.Debug.Log($"Loaded PlayerID: {playerID}");
        }
        else
        {
            //Generate New GUID & Save Encrypted
            playerID = Guid.NewGuid().ToString();

            //Encrypt & Save It to File so It Persists
            string encrypted = Encrypt(playerID);
            System.IO.File.WriteAllText(idFilePath, encrypted);

            UnityEngine.Debug.Log($"Generated & Saved PlayerID: {playerID}");
        }
    }

    private void TogglePlayerIDDisplay()
    {
        isShowingFullID = !isShowingFullID;

        if (isShowingFullID)
        {
            playerIdText.text = $"ID: {playerID}";
        }
        else
        {
            playerIdText.text = "Show ID";
        }
    }
    #endregion

    #region Encryption & Decryption
    private string Encrypt(string plain)
    {
        //Encrypts A Plaintext String Using AES With The Deobfuscated Key & IV
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        //Convert The Encrypted Bytes to A Base64 String For Storage
        return Convert.ToBase64String(encryptedBytes);
    }

    private string Decrypt(string encrypted)
    {
        //Decrypts an Encrypted Base64 String Back to Plaintext Using AES With The Deobfuscated Key & IV
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        byte[] encryptedBytes = Convert.FromBase64String(encrypted);
        byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
    #endregion

    //Getter For PlayerID
    public string PlayerID => playerID;
}