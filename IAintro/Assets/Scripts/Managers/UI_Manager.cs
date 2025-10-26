using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    #region SINGLETON
    static UI_Manager uiManager;
    public static UI_Manager instance
    {
        get
        {
            return RequestUIManager();
        }
    }

    private static UI_Manager RequestUIManager()
    {
        if (!uiManager)
        {
            GameObject steeringObstacleManagerObj = new GameObject("UI_manager");
            uiManager = steeringObstacleManagerObj.AddComponent<UI_Manager>();
        }
        return uiManager;
    }

    private void Awake()
    {
        if (uiManager == null)
        {
            uiManager = this;

        }
        else if (uiManager != this)
        {
            Destroy(gameObject);
        }

        GenerateFishTypeImage();
    }
    #endregion

    #region VARS
    [SerializeField] Slider fishAmountSlider;
    [SerializeField] TextMeshProUGUI fishTotalAmount;
    [SerializeField] TextMeshProUGUI fishAmountTypeText;
    [SerializeField] HorizontalLayoutGroup layout;
    #endregion

    public void UpdateFishAmountUI(List<int> fishAmount)
    {
        fishTotalAmount.text = "Fishes: " + fishAmountSlider.value.ToString();
        string info = "";
        FishPrefabs db = FishManager.instance.fishDatabase;

        for (int i = 0; i < db.arrayPrefabs.Length; i++)
        {
            string label = db.arrayPrefabs[i].label.ToString();
            info += $"{label}: {fishAmount[i]}";
            if (i < fishAmount.Count - 1) info += ", ";
        }

        fishAmountTypeText.text = info;
    }

    public void GenerateFishTypeImage()
    {
        FishPrefabs db = FishManager.instance.fishDatabase;

        foreach (FishEntry prefab in db.arrayPrefabs)
        {
            GameObject spriteFish = new GameObject(prefab.label.ToString(), typeof(Image));
            spriteFish.transform.SetParent(layout.transform, false);
            spriteFish.GetComponent<Image>().sprite = prefab.previewSprite;
        }
    }
}
