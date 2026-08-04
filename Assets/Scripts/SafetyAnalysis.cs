using UnityEngine;
using TMPro;

public class SafetyAnalysis : MonoBehaviour
{
    [Header("Safety Limits")]
    public float innerDangerRadius = 0f;    // Too close to tower
    public float innerWarningRadius = 0.1f;   // Approaching too close
    public float outerWarningRadius = 0.6f;   // Approaching too far
    public float outerDangerRadius = 0.75f;    // Too far out

    [Header("UI References")]
    public TextMeshProUGUI statusText;

    private CraneController crane;

    void Start()
    {
        crane = GetComponent<CraneController>();
        UpdateUI("SAFE: Operation Normal", Color.green);
    }

    void Update()
    {
        if (crane == null) return;
     
        AnalyseSafety(crane.GetTrolleyDistance());
        
    }

    //Checks trolley distance every frame, sets Safe/Warning/Danger
    void AnalyseSafety(float distance)
    {
        if (distance <= innerDangerRadius)
        {
            UpdateUI("DANGER: Too close to tower!", Color.red);
        }
        else if (distance <= innerWarningRadius)
        {
            UpdateUI("WARNING: Approaching tower!",
                new Color(1f, 0.6f, 0f));
        }
        else if (distance >= outerDangerRadius)
        {
            UpdateUI("DANGER: Load out of safe range!", Color.red);
        }
        else if (distance >= outerWarningRadius)
        {
            UpdateUI("WARNING: Approaching limit!",
                new Color(1f, 0.6f, 0f));
        }
        else
        {
            UpdateUI("SAFE: Operation Normal", Color.green);
        }
    }

    // Changes text and panel color based on safety state
    void UpdateUI(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color; 
        }
    }


}