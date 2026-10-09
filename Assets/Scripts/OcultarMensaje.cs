using UnityEngine;

public class OcultarMensaje : MonoBehaviour
{
    public float segundos = 5f;

    void Start()
    {
        Invoke("Ocultar", segundos);
    }

    void Ocultar()
    {
        gameObject.SetActive(false);
    }
}