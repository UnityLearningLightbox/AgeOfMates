using UnityEngine;

public class TestDrop : MonoBehaviour
{
    public GameObject prefabObjeto; // El prefab que quieres spawnear
    public Transform puntoDrop;     // Empty delante del jugador

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (prefabObjeto != null && puntoDrop != null)
            {
                Vector3 dropPos = puntoDrop.position + puntoDrop.forward * 1.5f;
                dropPos.y += 0.5f; // un poco por encima del suelo

                GameObject nuevoObjeto = Instantiate(prefabObjeto, dropPos, Quaternion.identity);

                // Añadir rigidbody para que caiga
                if (nuevoObjeto.GetComponent<Rigidbody>() == null)
                {
                    nuevoObjeto.AddComponent<Rigidbody>();
                }

                Debug.Log("Objeto dropeado en: " + dropPos);
            }
            else
            {
                Debug.LogWarning("Falta asignar prefabObjeto o puntoDrop en el Inspector.");
            }
        }
    }
}
