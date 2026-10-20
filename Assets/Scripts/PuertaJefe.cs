using TMPro;
using UnityEngine;

public class PuertaJefe : MonoBehaviour
{
    public int frutasNecesarias = 5;
    public TextMeshProUGUI mensaje;
    public float duracionMensaje = 2f;

    private bool puertaAbierta = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        MovimientoPersonaje jugador =
            collision.gameObject.GetComponent<MovimientoPersonaje>();

        if (jugador == null || puertaAbierta) return;

        if (jugador.contador >= frutasNecesarias)
        {
            puertaAbierta = true;

            MostrarMensajeTemporal(
                "¡Poder completo! Elimina al jefe!"
            );

            AbrirPuerta();
        }
        else
        {
            MostrarMensajeTemporal(
                "Necesitas las 5 frutas"
            );
        }
    }

    void MostrarMensajeTemporal(string texto)
    {
        if (mensaje == null) return;

        mensaje.text = texto;
        mensaje.gameObject.SetActive(true);

        CancelInvoke(nameof(OcultarMensaje));
        Invoke(nameof(OcultarMensaje), duracionMensaje);
    }

    void AbrirPuerta()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            sprite.enabled = false;
        }
    }

    void OcultarMensaje()
    {
        if (mensaje != null)
        {
            mensaje.gameObject.SetActive(false);
        }
    }
}