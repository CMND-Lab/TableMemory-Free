using UnityEngine;
using UXF;

public class CustomInstructions : MonoBehaviour
{
    public InstructionController experimentInstructionTrials;
    public void SetInstructions()
    {
        if (experimentInstructionTrials != null)
        {
            string instructionText = experimentInstructionTrials.GetText();

            Debug.Log(instructionText);
            instructionText = instructionText.Replace("%", Session.instance.settings.GetInt("n_experimental_blocks").ToString());
            Debug.Log(instructionText);

            experimentInstructionTrials.SetInstructionText(instructionText);
        }
    }
}