using TMPro;
using UnityEngine;

public class PoderNivel2 : MonoBehaviour
{
    public int frutasNecesarias = 5;
    public KeyCode teclaDisparo = KeyCode.F;
    public float velocidadProyectil = 12f;
    public float tiempoEntreDisparos = 0.25f;
    public Sprite spriteProyectil;
    public TextMeshProUGUI textoPoder;

    private MovimientoPersonaje movimiento;
    private SpriteRenderer spriteRenderer;
    private float proximoDisparo;

    public bool PoderActivo => movimiento != null && movimiento.contador >= frutasNecesarias;

    void Awake()
    {
        movimiento = GetComponent<MovimientoPersonaje>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        ActualizarTexto();

        if (PoderActivo && Input.GetKeyDown(teclaDisparo) && Time.time >= proximoDisparo)
        {
            Disparar();
            proximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    void ActualizarTexto()
    {
        if (textoPoder == null || movimiento == null) return;

        int frutas = Mathf.Min(movimiento.contador, frutasNecesarias);
        textoPoder.text = PoderActivo
            ? "Poder: LISTO  [F]"
            : "Poder: " + frutas + "/" + frutasNecesarias;
        textoPoder.color = PoderActivo ? Color.yellow : Color.white;
    }

    void Disparar()
    {
        bool miraDerecha = spriteRenderer == null || !spriteRenderer.flipX;
        Vector2 direccion = miraDerecha ? Vector2.right : Vector2.left;

        GameObject proyectil = new GameObject("BolaEnergia");
        proyectil.transform.position = transform.position + (Vector3)(direccion * 0.75f);
        proyectil.transform.localScale = Vector3.one * 2.2f;

        SpriteRenderer visual = proyectil.AddComponent<SpriteRenderer>();
        visual.sprite = spriteProyectil;
        visual.color = new Color(0.2f, 0.9f, 1f, 1f);
        visual.sortingOrder = 20;

        Rigidbody2D rb = proyectil.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D collider = proyectil.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.22f;

        ProyectilEnergia script = proyectil.AddComponent<ProyectilEnergia>();
        script.Inicializar(direccion, velocidadProyectil);
    }
}
