using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SnakeGameAgent02 : Agent
{
    [SerializeField] private SnakeGame m_Game;
    
    
    private void Start()
    {
        m_Game.OnFood += () =>
        {
            AddReward(1f/100f); 
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


        m_Game.snakeSegments.ForEach(segment =>
        {
            var segmentPosition = Vector2Int.RoundToInt(
                    new Vector2(segment.localPosition.x + m_Game.gridSize.x / 2f, 
                (segment.localPosition.y) + (m_Game.gridSize.y / 2)));
            
            array[segmentPosition.x, segmentPosition.y] = -1;
            
        });

        var headPos =  Vector2Int.RoundToInt(
            new Vector2(m_Game.snakeSegments[0].localPosition.x + m_Game.gridSize.x / 2f, 
                (m_Game.snakeSegments[0].localPosition.y) + (m_Game.gridSize.y / 2)));
        
            for (int x = headPos.x - 2; x < headPos.x + 3; x++)
            { 
                for (int y = headPos.y - 2; y < headPos.y + 3; y++)
                {
                    try
                    {
                        sensor.AddObservation(array[x, y]);
                    }
                    catch
                    {
                        sensor.AddObservation(-1);
                    }
                }
            }

            var foodpos = m_Game.currentFoodPosition;
            Vector2 fp = (Vector2)foodpos;
            Vector2 hp = new Vector2(m_Game.snakeSegments[0].transform.localPosition.x,
                m_Game.snakeSegments[0].transform.localPosition.y);
            
            sensor.AddObservation(m_Game.direction == Vector2.up ? 1 : 0);
            sensor.AddObservation(m_Game.direction == Vector2.down ? 1 : 0);
            sensor.AddObservation(m_Game.direction == Vector2.left ? 1 : 0);
            sensor.AddObservation(m_Game.direction == Vector2.right ? 1 : 0);
            
            sensor.AddObservation((fp - hp).normalized);
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
