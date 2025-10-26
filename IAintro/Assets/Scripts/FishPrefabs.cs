using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "FishPrefabsDatabase", menuName = "Pecera/FishPrefabsDatabase")]
public class FishPrefabs : ScriptableObject
{
    public FishEntry[] arrayPrefabs;
    public List<int> fishAmount = new List<int>();

#if UNITY_EDITOR
    private void OnValidate() // Sucede cada vez que se cambia algo en el editor.
    {
        foreach (var entry in arrayPrefabs)
        {
            if (entry.prefab != null)
            {
                var spriteRenderer = entry.prefab.GetComponentInChildren<SpriteRenderer>(true);

                if (spriteRenderer != null) entry.previewSprite = spriteRenderer.sprite;
                else
                {
                    entry.previewSprite = null;
                }
            }
            else
            {
                entry.previewSprite = null;
            }

        }
    }
#endif
}

[System.Serializable] 
public class FishEntry
{
    public enum FishType{FISH, BIG, LITTLE};

    public FishType label;
    public GameObject prefab;
    public Sprite previewSprite;
}