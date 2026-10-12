using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MovimientoPersonaje : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 4f;

    public TextMeshProUGUI contadorTexto;
    public GameObject mensajePerdiste;
    public float segundosAntesDeReiniciar = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool enElSuelo = false;
    private bool muerto = false;
    public int contador = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ActualizarContador();
    }

    void Update()
    {
        if (muerto) return;

        float movimientoX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(movimientoX * velocidadMovimiento, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && enElSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            enElSuelo = false;
        }

        if (movimientoX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movimientoX < 0)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetFloat("Velocidad", Mathf.Abs(movimientoX));
        animator.SetBool("EnElSuelo", enElSuelo);
    }

    public void Morir()
    {
        if (muerto) return;

        muerto = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (mensajePerdiste != null)
        {
            mensajePerdiste.SetActive(true);
        }

        Invoke("ReiniciarEscena", segundosAntesDeReiniciar);
    }

    void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (muerto) return;

        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            contador++;
            ActualizarContador();
        }
    }

    void ActualizarContador()
    {
        contadorTexto.text = "Frutas: " + contador;
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