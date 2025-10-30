using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main_Menu_Manager : MonoBehaviour
{
    [SerializeField] public GameObject ProfileCanvas;
    [SerializeField] public GameObject OptionCanvas;
    [SerializeField] public GameObject ValidationCanvas;

    [SerializeField] public TMP_Text ProgressionSave_Text1;
    [SerializeField] public TMP_Text ProgressionSave_Text2;
    [SerializeField] public TMP_Text ProgressionSave_Text3;

    [SerializeField] public Slider MasterVolumeSlider;

    private int Index_Save_To_Delete;

    //Option
    private float MasterVolumeFloat;

    public void OpenProfileWindow()
    {
        UpdateProfile();
        ProfileCanvas.SetActive(true);
    }

    public void CloseProfileWindow()
    {
        ProfileCanvas.SetActive(false);
    }

    public void OpenOptionWindow()
    {
        UpdateOption();
        OptionCanvas.SetActive(true);
    }

    public void CloseOptionWindow()
    {
        DataBase.Instance.UpdateOptions();
        OptionCanvas.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OpenValidationWindow(int index)
    {
        Index_Save_To_Delete = index;
        ValidationCanvas.SetActive(true);
    }

    public void CloseValidationWindow()
    {
        ValidationCanvas.SetActive(false);
    }

    public void UpdateProfile()
    {
        StartCoroutine(UpdateProfileText(1, ProgressionSave_Text1));
        StartCoroutine(UpdateProfileText(2, ProgressionSave_Text2));
        StartCoroutine(UpdateProfileText(3, ProgressionSave_Text3));
    }

    IEnumerator UpdateProfileText(int profileIndex, TMP_Text targetText)
    {
        SaveSystem.Instance.LoadGame(profileIndex, saveFile =>
        {
            if (saveFile == null)
            {
                targetText.text = "New Game";
            }
            else
            {
                targetText.text = "Stage " + saveFile.CurrentStage;
            }
        });
        yield return null;
    }

    public void StartGame(int profileNumber)
    {
        SaveSystem.Instance.LoadGame(profileNumber, saveFile =>
        {
            if (saveFile != null)
            {
                SaveHolder.Instance.saveFile = saveFile;
                SaveHolder.Instance.SaveProfile = profileNumber;
                RestoreLoadedData();
                #if UNITY_EDITOR
                UnityEditor.Selection.activeObject = null;
                #endif
                SceneManager.LoadScene("TransitionScene");
            }
            else
            {
                SaveHolder.Instance.saveFile = new SaveFile();
                SaveHolder.Instance.SaveProfile = profileNumber;
                #if UNITY_EDITOR
                UnityEditor.Selection.activeObject = null;
                #endif
                SceneManager.LoadScene("TransitionScene");
            }
        });
    }

    public void DeleteSave()
    {
        string saveFilePath = Application.persistentDataPath + "/save" + Index_Save_To_Delete + ".dat";

        if (System.IO.File.Exists(saveFilePath))
        {
            System.IO.File.Delete(saveFilePath);
            Debug.Log("[SaveSystem] ✅ Save file deleted");
        }
        else
        {
            Debug.LogWarning("[SaveSystem] ⚠ No save file found to delete");
        }
        UpdateProfile();
        ValidationCanvas.SetActive(false);
    }

    public void RestoreLoadedData()
    {
        DataBase.Instance.CurrentStage = SaveHolder.Instance.saveFile.CurrentStage;
        DataBase.Instance.DeckList = SaveHolder.Instance.saveFile.DeckList;
        DataBase.Instance.Money = SaveHolder.Instance.saveFile.Money;
        DataBase.Instance.CoreLife = SaveHolder.Instance.saveFile.CoreLife;
    }

    public void SetOptions()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolumeFloat);
        DataBase.Instance.UpdateOptions();
    }
    
    public void UpdateOption()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            MasterVolumeSlider.value = 1f;
            PlayerPrefs.SetFloat("MasterVolume", 1f);
        }
    }

    public void SetMasterVolume()
    {
        MasterVolumeFloat = MasterVolumeSlider.value;
        DataBase.Instance.MasterVolume = MasterVolumeFloat;
    }
}
