using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator healthTextAnim;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        // 1. Memastikan HP terisi kembali jika sebelumnya mati / 0
        if (StatsManager.Instance != null)
        {
            if (StatsManager.Instance.currentHealth <= 0)
            {
                StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
            }
        }

        // 2. Tampilkan UI HP terbaru saat spawn
        UpdateHealthUI();
    }

    private void OnEnable()
    {
        UpdateHealthUI();
    }

    public void ChangeHealth(int amount)
    {
        if (StatsManager.Instance == null) return;

        StatsManager.Instance.currentHealth += amount;

        if (healthTextAnim != null)
        {
            healthTextAnim.Play("TextUpdate");
        }

        UpdateHealthUI();

        // Logika saat Player Mati
        if (StatsManager.Instance.currentHealth <= 0)
        {
            // 1. Reset darah ke penuh
            StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;

            // 2. MATIKAN SAKLAR TELEPORT (Agar tidak bentrok dengan posisi respawn di Village)
            SceneChanger.isTeleporting = false;

            // 3. MATIKAN LAGU SECARA TOTAL SEBELUM PINDAH SCENE
            AudioManager audioManager = ServiceLocator.Get<AudioManager>();
            if (audioManager != null)
            {
                audioManager.StopMusic();
            }

            // 4. Balik ke Main Menu
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null && StatsManager.Instance != null)
        {
            healthText.text = "HP: " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
        }
    }
}