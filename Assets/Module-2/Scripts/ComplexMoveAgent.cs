using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Events;

public class ComplexMoveAgent : Agent
{
    [SerializeField] PressableButton button;
    [SerializeField] GameObject goal;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float buttonReward = +0.25f;
    [SerializeField] float goalReward = +1;
    [SerializeField] float wallReward = -1;
    [SerializeField] KeyCode interactionKey = KeyCode.Space;

    [SerializeField] Vector2 randomizeCornerRange = new Vector2(5, 8);

    public UnityEvent OnEpisodeSuccess = new();
    public UnityEvent OnEpisodeFailure = new();

    public override void OnEpisodeBegin()
    {
        goal.SetActive(false);
        // Get random quadrants
        Vector2 quadrant = RandomQuadrant();
        Vector2 oppositeQuadrant = RandomQuadrant();
        // Replace "opposite" quadrant if its the same as quadrant with negative version
        oppositeQuadrant = oppositeQuadrant == quadrant ? -1 * quadrant : oppositeQuadrant;
        // Randomize Button (parent) first
        Vector3 buttonRandomPos = RandomizeVector3();
        button.transform.parent.localPosition = new Vector3(buttonRandomPos.x * quadrant.x, 1, buttonRandomPos.z * quadrant.y);
        // Randomize Goal away from button
        Vector3 goalRandomPos = RandomizeVector3();
        goal.transform.localPosition = new Vector3(goalRandomPos.x * oppositeQuadrant.x, 1, goalRandomPos.z * oppositeQuadrant.y);
        // Player always starts in the center (x and z = 0, y = 1)
        transform.localPosition = Vector3.up;

        // Reset Button
        button.ResetButton();
    }

    private Vector2 RandomQuadrant()
    {
        return new(Random.value < 0.5 ? 1 : -1, Random.value < 0.5 ? 1 : -1);
    }

    private Vector3 RandomizeVector3()
    {
        float randXValue = Random.Range(randomizeCornerRange.x, randomizeCornerRange.y);
        float randZValue = Random.Range(randomizeCornerRange.x, randomizeCornerRange.y);
        return new(randXValue, 1, randZValue);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Get goal position only when activated by button,
        // otherwise just (0,0,0)
        var goalPos = goal.activeSelf ? goal.transform.localPosition : Vector3.zero;
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(button.transform.localPosition);
        sensor.AddObservation(goalPos);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Apply recieved actions to the agent
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        // 0 = not pressing 
        // 1 = pressing
        if (actions.DiscreteActions[0] == 1)
        {
            // Try to press the button
            var successfulPress = button.PressButton();
            // Activate the goal on button success
            if (successfulPress && !goal.activeSelf)
            {
                goal.SetActive(true);
                AddReward(buttonReward);
            }
        }
        transform.localPosition += new Vector3(moveX, 0, moveZ).normalized * Time.deltaTime * moveSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        // NOTE: Using tag instead of script to allow more portablitiy,
        //       can convert to checking for script if you would like.

        // Check for trigger enter between agent and wall or agent and goal.
        if (other.CompareTag("Finish"))
        {
            AddReward(goalReward);
            OnEpisodeSuccess.Invoke();
            EndEpisode();
        }
        else if (other.CompareTag("Respawn"))
        {
            AddReward(wallReward);
            OnEpisodeFailure.Invoke();
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allow player to control when in "Heuristic Mode".
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal") * moveSpeed;
        continuousAction[1] = Input.GetAxisRaw("Vertical") * moveSpeed;
        ActionSegment<int> discreteAction = actionsOut.DiscreteActions;
        // Convert key press to a number for discete action
        discreteAction[0] = Input.GetKey(interactionKey) ? 1 : 0;
    }
}
