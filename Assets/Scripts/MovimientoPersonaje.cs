using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool enElSuelo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float movimientoX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(movimientoX * velocidadMovimiento, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && enElSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            enElSuelo = false;
        }

        // Voltear el sprite según la dirección
        if (movimientoX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movimientoX < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Enviar datos al Animator
        animator.SetFloat("Velocidad", Mathf.Abs(movimientoX));
        animator.SetBool("EnElSuelo", enElSuelo);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            if (contacto.normal.y > 0.5f)
            {
                enElSuelo = true;
                return;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        enElSuelo = false;
    }
}