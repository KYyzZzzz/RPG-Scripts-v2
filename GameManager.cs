using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DialogueManager DialogueManager;
    public DialogueHistoryTracker DialogueHistoryTracker;
    public LocationHistoryTracker LocationHistoryTracker;
    public QuestManager QuestManager;

    [Header("Main Menu Settings")]
    [Tooltip("Nama scene Main Menu kamu persis di Build Settings")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Persistent Objects")]
    public GameObject[] persistentObjects;

    private bool isComingFromMainMenu = false;

    private void Awake()
    {
        if (Instance != null)
        {
            CleanUpAndDestroy();
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMainMenu = (scene.name == mainMenuSceneName);

        // 1. Matikan UI/Player jika di Main Menu, nyalakan jika di Gameplay
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!isMainMenu);
            }
        }

        if (isMainMenu)
        {
            isComingFromMainMenu = true;
        }
        else
        {
            // Mengembalikan waktu game ke normal jika sebelumnya di-pause saat mati
            Time.timeScale = 1f;

            RebindManagerReferences();

            // HANYA jalankan spawn & reset jika baru kembali dari Main Menu
            if (isComingFromMainMenu)
            {
                MovePlayerToSpawnAndReset();
                isComingFromMainMenu = false;
            }
        }
    }

    private void RebindManagerReferences()
    {
        if (DialogueManager == null)
            DialogueManager = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);

        if (DialogueHistoryTracker == null)
            DialogueHistoryTracker = FindFirstObjectByType<DialogueHistoryTracker>(FindObjectsInactive.Include);

        if (LocationHistoryTracker == null)
            LocationHistoryTracker = FindFirstObjectByType<LocationHistoryTracker>(FindObjectsInactive.Include);

        if (QuestManager == null)
            QuestManager = FindFirstObjectByType<QuestManager>(FindObjectsInactive.Include);
    }


    private void MovePlayerToSpawnAndReset()
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        GameObject playerObj = persistentObjects[0]; // Element 0 wajib Player

        if (playerObj != null)
        {
            Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();

            // Pindahkan Posisi ke SpawnPoint
            if (spawnPoint != null)
            {
                if (rb != null)
                {
                    // Set posisi lewat Rigidbody2D, bukan transform langsung,
                    // supaya physics engine tetap sinkron
                    rb.position = spawnPoint.transform.position;
                }
                else
                {
                    playerObj.transform.position = spawnPoint.transform.position;
                }
            }

            // RESET 1: Reset Fisika Rigidbody2D
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            // PAKSA physics engine sinkron sekarang juga, supaya semua
            // trigger collider di sekitar spawn point (NPC, item, dll)
            // langsung terdeteksi tanpa nunggu 1 frame fisik lagi
            Physics2D.SyncTransforms();

            // RESET 2: Paksa Nyalakan Semua Script di Objek Player
            MonoBehaviour[] scripts = playerObj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null)
                {
                    script.enabled = true;
                }
            }

            // RESET 3: Reset State Animasi (agar tidak nyangkut di animasi mati)
            Animator anim = playerObj.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
            }
        }
    }

    private void MarkPersistentObjects()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }

        Destroy(gameObject);
    }
}