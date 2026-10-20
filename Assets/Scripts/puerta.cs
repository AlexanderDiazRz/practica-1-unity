using UnityEngine;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{
    public GameObject mensajeCompletado;
    public int frutasNecesarias = 10;
    public string escenaSiguiente = "Nivel2";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje jugador =
                other.GetComponent<MovimientoPersonaje>();

            if (jugador != null && jugador.contador >= frutasNecesarias)
            {
                mensajeCompletado.SetActive(true);
                Invoke(nameof(CargarNivel), 1.5f);
            }
        }
    }

    void CargarNivel()
    {
        SceneManager.LoadScene(escenaSiguiente);
    }
}