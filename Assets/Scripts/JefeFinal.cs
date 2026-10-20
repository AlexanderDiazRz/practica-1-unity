using System.Collections;
using TMPro;
using UnityEngine;

public class JefeFinal : MonoBehaviour
{
    public int vidaMaxima = 10;
    public float velocidadPersecucion = 4f;

    public float limiteIzquierdo = 92f;
    public float limiteDerecho = 105f;

    public Transform jugador;
    public TextMeshProUGUI textoVida;
    public GestorNivel2 gestorNivel;

    private int vidaActual;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool derrotado;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        vidaActual = vidaMaxima;
        BuscarJugador();
        ActualizarTexto();
    }

    void BuscarJugador()
    {
        if (jugador != null) return;

        GameObject objetoJugador =
            GameObject.FindGameObjectWithTag("Player");

        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }
    }

    void FixedUpdate()
    {
        if (derrotado || rb == null) return;

        if (jugador == null)
        {
            BuscarJugador();
            return;
        }

        float objetivoX = Mathf.Clamp(
            jugador.position.x,
            limiteIzquierdo,
            limiteDerecho
        );

        float diferenciaX = objetivoX - transform.position.x;

        if (Mathf.Abs(diferenciaX) > 0.2f)
        {
            float direccion = Mathf.Sign(diferenciaX);

            rb.linearVelocity = new Vector2(
                direccion * velocidadPersecucion,
                rb.linearVelocity.y
            );

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direccion > 0;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }
    }

    public void RecibirDanio(int cantidad)
    {
        if (derrotado) return;

        vidaActual = Mathf.Max(
            0,
            vidaActual - cantidad
        );

        ActualizarTexto();
        StartCoroutine(ParpadeoGolpe());

        if (vidaActual <= 0)
        {
            Derrotar();
        }
    }

    IEnumerator ParpadeoGolpe()
    {
        if (spriteRenderer == null) yield break;

        Color colorOriginal = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.12f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorOriginal;
        }
    }

    void Derrotar()
    {
        derrotado = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (textoVida != null)
        {
            textoVida.text = "JEFE DERROTADO";
        }

        if (gestorNivel != null)
        {
            gestorNivel.CompletarJuego();
        }

        Destroy(gameObject, 0.25f);
    }

    void ActualizarTexto()
    {
        if (textoVida != null)
        {
            textoVida.text =
                "Jefe: " + vidaActual + "/" + vidaMaxima;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        MovimientoPersonaje jugadorTocado =
            collision.gameObject.GetComponent<MovimientoPersonaje>();

        if (jugadorTocado != null)
        {
            jugadorTocado.Morir();
        }
    }
}