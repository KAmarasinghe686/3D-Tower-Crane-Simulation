using UnityEngine;
using UnityEngine.InputSystem;

public class CraneController : MonoBehaviour
{
    [Header("Speeds")]
    public float rotateSpeed = 30f;
    public float trolleySpeed = 0.5f;
    public float hoistSpeed = 3f;

    [Header("Parts")]
    public Transform trolley;
    public Transform hook;
    public LineRenderer cable;

    [Header("Trolley Limits")]
    public float trolleyMinX = -1.5f;
    public float trolleyMaxX = 7.5f;

    [Header("Swing Settings")]
    public float swingDamping = 0.95f;
    public float swingForceMultiplier = 0.01f;
    public float maxSwingAngle = 2f;

    private float currentTrolleyX;
    private float currentHookY;
    private float previousTrolleyX;

    private float swingAngle = 0f;
    private float swingVelocity = 0f;

    private SafetyAnalysis safetyAnalysis;

    void Start()
    {
        if (trolley != null) currentTrolleyX = trolley.localPosition.x;
        if (hook != null) currentHookY = hook.localPosition.y;
        previousTrolleyX = currentTrolleyX;
        safetyAnalysis = GetComponent<SafetyAnalysis>();
    }

    void Update()
    {
        HandleRotation();
        HandleTrolley();
        HandleHoist();
        HandleSwing();
        UpdateCable();
        HandleDrop();
    }

    //Left/Right arrows rotate the whole crane
    void HandleRotation()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
            transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
        if (Keyboard.current.rightArrowKey.isPressed)
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    //W/S moves the trolley along the jib
    void HandleTrolley()
    {
        previousTrolleyX = currentTrolleyX;

        if (Keyboard.current.wKey.isPressed)
            currentTrolleyX += trolleySpeed * Time.deltaTime;
        if (Keyboard.current.sKey.isPressed)
            currentTrolleyX -= trolleySpeed * Time.deltaTime;

        currentTrolleyX = Mathf.Clamp(currentTrolleyX, trolleyMinX, trolleyMaxX);

        trolley.localPosition = new Vector3(
            currentTrolleyX,
            trolley.localPosition.y,
            trolley.localPosition.z
        );

        float trolleyDelta = currentTrolleyX - previousTrolleyX;
        swingVelocity += trolleyDelta * swingForceMultiplier;
    }

    //Up/Down arrows raise and lower the hook
    void HandleHoist()
    {
        if (Keyboard.current.upArrowKey.isPressed)
            currentHookY += hoistSpeed * Time.deltaTime;
        if (Keyboard.current.downArrowKey.isPressed)
            currentHookY -= hoistSpeed * Time.deltaTime;

        currentHookY = Mathf.Clamp(currentHookY, -22f, -0.2f);
        hook.localPosition = new Vector3(0, currentHookY, 0);
    }

    //Calculates pendulum physics when trolley moves
    void HandleSwing()
    {
        float cableLength = Mathf.Abs(currentHookY);
        if (cableLength < 0.1f) cableLength = 0.1f;

        float gravity = 9.8f;
        float pendulumAcceleration = -(gravity / cableLength)
                                     * Mathf.Sin(swingAngle * Mathf.Deg2Rad);

        swingVelocity += pendulumAcceleration * Time.deltaTime;
        swingVelocity *= swingDamping;
        swingAngle += swingVelocity;

        swingAngle = Mathf.Clamp(swingAngle, -maxSwingAngle, maxSwingAngle);

        float swingOffset = Mathf.Sin(swingAngle * Mathf.Deg2Rad) * cableLength;
        hook.localPosition = new Vector3(swingOffset, currentHookY, 0);
    }

    //Draws the cable line between trolley and hook
    void UpdateCable()
    {
        if (cable != null)
        {
            cable.SetPosition(0, trolley.position);
            cable.SetPosition(1, hook.position);
        }
    }

    //E key picks up, Space drops cargo
    void HandleDrop()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameObject[] cargoObjects = GameObject.FindGameObjectsWithTag("Cargo");
            GameObject nearest = null;
            float minDist = float.MaxValue;

            foreach (GameObject cargoObj in cargoObjects)
            {
                float distance = Vector3.Distance(hook.position, cargoObj.transform.position);
                if (distance < minDist)
                {
                    minDist = distance;
                    nearest = cargoObj;
                }
            }

            if (nearest != null)
            {
                CraneCargoPickup cargo = nearest.GetComponent<CraneCargoPickup>();
                if (cargo != null) cargo.TryAttach();
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameObject[] cargoObjects = GameObject.FindGameObjectsWithTag("Cargo");
            foreach (GameObject cargoObj in cargoObjects)
            {
                CraneCargoPickup cargo = cargoObj.GetComponent<CraneCargoPickup>();
                if (cargo != null && cargo.IsAttached())
                {
                    cargo.Detach();
                    currentHookY += 1f;
                    currentHookY = Mathf.Clamp(currentHookY, -22f, -0.2f);
                    hook.localPosition = new Vector3(0, currentHookY, 0);
                }
            }
        }
    }

    public float GetTrolleyDistance()
    {
        return currentTrolleyX;
    }

    public float GetHookHeight()
    {
        return currentHookY;
    }
}