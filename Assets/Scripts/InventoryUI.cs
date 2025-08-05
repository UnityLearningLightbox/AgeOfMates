using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject slotPrefab;         // Prefab del slot con un hijo Image
    public Transform slotParent;          // Panel con Grid Layout Group
    private int slotCount = 4;            // SOLO 4 SLOTS

    [Header("Ícono de prueba")]
    public Sprite testSprite;             // Sprite temporal para probar

    private List<Image> slotIcons = new List<Image>();

    void Start()
    {
        CrearInventario();
    }

    void Update()
    {
        // Pulsar E para añadir un objeto de prueba
        if (Input.GetKeyDown(KeyCode.E))
        {
            AñadirItem(testSprite);
        }
    }

    void CrearInventario()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            Image iconImage = slot.transform.GetChild(0).GetComponent<Image>();
            iconImage.enabled = false;
            slotIcons.Add(iconImage);
        }
    }

    public void AñadirItem(Sprite itemIcon)
    {
        foreach (var icon in slotIcons)
        {
            if (!icon.enabled)
            {
                icon.sprite = itemIcon;
                icon.enabled = true;
                return;
            }
        }

        Debug.Log("Inventario lleno.");
    }
}

