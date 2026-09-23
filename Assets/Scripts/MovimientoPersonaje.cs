using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 4f;

    private Rigidbody2D rb;
    private bool enElSuelo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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