using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using UnityEngine;

public class AIViewDisplay : MonoBehaviour
{
    [SerializeField] Agent agent;
    [SerializeField] TMP_Text agentNameText;
    [SerializeField] TMP_Text observationText;
    [SerializeField] TMP_Text actionText;
    [SerializeField] TMP_Text rewardsText;

    [SerializeField] string[] observationName;
    [SerializeField] string[] discreteActionName;
    [SerializeField] string[] continuousActionName;


    private string agentName = "Agent";
    private void Awake()
    {
        if (agent && agent.TryGetComponent(out Unity.MLAgents.Policies.BehaviorParameters behaveParams))
        {
            agentName = behaveParams.BehaviorName;
        }
        if (agentNameText)
        {
            agentNameText.text = $"AI Viewer: {agentName}";
        }
    }

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
            actionText.text = "Actions:\n";
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
