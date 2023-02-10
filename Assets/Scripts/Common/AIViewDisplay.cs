using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using UnityEngine;

public class AIViewDisplay : MonoBehaviour
{
    [SerializeField] Agent agent;
    [SerializeField] TMP_Text observationText;
    [SerializeField] TMP_Text actionText;
    [SerializeField] TMP_Text rewardsText;

    [SerializeField] string[] observationName;
    [SerializeField] string[] discreteActionName;
    [SerializeField] string[] continuousActionName;

    // Update is called once per frame
    void Update()
    {
        if (!agent) return;

        if (observationText)
        {
            observationText.text = "Observations:\n";
            int oIndex = 0;
            foreach (var observation in agent.GetObservations())
            {
                observationText.text +=
                    oIndex < observationName.Length
                    ? $"\t{observationName[oIndex]}: {observation}\n"
                    : $"\tObservation #{oIndex + 1}: {observation}\n";
                oIndex++;
            }
        }
        if (actionText)
        {
            actionText.text = "Observations:\n\t";
            int aIndex = 0;
            foreach (var dActions in agent.GetStoredActionBuffers().DiscreteActions)
            {
                actionText.text +=
                    aIndex < discreteActionName.Length
                    ? $"\t{discreteActionName[aIndex]}: {dActions}\n"
                    : $"\tDiscrete Action #{aIndex + 1}: {dActions}\n";
                aIndex++;
            }
            aIndex = 0;
            foreach (var cActions in agent.GetStoredActionBuffers().ContinuousActions)
            {
                actionText.text +=
                    aIndex < continuousActionName.Length
                    ? $"\t{continuousActionName[aIndex]}: {cActions}\n"
                    : $"\tContinuous Action #{aIndex + 1}: {cActions}\n";
                aIndex++;
            }
        }
        if (rewardsText)
        {
            rewardsText.text = $"Current Rewards: {agent.GetCumulativeReward()}";
        }
    }
}
