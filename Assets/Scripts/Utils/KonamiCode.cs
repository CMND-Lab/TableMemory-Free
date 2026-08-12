using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Valve.VR;

public class KonamiCode : MonoBehaviour
{
    public SteamVR_ActionSet actionSet;

    public SteamVR_Action_Boolean upKeyAction = SteamVR_Input.GetBooleanAction("Konami_Up");
    public SteamVR_Action_Boolean downKeyAction = SteamVR_Input.GetBooleanAction("Konami_Down");
    public SteamVR_Action_Boolean leftKeyAction = SteamVR_Input.GetBooleanAction("Konami_Left");
    public SteamVR_Action_Boolean rightKeyAction = SteamVR_Input.GetBooleanAction("Konami_Right");
    public SteamVR_Action_Boolean abKeyAction = SteamVR_Input.GetBooleanAction("Konami_AB");
    public SteamVR_Action_Boolean plusKeyAction = SteamVR_Input.GetBooleanAction("Konami_Plus");

    // Define the Konami code sequence
    private KonamiKeys[] konamiCode = {
        KonamiKeys.Up,
        KonamiKeys.Up,
        KonamiKeys.Down,
        KonamiKeys.Down,
        KonamiKeys.Left,
        KonamiKeys.Right,
        KonamiKeys.Left,
        KonamiKeys.Right,
        KonamiKeys.AB,
        KonamiKeys.AB,
        KonamiKeys.Plus
    };

    [SerializeField] int currentIndex = 0;

    private void Awake()
    {
        actionSet.Activate();
    }

    private void OnEnable()
    {
        upKeyAction.onStateDown += OnUpKey;
        downKeyAction.onStateDown += OnDownKey;
        rightKeyAction.onStateDown += OnRightKey;
        leftKeyAction.onStateDown += OnLeftKey;

        abKeyAction.onStateDown += OnABKey;
        plusKeyAction.onStateDown += OnPlusKey;
    }

    private void OnDisable()
    {
        upKeyAction.onStateDown -= OnUpKey;
        downKeyAction.onStateDown -= OnDownKey;
        rightKeyAction.onStateDown -= OnRightKey;
        leftKeyAction.onStateDown -= OnLeftKey;

        abKeyAction.onStateDown -= OnABKey;
        plusKeyAction.onStateDown -= OnPlusKey;
    }

    private void OnUpKey(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) { HitCode(KonamiKeys.Up); }
    private void OnDownKey(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) { HitCode(KonamiKeys.Down); }
    private void OnLeftKey(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) { HitCode(KonamiKeys.Left); }
    private void OnRightKey(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) { HitCode(KonamiKeys.Right); }
    private void OnABKey(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) { HitCode(KonamiKeys.AB); }
    private void OnPlusKey(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) { HitCode(KonamiKeys.Plus); }


    [SerializeField] UnityEvent konamiEvent;

    void HitCode(KonamiKeys currentKey)
    {
        // Check if the current index of the Konami code matches the corresponding button press
        if (currentIndex < konamiCode.Length && currentKey == konamiCode[currentIndex])
        {
            currentIndex++;
        }
        else
        {
            // Reset the index if the wrong button is pressed
            currentIndex = 0;
        }

        if (currentIndex == konamiCode.Length)
        {
            // Konami code successfully entered
            Debug.Log("Konami code activated!");
            // Add your code to execute when Konami code is entered successfully
            // For example, you can trigger an event or perform some action
            // Reset the index for next input sequence
            currentIndex = 0;

            konamiEvent.Invoke();
        }
    }

    private enum KonamiKeys {
        Up,
        Down,
        Left,
        Right,
        AB,
        Plus
    }
}