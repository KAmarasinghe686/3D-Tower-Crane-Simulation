using TMPro;
using UnityEngine;
using System.Collections;

public class CraneCargoPickup : MonoBehaviour
{
    private bool isAttached = false;
    private Rigidbody rb;
    private Vector3 savedScale;
    private float attachCooldown = 0f;
    private Collider nearbyHook = null;
    private float groundY = 0.7f; 

    [Header("UI")]
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI dropResultText;

    private CraneController crane;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        savedScale = transform.localScale;
        crane = FindObjectOfType<CraneController>();
        if (hintText != null) hintText.text = "";
        if (dropResultText != null) dropResultText.text = "";
    }

    void Update()
    {
        if (attachCooldown > 0)
            attachCooldown -= Time.deltaTime;

        if (isAttached && crane != null && dropResultText != null)
        {
            float hookY = crane.GetHookHeight();
            if (hookY > -18f)
            {
                dropResultText.text = "WARNING: Too high to drop safely! Lower the hook!";
                dropResultText.color = Color.red;
            }
            else
            {
                dropResultText.text = "Safe height to drop - Press Space";
                dropResultText.color = Color.green;
            }
        }
    }

    //Shows "Press E" when hook touches box
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Hook" && !isAttached && attachCooldown <= 0)
        {
            nearbyHook = other;
            if (hintText != null)
            {
                hintText.text = "Press E to Pick Up";
                hintText.color = Color.darkGreen;
            }
        }
    }

    //Clears hint when hook leaves
    void OnTriggerExit(Collider other)
    {
        if (other.name == "Hook")
        {
            nearbyHook = null;
            if (hintText != null)
                hintText.text = "";
        }
    }

    // Picks up box, parents it to hook
    public void TryAttach()
    {
        if (nearbyHook == null) return;
        if (isAttached) return;
        if (attachCooldown > 0) return;

        isAttached = true;
        savedScale = transform.localScale;
        if (rb != null) rb.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;

        Vector3 worldScale = transform.lossyScale;
        transform.SetParent(nearbyHook.transform);
        GetComponent<Collider>().isTrigger = true;
        float objectHeight = GetComponent<Collider>().bounds.size.y;
        transform.localPosition = new Vector3(0, -(objectHeight / 2f), 0);
        transform.localRotation = Quaternion.identity;

        Vector3 ps = nearbyHook.transform.lossyScale;
        transform.localScale = new Vector3(
            ps.x != 0 ? worldScale.x / ps.x : 1f,
            ps.y != 0 ? worldScale.y / ps.y : 1f,
            ps.z != 0 ? worldScale.z / ps.z : 1f
        );

        nearbyHook = null;
        if (hintText != null)
        {
            hintText.text = "Picked Up! Press Space to Drop";
            hintText.color = Color.white;
        }
        GetComponent<Collider>().isTrigger = false;
    }

    //Drops box, places it on ground
    public void Detach()
    {
        if (!isAttached) return;
        isAttached = false;
        attachCooldown = 2f;
        transform.SetParent(null);
        transform.localScale = savedScale;

        if (rb != null)
        {
            rb.isKinematic = true;
            GetComponent<Collider>().isTrigger = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (hintText != null) hintText.text = "";
        if (dropResultText != null) dropResultText.text = "";

        StartCoroutine(SnapToGroundDelayed());
    }

    private IEnumerator SnapToGroundDelayed()
    {
        yield return null;
        yield return null; // ← extra frame for trigger to fire
        transform.position = new Vector3(
            transform.position.x,
            groundY,
            transform.position.z
        );
    }

    //Returns true/false if box is currently held
    public bool IsAttached()
    {
        return isAttached;
    }
}