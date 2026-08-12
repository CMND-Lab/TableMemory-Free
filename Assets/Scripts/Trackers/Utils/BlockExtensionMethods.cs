// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using System.Linq;
// using UXF;
//
// /*
//  *  NOTE THAT THIS SCRIPT HAS BEEN UPDATED BASED ON UXF VERSION 2
//  */
// namespace UXFExtensionMethods
// {
//     public static class BlockExtensionMethods
//     {
//          /// <summary>
//         /// This method does not invoke OnTrialBegin Event.
//         /// Begins the trial, updating the current trial and block number, setting the status to in progress, starting the timer for the trial, and beginning recording positions of every object with an attached tracker
//         /// </summary>
//         /// <param name="trial"></param>
//         public static void BeginExtension(this Trial trial)
//         {
//             // UXFV2 if (trial.session.InTrial) trial.session.CurrentTrial.End();
//             
//             trial.session.currentTrialNum = trial.number;
//             trial.session.currentBlockNum = trial.block.number;
//
//             trial.status = TrialStatus.InProgress;
//             // startTime = Time.time
//             // startTime is not a public variable
//             // UXFV1 trial.result = new ResultsDictionary(trial.session.Headers, trial.session.adHocHeaderAdd);
//             // UXFV2
//             trial.result = new ResultsDictionary(trial.session.Headers, true);
//             
//             // UXFV1 trial.result["directory"] = Extensions.CombinePaths(trial.session.experimentName, trial.session.ppid, trial.session.FolderName).Replace('\\', '/');
//             trial.result["experiment"] = trial.session.experimentName;
//             trial.result["ppid"] = trial.session.ppid;
//             trial.result["session_num"] = trial.session.number;
//             trial.result["trial_num"] = trial.number;
//             trial.result["block_num"] = trial.block.number;
//             trial.result["trial_num_in_block"] = trial.numberInBlock;
//             /*
//             TODO 
//             the value of end_time must be consistent 
//             with the value used for missing values of the tracker
//             */
//             trial.result["start_time"] = 0.0f;
//
//             foreach (Tracker tracker in trial.session.trackedObjects)
//             {
//                 // UXFV1  tracker.StartRecording();
//                 try
//                 {
//                     tracker.StartRecording();
//                 }
//                 catch (NullReferenceException)
//                 {
//                     Utilities.UXFDebugLogWarning("An item in the Tracked Objects field of the UXF session if empty (null)!");
//                 }
//             }
//             // Debug.Log("Next trial started without invoking OnTrialBegin Event");
//         }
//         
//         /// <summary>
//         /// This method does not invoke OnTrialEnd Event
//         /// Ends the Trial, queues up saving results to output file, stops and saves tracked object data.
//         /// </summary>
//         /// <param name="trial"></param>
//         public static void EndExtension(this Trial trial)
//         {
//             trial.status = TrialStatus.Done;
//             // endTime = Time.time
//             // endTime is not a public variable
//             /*
//             TODO 
//             the value of end_time must be consistent 
//             with the value used for missing values of the tracker
//             */
//             trial.result["end_time"] = 0.0f;
//             
//             // check no duplicate trackers
//             List<string> duplicateTrackers = trial.session.trackedObjects.Where(tracker => tracker != null)
//                 .GroupBy(tracker => tracker.dataName)
//                 .Where(g => g.Count() > 1)
//                 .Select(y => y.Key)
//                 .ToList(); 
//
//             if (duplicateTrackers.Any()) throw new InvalidOperationException(string.Format("Two or more trackers in the Tracked Objects field in the Session Inspector have the following object name and descriptor pair, please change the object name fields on the trackers to make them unique: {0}", string.Join(",", duplicateTrackers)));
//
//
//             foreach (Tracker tracker in trial.session.trackedObjects)
//             {   
//                 // UXFV1
//                 // tracker.StopRecording();
//                 // string dataName = trial.session.SaveTrackerData(tracker);
//                 // trial.result[tracker.filenameHeader] = dataName;
//                 try
//                 {
//                     tracker.StopRecording();
//                     trial.SaveDataTable(tracker.data, tracker.dataName, dataType: UXFDataType.Trackers);
//                 }
//                 catch (NullReferenceException)
//                 {
//                     Utilities.UXFDebugLogWarning("An item in the Tracked Objects field of the UXF session if empty (null)!");
//                 }
//                 
//             }
//
//             foreach (string s in trial.session.settingsToLog)
//             {
//                 trial.result[s] = trial.settings.GetObject(s);
//             }
//             
//             // Debug.Log("Next trial finished without invoking OnTrialEnd Event");
//         }
//     }
// }
//
