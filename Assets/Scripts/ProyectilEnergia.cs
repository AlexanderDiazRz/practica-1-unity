using UnityEngine;

public class ProyectilEnergia : MonoBehaviour
{
    public float tiempoDeVida = 3f;

    public void Inicializar(Vector2 direccion, float velocidad)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direccion.normalized * velocidad;
        Destroy(gameObject, tiempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        JefeFinal jefe = other.GetComponentInParent<JefeFinal>();
        if (jefe != null)
        {
            jefe.RecibirDanio(1);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger) Destroy(gameObject);
    }
}
