using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float velocidad = 2f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int direccion = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Physics2D.queriesHitTriggers = false;
    }

    void FixedUpdate()
    {
        if (HayPared() || !HaySuelo())
        {
            direccion *= -1;
        }

        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
        spriteRenderer.flipX = direccion < 0;
        animator.SetFloat("Velocidad", Mathf.Abs(rb.linearVelocity.x));
    }

    bool HayPared()
    {
        float x = direccion > 0 ? col.bounds.max.x + 0.05f : col.bounds.min.x - 0.05f;
        Vector2 origen = new Vector2(x, col.bounds.center.y);
        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.right * direccion, 0.1f);

        return hit.collider != null
            && !hit.collider.CompareTag("Player")
            && !hit.collider.CompareTag("Enemy");
    }

    bool HaySuelo()
    {
        float x = direccion > 0 ? col.bounds.max.x + 0.05f : col.bounds.min.x - 0.05f;
        Vector2 origen = new Vector2(x, col.bounds.min.y + 0.1f);
        return Physics2D.Raycast(origen, Vector2.down, 0.5f).collider != null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        MovimientoPersonaje jugador = collision.gameObject.GetComponent<MovimientoPersonaje>();

        if (jugador != null)
        {
            jugador.Morir();
        }
    }
}