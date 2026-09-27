using UnityEngine;
using System.Collections;

public class RoomService : MonoBehaviour
{
    [Header("Musik Khusus Scene Ini")]
    public AudioClip sceneMusic;
    [Range(0, 1)] public float musicVolume = 1f;

    [Header("Pengaturan Respawn (Khusus Habis Mati / Start Game)")]
    [Tooltip("Centang ini HANYA di scene tempat player pertama kali spawn / respawn (seperti Village)")]
    [SerializeField] private bool useRespawnPosition = false;
    [SerializeField] private Vector2 respawnPosition;

    private IEnumerator Start()
    {
        // Pindahkan player HANYA JIKA:
        // 1. Tidak sedang teleport lewat portal biasa (!SceneChanger.isTeleporting)
        // 2. Scene ini diizinkan mengatur titik respawn (useRespawnPosition == true)
        if (!SceneChanger.isTeleporting && useRespawnPosition)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }

                // Masukkan player ke koordinat bebas pilihanmu
                player.transform.position = respawnPosition;
            }
        }

        // Reset status penanda teleportasi
        SceneChanger.isTeleporting = false;

        yield return null;

        // Putar musik khas scene ini
        if (sceneMusic != null)
        {
            AudioManager audioManager = ServiceLocator.Get<AudioManager>();
            if (audioManager != null)
            {
                audioManager.PlayMusic(sceneMusic, musicVolume);
            }
        }
    }
}