using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class RobotManager : MonoBehaviour
{
    // Enum to represent the different states of the robot
    enum RobotState
    {
        OnPark,
        OnProduct,
        OnDestination,
    }

    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI infoText;

    private List<Transform> robotList = new List<Transform>();
    private List<RobotState> robotStates = new List<RobotState>();

    private int currentRobotIndex = 0;
    private int destIndex = 0;
    private int deliveredObjects = 0;
    private int remainingObjects;

    private void Start()
    {
        // Subscribe to the button's click event
        startButton.onClick.AddListener(StartNextRobot);

        // Initialize robots and update remaining objects
        InitializeRobots();
        remainingObjects = Product.productTransform.childCount;
        UpdateInfoText();
    }

    private void Update()
    {
        // Update the information text on each frame
        UpdateInfoText();

        // Update robot states and move them
        UpdateRobotStates();
        MoveRobots();
    }

    private void UpdateInfoText()
    {
        int workingRobots = 0;
        int fullParks = 0;

        // Count the number of robots in different states
        for (int i = 0; i < robotStates.Count; i++)
        {
            if (robotStates[i] == RobotState.OnPark)
            {
                fullParks++;
            }
            else if (robotStates[i] == RobotState.OnProduct || robotStates[i] == RobotState.OnDestination)
            {
                workingRobots++;
            }
        }

        // Update the UI text to show relevant information
        infoText.text = $"Working robots: {workingRobots}/{robotList.Count}\r\nFull parks: {fullParks}/{Park.parkTransform.childCount}\r\n\nRemaining objects: {remainingObjects}\r\nDelivered objects: {deliveredObjects}";
    }

    private void InitializeRobots()
    {
        // Initialize robot list and states based on the children of this transform
        foreach (Transform child in transform)
        {
            robotList.Add(child);
            robotStates.Add(RobotState.OnPark);
        }
    }

    private void UpdateRobotStates()
    {
        // Release robots from parked state
        for (int i = 0; i < robotStates.Count; i++)
        {
            if (robotStates[i] != RobotState.OnPark)
            {
                Park.MakeAvailable(i);
            }
        }

        // Find the next available robot to start
        for (int i = 0; i < robotStates.Count; i++)
        {
            if (robotStates[i] == RobotState.OnPark)
            {
                currentRobotIndex = i;
                break;
            }
        }
    }

    private void MoveRobots()
    {
        // Move robots based on their states
        for (int i = 0; i < robotStates.Count; i++)
        {
            Transform robot = robotList[i];
            NavMeshAgent agent = robot.GetComponent<NavMeshAgent>();
            RobotState currentState = robotStates[i];

            switch (currentState)
            {
                case RobotState.OnProduct:
                    if (!agent.pathPending && agent.remainingDistance < 0.1f)
                    {
                        // Attach the product and move to the destination
                        Product.productTransform.GetChild(0).SetParent(robot.transform);
                        MoveRobotToDestination(robot, agent);
                        robotStates[i] = RobotState.OnDestination;
                    }
                    break;
                case RobotState.OnDestination:
                    if (!agent.pathPending && agent.remainingDistance < 0.1f)
                    {
                        // Attach the destination and move back to the park
                        robot.GetChild(1).SetParent(Destination.destinationTransform.GetChild(destIndex));
                        destIndex++;
                        MoveRobotToPark(robot, agent);
                        robotStates[i] = RobotState.OnPark;
                    }
                    break;
            }
        }
    }

    private void StartNextRobot()
    {
        // Start the next robot if conditions are met
        if (robotStates[currentRobotIndex] == RobotState.OnPark && Product.IsAvailable())
        {
            Transform robot = robotList[currentRobotIndex];
            NavMeshAgent agent = robot.GetComponent<NavMeshAgent>();
            MoveRobotToProductLocation(robot, agent);
            robotStates[currentRobotIndex] = RobotState.OnProduct;
        }
    }

    private void MoveRobotToProductLocation(Transform robot, NavMeshAgent agent)
    {
        // Move the robot to the product location
        agent.SetDestination(Product.GetAvailableProduct().position);
    }

    private void MoveRobotToDestination(Transform robot, NavMeshAgent agent)
    {
        // Move the robot to the destination
        agent.SetDestination(Destination.GetLocation().position);
    }

    private void MoveRobotToPark(Transform robot, NavMeshAgent agent)
    {
        // Update remaining and delivered objects, move robot to the park
        remainingObjects--;
        deliveredObjects++;
        agent.SetDestination(Park.GetLocation().position);
    }
}
