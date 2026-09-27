using UnityEngine;
using UnityEngine.SceneManagement;

public class UIVisibilityController : MonoBehaviour
{
    [Tooltip("Tulis nama scene Main Menu persis seperti di Build Settings")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Masukkan semua objek UI yang ingin disembunyikan di Main Menu")]
    public GameObject[] uiElementsToHide;

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
        // Mengecek apakah kita sedang berada di Main Menu
        bool isMainMenu = (scene.name == mainMenuSceneName);

        // Melakukan loop untuk menyembunyikan/memunculkan semua UI yang didaftarkan
        foreach (GameObject uiElement in uiElementsToHide)
        {
            if (uiElement != null)
            {
                // Jika isMainMenu true, maka SetActive menjadi false (kebalikannya)
                uiElement.SetActive(!isMainMenu);
            }
        }
    }
}