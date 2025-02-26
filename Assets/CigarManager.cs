using System.Collections.Generic;
using UnityEngine;

public class CigarManager : MonoBehaviour
{
    public static CigarManager Instance;

    [SerializeField] List<string> cardNames = new List<string>();
    [SerializeField] List<GameObject> cardPrefabs = new List<GameObject>();

    Dictionary<string, GameObject> cardPrefabDictionary = new Dictionary<string, GameObject>();

    void Awake()
    {
        Instance = this;

        //Check Both Lists Are Same Size
        if (cardNames.Count != cardPrefabs.Count)
        {
            Debug.LogError("Lists Aren;t Same Size");
            return;
        }

        //Convert Lists To Dictionary -> Faster Lookup
        for (int i = 0; i < cardNames.Count; i++)
        {
            if (!cardPrefabDictionary.ContainsKey(cardNames[i]) && cardPrefabs[i] != null)
            {
                cardPrefabDictionary.Add(cardNames[i], cardPrefabs[i]);
            }
        }
    }

    public GameObject GetCardPrefab(string cardName)
    {
        if (cardPrefabDictionary.TryGetValue(cardName, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogWarning("No card prefab found with the name: " + cardName);
        return null;
    }
}
