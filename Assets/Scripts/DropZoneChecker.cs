using UnityEngine;
using TMPro;

public class DropZoneChecker : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dropResultText;

    //Shows success message when box lands in zone
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter hit by: " + other.name + " tag: " + other.tag);
        if (other.CompareTag("Cargo"))
        {
            CraneCargoPickup cargo = other.GetComponent<CraneCargoPickup>();
            Debug.Log("Cargo found, IsAttached: " + cargo.IsAttached());
            if (cargo != null && !cargo.IsAttached())
            {
                if (dropResultText != null)
                    dropResultText.text = "Cargo delivered successfully!";
                Renderer rend = GetComponent<Renderer>();
                if (rend != null)
                    rend.material.color = Color.green;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("TriggerStay hit by: " + other.name + " tag: " + other.tag);
    }

    //Shows miss message when box leaves zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cargo"))
        {
            CraneCargoPickup cargo = other.GetComponent<CraneCargoPickup>();
            if (cargo != null && !cargo.IsAttached())
            {
                if (dropResultText != null)
                    dropResultText.text = "Cargo missed the zone!";
            }
        }
    }

}