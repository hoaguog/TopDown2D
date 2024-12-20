using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class HardAI : Agent
{
    public Transform target; // Target to reach
    public float moveSpeed = 5f; // Movement speed
    private Rigidbody2D rBody; // Rigidbody component
    [SerializeField] private Transform spawnPoints; // Spawn points for episodes
    [SerializeField] private List<Transform> obstacles; // List of obstacles
    [SerializeField] private FieldOfView fov;
    public AIWeapon wp;
    public string AIName;
    public int heath = 100;
    public CameraFolow UIcam;
    [SerializeField] private bool isInSide = false;
    public DataReadWrite dt;
    public float maxTimeToReachSide = 90f;
    private float elapsedTime = 0f;
    public int killCount = 0;
    public int score = 0;
    public bool isDead = false;
    private BOTscoreboard board;
    private float timeInSide = 0f;
    [SerializeField] AudioSource footStep;
    [SerializeField] AudioClip step;
    private bool isMove=false;
    public override void Initialize()
    {
        killCount = 0;
        rBody = GetComponent<Rigidbody2D>();
        fov = this.GetComponent<FieldOfView>();
        wp = this.GetComponentInChildren<AIWeapon>();
        dt = this.GetComponentInChildren<DataReadWrite>();
        board = this.GetComponent<BOTscoreboard>();
        if (wp == null)
        {
            Debug.LogError("AIWeapon component not found on " + gameObject.name);
        }

        wp.shooter = gameObject.name;
        board.kill = killCount;
        board.score = score;
    }
    public void AddKill()
    {
        killCount++;
        //Debug.Log($"{gameObject.name} kill {killCount} men");
    }


    public override void OnEpisodeBegin()
    {
        // Reset the agent's position to a random spawn point
        elapsedTime = 0f;
        //dt.GetWeaponByName("SMG");
        wp.LoadWeaponInfo();
        transform.localPosition = spawnPoints.localPosition;
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
    private void Update()
    {
        if (isInSide)
        {
            timeInSide += Time.deltaTime;
            if (timeInSide >= 2f)
            {
                score += 5;
                timeInSide = 0f;
            }
        }
        else { timeInSide = 0f; }
        if (isMove)
        {
            if (!footStep.isPlaying)
            {
                footStep.PlayOneShot(step);

            }
        }
        board.score = this.score;
        board.kill = this.killCount;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= maxTimeToReachSide)
        {
            if (!isInSide)
            {
                AddReward(-1f);
                EndEpisode();
                Debug.Log("Failed to reach Side in time. Ending episode.");
                return;
            }

        }
        if (fov != null && wp.remainingAmmo >= 0 && wp.ammoCapacity > 0)
        {
            fov.FindVisibleTargets();
            if (fov.visibleTargets.Count > 0)
            {
                Transform firstTarget = fov.visibleTargets[0];
                //Debug.Log($"First target: {firstTarget.name}");
                Vector3 directionToTarget = (firstTarget.localPosition - transform.localPosition).normalized;
                RotateTowardsDirection(directionToTarget);
                if (wp != null)
                {
                    wp.Fire();
                    AddReward(0.1f);
                }
                else
                {
                    Debug.Log("no wp");
                }
                elapsedTime = elapsedTime -0.1f;
                AddReward(0.01f);
                return;
            }
        }
        else
        {
            wp.ResetRecoil();
        }

        // Capture the actions determined by the neural network
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        // Move the agent in the specified direction
        Vector3 move = new Vector3(moveX, moveY, 0).normalized * moveSpeed * Time.deltaTime;
        transform.position += move;
        // Rotate the agent to face the movement direction
        if (move.sqrMagnitude > 0.001f) // Check if there's significant movement
        {
            float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg;
            rBody.MoveRotation(angle); // Rotate using Rigidbody2D
            isMove = true;
        }
        else
        {
            isMove = false;
        }

        // Reward based on distance to the target (Side)
        float distanceToTarget = Vector2.Distance(transform.localPosition, target.localPosition);
        AddReward(-0.001f * distanceToTarget); // Slight penalty for being far from the target

        // Reward for reaching the target (Side)
        if (distanceToTarget < 1.0f)
        {
            SetReward(0.2f); // Higher reward for reaching the Side quickly
            //EndEpisode();
        }
        if (distanceToTarget < 0.01f)
        {
            SetReward(2f);
            EndEpisode();
        }

        // Penalty for getting too close to an obstacle
        foreach (Transform obstacle in obstacles)
        {
            float distanceToObstacle = Vector2.Distance(transform.localPosition, obstacle.localPosition);
            if (distanceToObstacle < 1.0f)
            {
                AddReward(-0.5f);
                //EndEpisode(); // End episode if agent hits an obstacle
                break;
            }
        }

        // Small penalty for time taken to encourage faster completion
        AddReward(-0.001f);
        if (isInSide)
        {
            AddReward(0.2f); // Continuous reward for staying in the Side
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
        // Check if AI enters the Side
        if (other.CompareTag("Side"))
        {
            isInSide = true; // Set flag to true when AI enters the Side
            elapsedTime = 0f;
            AddReward(2.0f); // Give an initial reward for reaching the Side
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if AI leaves the Side
        if (other.CompareTag("Side"))
        {
            isInSide = false; // Set flag to false when AI leaves the Side
            AddReward(-1.0f); // Penalize for leaving the Side
        }
    }
    public void TakeDamage(int bulletDamage, string shooter)
    {
        heath -= bulletDamage;
        if (heath <= 0)
        {
            AddReward(-5f);
            EndEpisode();
            isDead = true;
            UIcam.ShowKillName(shooter, gameObject.name);
            heath = 100;
        }
    }
    private void RotateTowardsDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}