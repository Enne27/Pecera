using UnityEngine;

public class SplashSpawner : MonoBehaviour
{
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private RectTransform fishTankLimits; 

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;

            if (IsInsideAquarium(worldPos))
            {
                Instantiate(splashPrefab, worldPos, Quaternion.identity);
            }
        }
    }

    private bool IsInsideAquarium(Vector3 point)
    {
        // Calcula los límites del RectTransform en el mundo
        Vector3 min = fishTankLimits.position - (Vector3)(fishTankLimits.rect.size * 0.5f * fishTankLimits.lossyScale);
        Vector3 max = fishTankLimits.position + (Vector3)(fishTankLimits.rect.size * 0.5f * fishTankLimits.lossyScale);

        return (point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y);
    }
}