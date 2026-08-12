using System.Collections;
using UnityEngine;

namespace TableMemory
{
    public class TimeManager : MonoBehaviour
    {
        public TaskController taskController;

        private Coroutine countdown;

        public float studyTimerCount = 2.5f;

        private void Start()
        {
            DataManager.RecordSessionSetting("trial_study_time", studyTimerCount);
        }

        public void BeginStudyCountdown()
        {
            countdown = StartCoroutine(StudyCountdown());
        }

        public void StopCountdown()
        {
            if (countdown != null) {
                StopCoroutine(countdown);
            }
            countdown = null;
        }

        IEnumerator StudyCountdown()
        {
            yield return new WaitForSeconds(studyTimerCount);

            taskController.TimerEnd();
        }
    }
}


