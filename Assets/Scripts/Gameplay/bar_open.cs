using System.Collections;
using UnityEngine;
using AHeavyToll.Gameplay;

public class bar_open : MonoBehaviour, IInteractable
{
    public static bar_open Instance { get; private set; }

    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    private bool isOpen = false;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private Coroutine _currentCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _closedRotation = transform.rotation;
        _openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(openAngle, 0, 0));
    }

    // Player interaction (optional)
    public void Interact()
    {
        SetBarState(!isOpen);
    }

    // SYSTEM CONTROL (THIS is what VettingSystem / CarController uses)
    public void OpenBar()
    {
        SetBarState(true);
    }

    public void CloseBar()
    {
        SetBarState(false);
    }

    // CORE LOGIC (single source of truth)
    public void SetBarState(bool open)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(RotateBar(open));
    }

    private IEnumerator RotateBar(bool open)
    {
        Quaternion targetRotation = open ? _openRotation : _closedRotation;
        isOpen = open;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public string GetPromptText()
    {
        return isOpen ? "Close Bar" : "Open Bar";
    }
}