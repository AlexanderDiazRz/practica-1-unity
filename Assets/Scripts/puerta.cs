using UnityEngine;
using TMPro;

public class Puerta : MonoBehaviour
{
    public GameObject mensajeCompletado;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mensajeCompletado.SetActive(true);
        }
    }
}