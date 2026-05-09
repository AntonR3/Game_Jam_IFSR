using UnityEngine;

public class fan : MonoBehaviour
{
    [Header("Einstellungen")]
    public float spinSpeed = 100f; // Geschwindigkeit in Grad pro Sekunde

    void Update()
    {
        // Wir rotieren um die Y-Achse (Standard für Ventilatoren)
        // Time.deltaTime macht die Bewegung unabhängig von der Framerate
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }
    void onDestroy()
    {
        transform.rotation = Quaternion.identity; // Setzt die Rotation zurück, wenn das Objekt zerstört wird
    }
}
