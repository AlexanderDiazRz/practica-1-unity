using UnityEngine;

public class ZonaPeligrosa : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        MovimientoPersonaje jugador = other.GetComponent<MovimientoPersonaje>();
        if (jugador != null) jugador.Morir();
    }
}
