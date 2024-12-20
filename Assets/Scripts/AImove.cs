using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AImove : Agent
{
    public Transform target; // Target to reach
    public float moveSpeed = 5f; // Movement speed
    private Rigidbody2D rBody; // Rigidbody component
    [SerializeField] private List<Transform> spawnPoints; // Spawn points for episodes
    [SerializeField] private List<Transform> obstacles; // List of obstacles

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the agent's position to a random spawn point
        transform.localPosition = spawnPoints[Random.Range(0, spawnPoints.Count)].localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Gather observations about the agent's position, target, and obstacles
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation((target.localPosition - transform.localPosition).normalized);

        foreach (Transform obstacle in obstacles)
        {
            Vector3 relativePosition = obstacle.localPosition - transform.localPosition;
            sensor.AddObservation(relativePosition.normalized);
            sensor.AddObservation(Vector3.Distance(transform.localPosition, obstacle.localPosition));
        }

        // Add additional obstacle observations
        sensor.AddObservation(obstacles.Count);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Capture the actions determined by the neural network
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        // Move the agent in the specified direction
        Vector3 move = new Vector3(moveX, moveY, 0).normalized * moveSpeed * Time.deltaTime;
        transform.position += move;

        // Reward based on distance to the target
        float distanceToTarget = Vector2.Distance(transform.localPosition, target.localPosition);
        AddReward(-0.01f * distanceToTarget); // Encourage getting closer to the target

        // Reward for reaching the target
        if (distanceToTarget < 1.0f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Penalty for getting too close to an obstacle
        foreach (Transform obstacle in obstacles)
        {
            float distanceToObstacle = Vector2.Distance(transform.localPosition, obstacle.localPosition);
            if (distanceToObstacle < 1.0f)
            {
                AddReward(-0.5f);
                break;
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allow manual control for testing
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // End the episode on reaching the side
        if (other.CompareTag("Side"))
        {
            SetReward(1.0f);
            EndEpisode();
        }
    }
}
