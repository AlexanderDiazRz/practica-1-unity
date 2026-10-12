using UnityEngine;

public class ComplexEnemy : MonoBehaviour
{
    public float velocidad = 2f;
    public float velocidadPersecucion = 3.5f;
    public float rangoDeteccion = 5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform jugador;
    private int direccion = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Physics2D.queriesHitTriggers = false;

        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void FixedUpdate()
    {
        float velocidadActual = velocidad;

        bool persiguiendo = jugador != null
            && Mathf.Abs(jugador.position.x - transform.position.x) <= rangoDeteccion
            && Mathf.Abs(jugador.position.y - transform.position.y) <= 1.5f;

        if (persiguiendo)
        {
            direccion = jugador.position.x > transform.position.x ? 1 : -1;
            velocidadActual = velocidadPersecucion;

            if (HayPared() || !HaySuelo())
            {
                velocidadActual = 0f;
            }
        }
        else if (HayPared() || !HaySuelo())
        {
            direccion *= -1;
        }

        rb.linearVelocity = new Vector2(direccion * velocidadActual, rb.linearVelocity.y);
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
        MovimientoPersonaje jugadorScript = collision.gameObject.GetComponent<MovimientoPersonaje>();

        if (jugadorScript != null)
        {
            jugadorScript.Morir();
        }
    }
}