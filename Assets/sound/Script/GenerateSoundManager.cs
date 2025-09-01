using UnityEngine;

public class GenerateSoundManager : MonoBehaviour
{
    // Instance so that there is only one GenerateSoundManager.
    public static GenerateSoundManager Instance { get; private set; }

    // Call Initialize in PlayMode
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            // Load by name search under the assumption that SoundManager is available in the CRIWARE folder of the Resources folder.
            var prefab = Resources.Load<GameObject>("CRIWARE/SoundManager");
            if (prefab == null)
            {
                // If the search fails (SoundManager.prefab does not exist in Assets/Resources/CRIWARE/), error
                Debug.LogError("[GenerateSoundManager] SoundManager not found in Editor Default Resources folder.");
                return;
            }

            // Generate SoundManager
            var soundManager = Instantiate(prefab);
            soundManager.AddComponent<GenerateSoundManager>();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        

    }
}

