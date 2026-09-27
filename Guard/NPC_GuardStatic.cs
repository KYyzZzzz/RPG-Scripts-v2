using UnityEngine;

public class NPC_GuardStatic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectRange = 5f;

    private void Start()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        // Cari player otomatis jika belum di-assign di Inspector
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Cek jarak dengan player
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance <= detectRange)
        {
            // Menoleh ke arah player
            if (playerTransform.position.x > transform.position.x)
            {
                sr.flipX = false; // Menghadap kanan (sesuaikan dengan orientasi default sprite)
            }
            else
            {
                sr.flipX = true;  // Menghadap kiri
            }
        }
    }
}
