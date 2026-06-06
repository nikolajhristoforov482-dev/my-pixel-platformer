using UnityEngine;
using UnityEngine.SceneManagement; // Подключаем перезапуск сцены при смерти

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;
    public float jumpForce = 14.5f;

    [Header("Здоровье и Урон")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isHurt = false; // Блокирует управление, пока проигрывается анимация урона
    private float hurtDuration = 0.35f; // Длина анимации Hit (7 кадров)

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D myCollider;
    private Animator animator;

    private int jumpCount = 0;
    private bool isGrounded;
    private bool wasGrounded;
    private float moveInput;
    private Sprite fallSprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth; // На старте жизни полные

        string path = "Assets/Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Fall (32x32).png";
#if UNITY_EDITOR
        Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (Object o in assets) { if (o is Sprite) { fallSprite = (Sprite)o; break; } }
#endif
    }

    void Update()
    {
        // Если игрока только что ударило — отключаем ввод с клавиатуры на долю секунды
        if (isHurt) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && spriteRenderer != null) spriteRenderer.flipX = false;
        else if (moveInput < 0 && spriteRenderer != null) spriteRenderer.flipX = true;

        isGrounded = CheckIfOnGround();

        if (isGrounded && !wasGrounded && rb.velocity.y <= 0.2f)
        {
            jumpCount = 0;
            if (animator != null)
            {
                animator.enabled = true;
                if (moveInput != 0) animator.Play("Run");
                else animator.Play("Idle");
            }
        }
        wasGrounded = isGrounded;

        if (!isGrounded && rb != null)
        {
            AnimatorStateInfo stateInfo = animator != null ? animator.GetCurrentAnimatorStateInfo(0) : default;
            bool isDoubleJumpingRightNow = stateInfo.IsName("DoubleJump") && stateInfo.normalizedTime < 1.0f;

            if (rb.velocity.y < -0.1f && !isDoubleJumpingRightNow)
            {
                if (animator != null) animator.enabled = false;
                if (spriteRenderer != null && fallSprite != null) spriteRenderer.sprite = fallSprite;
            }
            if (animator != null) animator.SetBool("IsRunning", false);
        }
        else if (isGrounded && animator != null)
        {
            animator.SetBool("IsRunning", moveInput != 0);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && jumpCount == 0)
            {
                jumpCount = 1;
                ExecuteJump("Jump");
            }
            else if (!isGrounded && jumpCount == 1)
            {
                jumpCount = 2;
                ExecuteJump("DoubleJump");
            }
        }
    }

    void FixedUpdate()
    {
        if (isHurt) return;

        if (rb != null)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    private void ExecuteJump(string animationName)
    {
        if (animator != null) animator.enabled = true;
        if (rb != null) rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        if (animator != null) animator.Play(animationName);
    }

    // ФУНКЦИЯ ПОЛУЧЕНИЯ УРОНА ОТ ШИПОВ
    public void TakeDamage(int damage)
    {
        if (isHurt) return; // Если уже неуязвимы — игнорируем повторный урон

        currentHealth -= damage;
        Debug.Log("Ай! Опасность! Осталось жизней: " + currentHealth);

        if (currentHealth <= 0)
        {
            // Если жизни кончились — мгновенно перезапускаем уровень
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        // Включаем режим анимации Hit
        isHurt = true;
        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("Hit"); // Запускаем созданную нами ранее анимацию урона!
        }

        // Небольшой физический отскок лягушонка вверх от шипов
        if (rb != null)
        {
            rb.velocity = new Vector2(spriteRenderer.flipX ? 4f : -4f, 8f);
        }

        // Через время анимации возвращаем управление игроку
        Invoke("ResetHurt", hurtDuration);
    }

    private void ResetHurt()
    {
        isHurt = false;
    }

    private bool CheckIfOnGround()
    {
        if (myCollider == null) return false;
        float radius = myCollider.size.x * 0.35f;
        Vector2 rayStart = new Vector2(myCollider.bounds.center.x, myCollider.bounds.min.y);
        int layerMask = LayerMask.GetMask("Ignore Raycast");
        Collider2D hit = Physics2D.OverlapCircle(rayStart, radius, layerMask);
        return hit != null;
    }
}
