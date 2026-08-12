using UnityEngine;
using TableMemory;
using TMPro;

public class ControllerHints : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hintText;

    private string[] studyTaskHints =
    {
        "Hold the controller orb inside the central orb until the study objects appear on the table",
        "Use the controller orb to touch the highlighted object",
        "You'll have to interact with a few objects...",
    };
    private string[] testTaskHints =
    {
        "Hold the controller orb inside the central orb until a test object appears in front of you",
        "OLD = was on the table\n\nNEW = was not on the table",
        "Move the controller to the object, and hold the trigger to grab it",
        "Place the object on the table, where you remember seeing it",
        "Use the laser pointer to click the <b>CONFIRM</b> button on the back of the table"
    };
    private string[] locationSwitchHints =
    {
        "Use the laser pointer to open the door next to you",
        "Use the laser pointer to click on the other table",
        "Always make sure you close the door!",
        "Continue with the experiment"
    };
    private string[] timeDelayHints =
    {
        "There will be a short delay between seeing the objects and the memory test..."
    };

    private string[] activeHints = {};
    private int currentHint;

    private void Awake()
    {
        
    }

    public void ShowTaskHints(TrialType trialType)
    {
        ShowNewHints(trialType == TrialType.Study ? studyTaskHints : testTaskHints);
    }

    public void ShowSwitchHints()
    {
        ShowNewHints(locationSwitchHints);
    }

    public void NextHint()
    {
        if (currentHint < activeHints.Length - 1)
        {
            currentHint++;
            hintText.text = activeHints[currentHint];
        }
    }

    public void SetHint(string newText)
    {
        hintText.text = newText;
    }

    public void ShowTimeDelayHints()
    {
        ShowNewHints(timeDelayHints);
    }

    private void ShowNewHints(string[] newHints)
    {
        gameObject.SetActive(true);
        activeHints = newHints;

        currentHint = 0;
        hintText.text = activeHints[currentHint];
    }
}