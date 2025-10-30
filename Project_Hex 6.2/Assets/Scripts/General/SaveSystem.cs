using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Threading.Tasks;

public class SaveSystem : Singleton<SaveSystem>
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit AES
    private const int Iterations = 100_000;
    private const string Password = "TonMotDePasseSuperSecret123!";

    [SerializeField] public SpinLoader Loader;

    public void SaveGame()
    {
        if (Loader != null)
        {
            Loader.gameObject.SetActive(true);
            Loader.isRunning = true;
        }

        Debug.Log("[SaveSystem] ▶ SaveGame called");
        int profileIndex = SaveHolder.Instance.SaveProfile;

        // Étape 1 : Snapshot (thread principal)
        SnapshotGameSave(false);
        SaveFile saveFile = SaveHolder.Instance.saveFile;
        string json = JsonUtility.ToJson(saveFile);
        string path = Application.persistentDataPath + "/save" + profileIndex + ".dat";

        // Étape 2 : Save en arrière-plan
        StartCoroutine(SaveDataCoroutine(path, json));
    }

    private void SaveEncryptedFile(string json, string path)
    {
        try
        {
            byte[] salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            using var keyDerivation = new Rfc2898DeriveBytes(Password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = keyDerivation.GetBytes(KeySize);
            byte[] iv = keyDerivation.GetBytes(16);

            byte[] encryptedData = EncryptStringToBytes_Aes(json, key, iv);
            byte[] finalData = new byte[salt.Length + encryptedData.Length];
            Buffer.BlockCopy(salt, 0, finalData, 0, salt.Length);
            Buffer.BlockCopy(encryptedData, 0, finalData, salt.Length, encryptedData.Length);

            File.WriteAllBytes(path, finalData);
            Debug.Log($"✅ Save written to: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Failed to save file: " + ex.Message);
        }
    }

    private IEnumerator SaveDataCoroutine(string path, string json)
    {
        var task = Task.Run(() => SaveEncryptedFile(json, path));
        yield return new WaitUntil(() => task.IsCompleted);
        if (Loader != null)
        {
            Loader.isRunning = false;
            Loader.gameObject.SetActive(false);
        }
    }

    public void LoadGame(int profileIndex, Action<SaveFile> onLoaded)
    {
        if (Loader != null)
        {
            Loader.gameObject.SetActive(true);
            Loader.isRunning = true;
        }
        string path = Application.persistentDataPath + "/save" + profileIndex + ".dat";
        StartCoroutine(LoadDataCoroutine(path, onLoaded));
    }

    private IEnumerator LoadDataCoroutine(string path, Action<SaveFile> onLoaded)
    {
        var task = Task.Run(() => LoadDataFromPath(path));
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("❌ Load failed: " + task.Exception);
            onLoaded?.Invoke(null);
        }
        else
        {
            SaveFile result = task.Result;
            Debug.Log("✅ Save loaded successfully");
            onLoaded?.Invoke(result);
        }
        if (Loader != null)
        {
            Loader.isRunning = false;
            Loader.gameObject.SetActive(false);
        }
    }

    private SaveFile LoadDataFromPath(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning("[SaveSystem] Save file not found at: " + path);
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            if (fileData.Length <= SaltSize)
            {
                Debug.LogError("[SaveSystem] File too small. Possibly corrupted.");
                return null;
            }

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(fileData, 0, salt, 0, SaltSize);

            byte[] encryptedData = new byte[fileData.Length - SaltSize];
            Buffer.BlockCopy(fileData, SaltSize, encryptedData, 0, encryptedData.Length);

            using var keyDerivation = new Rfc2898DeriveBytes(Password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = keyDerivation.GetBytes(KeySize);
            byte[] iv = keyDerivation.GetBytes(16);

            string decryptedJson = DecryptStringFromBytes_Aes(encryptedData, key, iv);
            return JsonUtility.FromJson<SaveFile>(decryptedJson);
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Load error: " + e.Message);
            return null;
        }
    }

    private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;
        aesAlg.Padding = PaddingMode.PKCS7;

        using MemoryStream msEncrypt = new();
        using (CryptoStream csEncrypt = new(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText);
            swEncrypt.Flush();
            csEncrypt.FlushFinalBlock();
        }

        return msEncrypt.ToArray();
    }

    private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;
        aesAlg.Padding = PaddingMode.PKCS7;

        using MemoryStream msDecrypt = new(cipherText);
        using CryptoStream csDecrypt = new(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
    public void SnapshotGameSave(bool InFight)
    {
        Debug.Log("[SaveSystem] ▶ SnapshotGameSave called");

        //SaveHolder.Instance.saveFile.InFight = InFight;
        SaveHolder.Instance.saveFile.CurrentStage = DataBase.Instance.CurrentStage;
        SaveHolder.Instance.saveFile.DeckList = DataBase.Instance.DeckList;
        SaveHolder.Instance.saveFile.Money = DataBase.Instance.Money;
        SaveHolder.Instance.saveFile.CoreLife = DataBase.Instance.CoreLife;
        //SaveHolder.Instance.saveFile.currentTurn = CombatSystem.Instance.CurrentTurn;

        //SaveHolder.Instance.saveFile.currentEnemySlots = CombatSystem.Instance.Enemy_Permanents;
        //SaveHolder.Instance.saveFile.currentPermanentSlots = CombatSystem.Instance.Player_Permanents;

        //SaveHolder.Instance.saveFile.currentDrawPile = CardSystem.Instance.drawPile;
        //SaveHolder.Instance.saveFile.currentDiscard = CardSystem.Instance.discardPile;
        //SaveHolder.Instance.saveFile.currentHand = CardSystem.Instance.hand;

        //SaveHolder.Instance.saveFile.currentEnemyID = CombatSystem.Instance.currentEnemy.ID;

        //SaveHolder.Instance.saveFile.currentEnemySlotsIntent = CombatSystem.Instance.GetMobsIntentForSave();

        //SaveHolder.Instance.saveFile.currentMana = ManaSystem.Instance.currentMana;
        //SaveHolder.Instance.saveFile.Max_Mana = ManaSystem.Instance.MAX_MANA;


        //SaveHolder.Instance.saveFile.effectsByEvent = GameEventSystem.Instance.effectsByEvent;

        Debug.Log("[SaveSystem] ✅ SnapshotGameSave completed");
    }
}