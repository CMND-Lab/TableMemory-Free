using System.Collections.Generic;
using UnityEngine;
using TableMemory;
using UXF;

public class DataManager : MonoBehaviour
{
    [SerializeField] GameObject controller;

    public static void RecordContext(Trial trial, ContextManager contextManager)
    {
        trial.result["context"] = contextManager.CurrentContext();
    }
    public static void RecordSessionSetting(string settingName, object settingValue)
    {
        Session.instance.participantDetails[settingName] = settingValue;
    }

    public static void RecordStudyTrial(Trial trial, StudyTrialSetting trialSettings, List<ObjectController> spawnObjectControllers)
    {
        RecordStudyStimuli(trial, spawnObjectControllers);
        trial.result["num_study_stimuli"] = spawnObjectControllers.Count;
        trial.result["num_study_highlights"] = trialSettings.GetNumHighlights();
    }
    public static void RecordStudyStimuli(Trial trial, List<ObjectController> stimuli) 
    { 
        for (int i = 0; i < stimuli.Count; i++)
        {
            RecordLocationForTrial(trial, "study_" + i.ToString(), stimuli[i]);
        }
    }
    public static void RecordHighlights(Trial trial, List<string> highlightedObjects)
    {
        trial.result["study_highlighted_objects"] = string.Join(", ", highlightedObjects);
    }

    public static void RecordLocationForTrial(Trial trial, string identifier, ObjectController stimulus)
    {
        trial.result[stimulus.GetName() + "_location"] = stimulus.GetPosition().ToString();
        trial.result[stimulus.GetName() + "_size"] = stimulus.GetSize().ToString();

        trial.result["object_name_" + identifier] = stimulus.GetName();
        trial.result["object_location_origin_" + identifier] = stimulus.GetPosition().ToString();
        trial.result["object_location_rotation_" + identifier] = stimulus.GetRotation().ToString();
        trial.result["object_location_size_" + identifier] = stimulus.GetSize().ToString();
    }

    public static void TrialTimedOut(Trial trial)
    {
        trial.result["valid_response"] = 0;
        trial.result["timed_out"] = 1;
        trial.result["experimenter_ended"] = 0;
    }

    public static void TrialForceStopped(Trial trial)
    {
        trial.result["valid_response"] = 0;
        trial.result["timed_out"] = 0;
        trial.result["experimenter_ended"] = 1;
    }

    public static void TrialValidResponse(Trial trial) 
    {
        // log output
        trial.result["valid_response"] = 1;
    }

    public void ExitStartOrb()
    {
        Session session = Session.instance;

        session.CurrentTrial.result["reach_init_time"] = Time.time;

        Vector3 p = controller.transform.position;
        session.CurrentTrial.result["reach_init_pos_x"] = p.x;
        session.CurrentTrial.result["reach_init_pos_y"] = p.y;
        session.CurrentTrial.result["reach_init_pos_z"] = p.z;
    }

    public void FinishTrialPosition(Trial trial)
    {
        Session session = Session.instance;

        Vector3 p = controller.transform.position;
        trial.result["fin_pos_x"] = p.x;
        trial.result["fin_pos_y"] = p.y;
        trial.result["fin_pos_z"] = p.z;
    }

    public static void RecordTime(string name)
    {
        Session session = Session.instance;

        session.CurrentTrial.result[name] = Time.time;
    }

    public static void RecordTrialResultList<T>(Trial trial, string resultName, List<T> resultList)
    {
        trial.result[resultName] = string.Join(", ", resultList);
        trial.result[resultName] = string.Join(", ", resultList);
    }

    public static void RecordHoldTimes(Trial trial, List<float> pickupTimes, List<float> dropTimes)
    {
        List<string> timePairs = new List<string>();

        for (int i = 0; i < pickupTimes.Count; i++) { 
            // Pickup time
            string time = pickupTimes[i].ToString();
            // Drop time for this pickup
            if (i < dropTimes.Count)
            {
                time += "," + dropTimes[i].ToString();
            }
            timePairs.Add(time);
        }

        trial.result["hold_times"] = string.Join(";", timePairs.ToArray());
    }
}