using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SnakeGameAgent : Agent
{
    [SerializeField] private SnakeGame m_Game;
    
    
    private void Start()
    {
        m_Game.OnFood += () =>
        {
            AddReward(1f); 
        };
        
        m_Game.OnDeath += () =>
        {
            // SetReward(-1f);
            EndEpisode();
        };
    }

    public override void OnEpisodeBegin()
    {
        m_Game.ResetGame();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float[,] array = new float[m_Game.gridSize.x+1, m_Game.gridSize.y+1];

        var foodPosition = new Vector2Int(m_Game.currentFoodPosition.x + m_Game.gridSize.x / 2, 
            m_Game.currentFoodPosition.y + m_Game.gridSize.y / 2);
        array[foodPosition.x, foodPosition.y] = 1;

        bool first = true;
        m_Game.snakeSegments.ForEach(segment =>
        {
            var value = -1f;
            if (first)
            {
                value = 0.5f;
                first = false;
            }
            var segmentPosition = Vector2Int.RoundToInt(
                    new Vector2(segment.localPosition.x + m_Game.gridSize.x / 2f, 
                (segment.localPosition.y) + (m_Game.gridSize.y / 2)));
            
            array[segmentPosition.x, segmentPosition.y] = value;
            
        });

        
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                sensor.AddObservation(array[i, j]);
            }
        }

        sensor.AddObservation(m_Game.direction == Vector2.up ? 1 : 0);
        sensor.AddObservation(m_Game.direction == Vector2.down ? 1 : 0);
        sensor.AddObservation(m_Game.direction == Vector2.left ? 1 : 0);
        sensor.AddObservation(m_Game.direction == Vector2.right ? 1 : 0);
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var discreteActions = actions.DiscreteActions;
        if (discreteActions[0] == 1 && m_Game.direction != Vector2.down)
            m_Game.direction = Vector2.up;
        else if (discreteActions[0] == 2 && m_Game.direction != Vector2.up)
            m_Game.direction = Vector2.down;
        else if (discreteActions[0] == 3 && m_Game.direction != Vector2.right)
            m_Game.direction = Vector2.left;
        else if (discreteActions[0] == 4 && m_Game.direction != Vector2.left)
            m_Game.direction = Vector2.right;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions[0] = 0;
        if (Input.GetKey(KeyCode.W))
            actions[0] = 1;
        else if (Input.GetKey(KeyCode.S))
            actions[0] = 2;
        else if (Input.GetKey(KeyCode.A))
            actions[0] = 3;
        else if (Input.GetKey(KeyCode.D))
            actions[0] = 4;
    }
}
