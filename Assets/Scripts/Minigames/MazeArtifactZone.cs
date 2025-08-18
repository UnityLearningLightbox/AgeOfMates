using UnityEngine;

public class MazeArtifactZone : MonoBehaviour
{
    [SerializeField] GameObject objetoConseguido;
    [SerializeField] Transform player;
    [SerializeField] Transform exitPosition;
    [SerializeField] GameObject mazeDoor;
    [SerializeField] GameObject startMaze;

    public InventoryUI inventoryUI;
    bool hasObject;

    private void Update()
    {
        hasObject = inventoryUI.objetos[0].tiene;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startMaze.SetActive(false);
            if (hasObject == true)
            {
                player.position = exitPosition.position;
                mazeDoor.SetActive(true);
            }
        }
    }
}
