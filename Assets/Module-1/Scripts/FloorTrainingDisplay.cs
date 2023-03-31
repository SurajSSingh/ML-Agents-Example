using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrainingDisplay : MonoBehaviour
{
    Queue<bool> episodes = new();

    [SerializeField, Tooltip("How far does the display look back when determining the success percentage. 0 means use the entire queue.")] uint episodeQueueSize = 100;
    [SerializeField] Color zeroPercentSuccessColor = Color.red;
    [SerializeField] Color hundredPercentSuccessColor = Color.green;

    [SerializeField] MeshRenderer floorMesh;

    private void Awake()
    {
        if (floorMesh)
        {
            floorMesh.material.color = zeroPercentSuccessColor;
        }
    }

    public void OnTaskComplete(bool isSuccessfulEpisode)
    {
        if (episodeQueueSize != 0 && episodes.Count >= episodeQueueSize)
        {
            episodes.TryDequeue(out _);
        }
        episodes.Enqueue(isSuccessfulEpisode);

        float successes = 0;
        foreach (bool episodeSuccess in episodes)
        {
            if (episodeSuccess) successes += 1;
        }
        Debug.Log((successes, episodes.Count, successes / (float)episodes.Count));

        if (floorMesh)
        {
            floorMesh.material.color = Color.Lerp(zeroPercentSuccessColor, hundredPercentSuccessColor, successes / (float)episodes.Count);
        }
    }

}
