using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class G_SettingsController : MonoBehaviour
{
    [SerializeField] GameObject settingObject;
    [SerializeField] Transform tabsTransform;
    [SerializeField] Transform settingsTransform;
    [SerializeField] TMP_Text tabText;
    [SerializeField] string[] tabNames = new string[4];
    [SerializeField] GameObject[] firstSelectedObjects = new GameObject[4];

    private List<GameObject> settingTabs = new List<GameObject>();

    private GameManager gameManager;

    private GameObject lastSelectedObject;

    public static G_SettingsController instance;


    private void OnValidate()
    {
        if (tabsTransform.childCount != tabNames.Length) { return; }

        for(int i = 0; i < tabNames.Length; i++)
        {
            tabsTransform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = tabNames[i];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        gameManager = GetGameManager();

        if (tabsTransform.childCount != settingsTransform.childCount)
        {
            Debug.LogError("Error:The number of \"Tabs\" and the number of \"Settings\" do not match",this);
            Destroy(this);
        }

        settingTabs.Clear();

        for(int i = 0; i < tabsTransform.childCount; i++)
        {
            int tabNumber = i;
            settingTabs.Add(settingsTransform.GetChild(i).gameObject);
            tabsTransform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => SelectTab(tabNumber));
        }

        settingObject.SetActive(false);
    }

    public void OpenSetting()
    {
        lastSelectedObject = GetGameManager().GetSelected();
        settingObject.SetActive(true);
        gameManager.SetSelected(gameManager.settingDefault);
        SelectTab(0);
        GetGameManager().WaitCancelButton(lastSelectedObject, settingObject);
    }

    private GameManager GetGameManager()
    {
        if (!gameManager)
        {
            gameManager = GetComponent<GameManager>();
        }
        return gameManager;
    }

    public void SelectTab(int tabNumber)
    {
        if(tabNumber >= settingTabs.Count)
        {
            Debug.LogError("Error:OutOfRangeException.\nSelected number \"" + tabNumber + "\" is too big",this);
            return;
        }
        tabText.text = tabNames[tabNumber];
        for(int i = 0; i < settingTabs.Count; i++)
        {
            settingTabs[i].SetActive(i == tabNumber);
        }
    }

    public void OnTabClick(int tabNumber)
    {
        GetGameManager().SetSelected(firstSelectedObjects[tabNumber]);
        GetGameManager().WaitCancelButton(gameManager.settingDefault);
    }


}
