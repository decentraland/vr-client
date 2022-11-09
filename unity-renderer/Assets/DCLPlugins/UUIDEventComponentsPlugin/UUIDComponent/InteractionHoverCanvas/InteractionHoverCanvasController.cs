using System;
using DCL.Models;
using DCL.Components;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DCL;

public class InteractionHoverCanvasController : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform backgroundTransform;
    public TextMeshProUGUI text;
    public GameObject[] icons;
    private Transform myTrans;

    bool isHovered = false;
    GameObject hoverIcon;
    Vector3 meshCenteredPos;

    [SerializeField]
    private float followSpeed = .8f;
    [SerializeField]
    private Vector3 offset = new Vector3(0f, .8f, 0f);

    const string ACTION_BUTTON_POINTER = "POINTER";
    const string ACTION_BUTTON_PRIMARY = "PRIMARY";
    const string ACTION_BUTTON_SECONDARY = "SECONDARY";

    private DataStore_Cursor dataStore;

    void Awake()
    {
        myTrans = transform;
        dataStore = DataStore.i.Get<DataStore_Cursor>();
        backgroundTransform.gameObject.SetActive(false);

        dataStore.hoverFeedbackButton.OnChange += OnChangeFeedbackButton;
        dataStore.hoverFeedbackText.OnChange += OnChangeFeedbackText;
        dataStore.hoverFeedbackEnabled.OnChange += OnChangeFeedbackEnabled;
        dataStore.hoverFeedbackHoverState.OnChange += OnChangeFeedbackHoverState;

        UpdateCanvas();
    }

    private void OnDestroy()
    {
        if (dataStore == null)
            return;

        dataStore.hoverFeedbackButton.OnChange -= OnChangeFeedbackButton;
        dataStore.hoverFeedbackText.OnChange -= OnChangeFeedbackText;
        dataStore.hoverFeedbackEnabled.OnChange -= OnChangeFeedbackEnabled;
        dataStore.hoverFeedbackHoverState.OnChange -= OnChangeFeedbackHoverState;
    }

    private void OnChangeFeedbackHoverState(bool current, bool previous)
    {
        SetHoverState(current);
    }

    private void OnChangeFeedbackEnabled(bool current, bool previous)
    {
        enabled = current;
        UpdateCanvas();
    }

    private void OnChangeFeedbackText(string current, string previous)
    {
        text.text = current;
        UpdateCanvas();
    }

    private void OnChangeFeedbackButton(string current, string previous)
    {
        ConfigureIcon(current);
        UpdateCanvas();
    }

    private void Update()
    {
        if (!isHovered)
            return;
        Vector3 offsetPos = CrossPlatformManager.GetPoint() + offset;
        myTrans.position = Vector3.Lerp(myTrans.position, offsetPos, followSpeed);
        myTrans.forward = CommonScriptableObjects.cameraForward.Get();
    }

    public void Setup(string button, string feedbackText)
    {
        text.text = feedbackText;
        ConfigureIcon(button);
        UpdateCanvas();
    }

    void ConfigureIcon(string button)
    {
        hoverIcon?.SetActive(false);

        switch (button)
        {
            case ACTION_BUTTON_POINTER:
                hoverIcon = icons[0];
                break;
            case ACTION_BUTTON_PRIMARY:
                hoverIcon = icons[1];
                break;
            case ACTION_BUTTON_SECONDARY:
                hoverIcon = icons[2];
                break;
            default: // ANY
                hoverIcon = icons[3];
                break;
        }

        hoverIcon.SetActive(true);
        backgroundTransform.gameObject.SetActive(true);
    }

    public void SetHoverState(bool hoverState)
    {
        if (!enabled || hoverState == isHovered)
            return;

        isHovered = hoverState;
        myTrans.position = CrossPlatformManager.GetPoint() + offset;
        UpdateCanvas();
    }

    public GameObject GetCurrentHoverIcon()
    {
        return hoverIcon;
    }

    void UpdateCanvas()
    {
        bool newValue = enabled && isHovered;

        if (canvas.enabled != newValue)
            canvas.enabled = newValue;
    }
}