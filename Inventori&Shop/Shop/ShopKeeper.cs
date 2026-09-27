using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;

    public Animator anim;
    public CanvasGroup shopCanvasGroup;
    public ShopManager shopManager;

    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmour;

    [SerializeField] private Camera shopkeeperCam;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -1);

    public static event Action<ShopManager, bool> OnShopStateChanged;
    private bool playerInRange;
    private bool isShopOpen;

    private void Start()
    {
        // Set referensi static ke ShopKeeper yang aktif di scene Village saat ini
        currentShopKeeper = this;
        RebindReferences();
    }

    private void OnDestroy()
    {
        // Bersihkan memori static jika scene Village dihancurkan (misal saat mati / balik ke Main Menu)
        if (currentShopKeeper == this)
        {
            currentShopKeeper = null;
        }
    }

    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                // Cari ulang referensi jika terputus/missing akibat reload scene
                RebindReferences();

                if (!isShopOpen)
                {
                    OpenShop();
                }
                else
                {
                    CloseShop();
                }
            }
        }
    }

    private void OpenShop()
    {
        Time.timeScale = 0;
        currentShopKeeper = this;
        isShopOpen = true;

        if (shopManager != null)
        {
            OnShopStateChanged?.Invoke(shopManager, true);
        }

        if (shopCanvasGroup != null)
        {
            shopCanvasGroup.alpha = 1;
            shopCanvasGroup.blocksRaycasts = true;
            shopCanvasGroup.interactable = true;
        }

        if (shopkeeperCam != null)
        {
            shopkeeperCam.transform.position = transform.position + cameraOffset;
            shopkeeperCam.gameObject.SetActive(true);
        }

        OpenItemShop();
    }

    public void CloseShop()
    {
        Time.timeScale = 1;
        isShopOpen = false;

        if (shopManager != null)
        {
            OnShopStateChanged?.Invoke(shopManager, false);
        }

        if (shopCanvasGroup != null)
        {
            shopCanvasGroup.alpha = 0;
            shopCanvasGroup.blocksRaycasts = false;
            shopCanvasGroup.interactable = false;
        }

        if (shopkeeperCam != null)
        {
            // Matikan atau biarkan kamera shop (sesuaikan jika perlu)
            shopkeeperCam.gameObject.SetActive(true);
        }
    }

    public void OpenItemShop()
    {
        if (shopManager != null)
            shopManager.PopulateShopItems(shopItems);
    }

    public void OpenWeaponShop()
    {
        if (shopManager != null)
            shopManager.PopulateShopItems(shopWeapons);
    }

    public void OpenArmourShop()
    {
        if (shopManager != null)
            shopManager.PopulateShopItems(shopArmour);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim != null) anim.SetBool("playerInRange", true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim != null) anim.SetBool("playerInRange", false);
            playerInRange = false;

            // Jika player jalan menjauh saat shop terbuka, otomatis tutup UI shop
            if (isShopOpen)
            {
                CloseShop();
            }
        }
    }

    // Fungsi otomatis untuk memasang kembali komponen UI/Kamera yang Missing
    private void RebindReferences()
    {
        if (shopManager == null)
        {
            shopManager = FindFirstObjectByType<ShopManager>();
        }

        if (shopCanvasGroup == null && shopManager != null)
        {
            shopCanvasGroup = shopManager.GetComponent<CanvasGroup>();
        }

        if (shopkeeperCam == null)
        {
            shopkeeperCam = Camera.main;
        }
    }
}