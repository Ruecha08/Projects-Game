using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isFalling = rb.velocity.y < -0.1f;
        animator.SetBool("isFalling", isFalling);

        bool isJumping = rb.velocity.y > 0.1f;
        animator.SetBool("isJumping", isJumping);
    }
}
