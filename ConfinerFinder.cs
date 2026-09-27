using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfinerFinder : MonoBehaviour
{
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
        // 1. Jika di Main Menu, abaikan pencarian confiner agar tidak error
        if (scene.name == "MainMenu")
        {
            return;
        }

        // 2. Ambil komponen CinemachineConfiner2D di objek ini
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
        if (confiner == null) return;

        // 3. Cari objek ber-tag "Confiner" di scene
        GameObject confinerObj = GameObject.FindWithTag("Confiner");

        // 4. Pastikan objeknya ditemukan dan memiliki komponen PolygonCollider2D sebelum dipasang
        if (confinerObj != null)
        {
            PolygonCollider2D polygonCollider = confinerObj.GetComponent<PolygonCollider2D>();
            if (polygonCollider != null)
            {
                confiner.BoundingShape2D = polygonCollider;

                // Opsional: Untuk Cinemachine versi baru, panggil InvalidateBoundingShapeCache jika batas tidak otomatis berubah
                confiner.InvalidateBoundingShapeCache();
            }
            else
            {
                Debug.LogWarning("Objek dengan tag 'Confiner' ditemukan, tetapi tidak memiliki komponen PolygonCollider2D!");
            }
        }
        else
        {
            Debug.LogWarning("Tidak ditemukan GameObject dengan tag 'Confiner' di scene ini!");
        }
    }
}