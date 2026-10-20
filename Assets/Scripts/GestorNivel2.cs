using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorNivel2 : MonoBehaviour
{
    public GameObject panelVictoria;
    public MovimientoPersonaje jugador;
    public float segundosParaReiniciar = 4f;
    public string escenaInicial = "SampleScene";

    private bool terminado;

    public void CompletarJuego()
    {
        if (terminado) return;
        terminado = true;

        if (panelVictoria != null) panelVictoria.SetActive(true);

        if (jugador != null)
        {
            Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }
        }

        Invoke(nameof(ReiniciarDesdeCero), segundosParaReiniciar);
    }

    void ReiniciarDesdeCero()
    {
        SceneManager.LoadScene(escenaInicial);
    }
}
