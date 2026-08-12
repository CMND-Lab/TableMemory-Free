using TMPro;
using UnityEngine;


namespace TableMemory
{
    public class ResearcherCanvasController : MonoBehaviour
    {
        public TextMeshProUGUI experimentText;
        public TextMeshProUGUI blockText;
        public TextMeshProUGUI trialText;
        public TextMeshProUGUI absoluteTrialText;

        private void Awake()
        {
          DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
        }
        
        private void Update()
        {
        }


        public void setExperimentText(string text) {
          experimentText.text = text;
        }

        public void setBlockText(string block) {
          blockText.text = block;
        }

        public void setTrialText(string trial) {
          trialText.text = trial;
        }

        public void setBlockText(int block) {
          blockText.text = block + "";
        }

        public void setTrialText(int trial) {
          trialText.text = trial + "";
        }

        public void setBlockText(int block, int total) {
          blockText.text = block + "/" + total;
        }

        public void setTrialText(int trial, int total) {
          trialText.text = trial + "/" + total;
        }

        public void setAbsoluteTrial(int trial) {
          absoluteTrialText.text = "(" + trial + ")";
        }
    }
}

