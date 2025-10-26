using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class FishManager : MonoBehaviour
{
    #region VARS    
    [Header("Database")]
    [SerializeField] public FishPrefabs fishDatabase;

    [Header ("UI")]
    [SerializeField] Slider fishAmountSlider;
    [SerializeField] RectTransform fishTank;

    [Header("Fish Prefabs")]
    List<int> currentFishAmount = new List<int>();
    [SerializeField] int fishStartingNum = 1;
    public List<GameObject> allFishes = new List<GameObject>();
    #endregion

    #region SINGLETON
    static FishManager fishManagerObj;
    public static FishManager instance
    {
        get
        {
            return RequestFishManager();
        }
    }

    private static FishManager RequestFishManager()
    {
        if (!fishManagerObj)
        {
            GameObject steeringObstacleManagerObj = new GameObject("FishManager");
            fishManagerObj = steeringObstacleManagerObj.AddComponent<FishManager>();
        }
        return fishManagerObj;
    }

    private void Awake()
    {
        if (fishManagerObj == null)
        {
            fishManagerObj = this;

        }
        else if (fishManagerObj != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        //fishAmountSlider.wholeNumbers = true; "Forzamos a que solo de enteros. Lo he hecho desde el inspector"
        currentFishAmount = new List<int>(fishDatabase.fishAmount);
        FishAmountChanged(fishStartingNum);
    }

    private void OnEnable()
    {
        fishAmountSlider.onValueChanged.AddListener(FishAmountChanged);
        if (fishStartingNum < 1) fishStartingNum = 1;
    }
    private void OnDisable()
    {
       fishAmountSlider.onValueChanged.RemoveListener(FishAmountChanged);
    }

    #region SPAWN_FISH
    /// <summary>
    /// Método que recibe la cantidad de peces a instanciar.
    /// </summary>
    /// <param name="fishNum"></param>
    private void FishAmountChanged(float fishNum)
    {
        SpawnFish(fishNum);
        UI_Manager.instance.UpdateFishAmountUI(currentFishAmount);
    }

    /// <summary>
    /// Método que hace la instanciación del pez.
    /// </summary>
    /// <param name="fishNum"></param>
    void SpawnFish(float fishNum)
    {
        currentFishAmount = new List<int>(new int[fishDatabase.arrayPrefabs.Length]);
        foreach (Transform child in fishTank)
        {
            Destroy(child.gameObject);
        }
        allFishes.Clear();

        for (int i = 0; i < fishNum; i++)
        {
            int fishType = Random.Range(0, fishDatabase.arrayPrefabs.Length);
            GameObject prefab = fishDatabase.arrayPrefabs[fishType].prefab;
            GameObject fish = Instantiate(prefab, fishTank);
            SteeringBehaviour entry = fish.GetComponent<SteeringBehaviour>();
            entry.fishType = fishDatabase.arrayPrefabs[fishType].label;
            currentFishAmount[fishType]++;
            allFishes.Add(fish);

            Transform fishRect = fish.GetComponent<Transform>();
            if (fishRect == null)
            {
                Debug.LogWarning("Prefab no tiene RectTransform!");
                continue;
            }

            // Obtenemos el tamaño de la pecera
            Vector2 tankSize = fishTank.rect.size;


            // Generamos posición local dentro de los límites
            float x = Random.Range(-tankSize.x / 3 /* +fishSize.x / 2*/, tankSize.x / 3 /*- fishSize.x / 2*/);
            float y = Random.Range(-tankSize.y / 3 /*+ fishSize.y / 2*/, tankSize.y / 3 /*- fishSize.y / 2*/);

            fishRect.localPosition = new Vector3(x, y, Random.Range(0f, 3f));
        }
    }

    #endregion

}
