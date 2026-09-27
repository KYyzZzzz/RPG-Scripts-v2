using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int facingDirection = 1;

    public Rigidbody2D rb;
    public Animator anim;

    private bool isKnockedBack;

    public Player_Combat player_Combat;

    // --- TAMBAHAN KODE DI SINI ---
    private void OnEnable()
    {
        // Reset status knockback saat player diaktifkan kembali
        isKnockedBack = false;

        // Reset kecepatan rigidbody jika tersisa gaya dorong
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    // ----------------------------

    private void Update()
    {
        if (Input.GetButtonDown("Slash") && player_Combat != null && player_Combat.enabled == true)
        {
            player_Combat.Attack();
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack == false)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal > 0 && transform.localScale.x < 0 ||
                horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }
            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            rb.linearVelocity = new Vector2(horizontal, vertical) * StatsManager.Instance.speed;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        if (!gameObject.activeInHierarchy) return;
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
}