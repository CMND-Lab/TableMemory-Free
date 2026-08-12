using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InstructionController : MonoBehaviour
{
    [SerializeField] UnityEvent callback;
    private TextMeshProUGUI instructionText;
    private CanvasController canvasController;

    [SerializeField] bool overrideButtons;
    [SerializeField] bool setBackButton;
    [SerializeField] bool setNextButton;
    [SerializeField] bool setCalibrateButton;
    [SerializeField] bool setContinueButton;

    private string defaultText;

    private void Awake()
    {
        InstructionText();
        CanvasController();

        defaultText = instructionText.text;
    }

    private TextMeshProUGUI InstructionText() {
        if (instructionText == null) {
            instructionText = GetComponent<TextMeshProUGUI>();
        }

        return instructionText;
    }

    private CanvasController CanvasController() {
        if (canvasController == null) {
            canvasController = GetComponentInParent<CanvasController>();
        }

        return canvasController;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        if (callback != null)
        {
            callback.Invoke();
        }

        if (overrideButtons)
        {
            CanvasController().nextButton.SetActive(setNextButton);
            CanvasController().backButton.SetActive(setBackButton);
            CanvasController().continueButton.SetActive(setContinueButton);
            CanvasController().calibrateButton.SetActive(setCalibrateButton);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
    public string GetText()
    {
        return InstructionText().text;
    }

    public void SetInstructionText(string newText)
    {
        InstructionText().text = newText;
    }
}

