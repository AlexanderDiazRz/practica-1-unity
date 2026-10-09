using UnityEngine;

public class Puerta : MonoBehaviour
{
    public GameObject mensajeCompletado;
    public int frutasNecesarias = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje jugador = other.GetComponent<MovimientoPersonaje>();

            if (jugador != null && jugador.contador >= frutasNecesarias)
            {
                mensajeCompletado.SetActive(true);
            }
        }
    }
}