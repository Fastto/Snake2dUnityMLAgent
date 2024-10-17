using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnakeGame : MonoBehaviour
{
    public SnakeGameAgent02 m_Agent;
    public Transform snakeSegmentPrefab;
    public Transform wallSegmentPrefab;
    public Transform foodSegmentPrefab;
    
    public Vector2Int gridSize = new Vector2Int(20, 20);

    [HideInInspector] public List<Transform> snakeSegments = new List<Transform>();
    private List<Transform> wallSegments = new List<Transform>();
    [HideInInspector] public Vector2 direction = Vector2.right;
    private float movementTimer = 0f;
    private float movementInterval = 0.2f; 

    [HideInInspector] public Vector2Int currentFoodPosition;
    private Transform food;

    [HideInInspector] public Action OnFood;
    [HideInInspector] public Action OnDeath;

    private void Start()
    {
        // ResetGame();
    }

    private void Update()
    {
        // HandleInput();
        movementTimer += Time.deltaTime;

        if (movementTimer >= movementInterval)
        {
            movementTimer = 0f;
            m_Agent.RequestDecision();
            MoveSnake();
        }
    }

    private void HandleInput()
    {
        // if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
        //     direction = Vector2.up;
        // else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
        //     direction = Vector2.down;
        // else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
        //     direction = Vector2.left;
        // else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
        //     direction = Vector2.right;
    }

    public void MoveSnake()
    {
        Vector3 newPosition = snakeSegments[0].position + (Vector3)direction;

        // Check if the snake hits the border
        if (newPosition.x - transform.position.x < -gridSize.x / 2 || newPosition.x - transform.position.x > gridSize.x / 2 ||
            newPosition.y- transform.position.y < -gridSize.y / 2 || newPosition.y - transform.position.y > gridSize.y / 2)
        {
            OnDeath?.Invoke();
            // ResetGame();
            return;
        }

        // Check if the snake hits itself
        foreach (var segment in snakeSegments)
        {
            if (newPosition == segment.position)
            {
                OnDeath?.Invoke();
                // ResetGame();
                return;
            }
        }

        // Move the snake
        for (int i = snakeSegments.Count - 1; i > 0; i--)
        {
            snakeSegments[i].position = snakeSegments[i - 1].position;
        }
        snakeSegments[0].position = newPosition;

        // Check if the snake eats food
        if (currentFoodPosition == Vector2Int.RoundToInt(new Vector2(newPosition.x - transform.position.x, 
                newPosition.y - transform.position.y)))
        {
            OnFood?.Invoke();
            GrowSnake();
            GenerateFood();
        }
    }

    public void ResetGame()
    {
        // Remove all segments
        foreach (var segment in snakeSegments)
        {
            Destroy(segment.gameObject);
        }

        snakeSegments.Clear();
        
        foreach (var segment in wallSegments)
        {
            Destroy(segment.gameObject);
        }
        
        wallSegments.Clear();

        for (int i = (-gridSize.x/2)-1; i < (gridSize.x/2) + 2; i++)
        {
            Transform wallSegment = Instantiate(wallSegmentPrefab, new Vector3(i,  (-gridSize.y/2)-1, 0) + transform.position, Quaternion.identity, transform);
            wallSegments.Add(wallSegment);
            wallSegment = Instantiate(wallSegmentPrefab, new Vector3(i, (gridSize.y/2)+1, 0) + transform.position, Quaternion.identity, transform);
            wallSegments.Add(wallSegment);
        }
        
        for (int i = (-gridSize.y/2); i < (gridSize.y/2) + 1; i++)
        {
            Transform wallSegment = Instantiate(wallSegmentPrefab, new Vector3((-gridSize.y/2)-1, i, 0) + transform.position, Quaternion.identity, transform);
            wallSegments.Add(wallSegment);
            wallSegment = Instantiate(wallSegmentPrefab, new Vector3((gridSize.y/2)+1, i, 0) + transform.position, Quaternion.identity, transform);
            wallSegments.Add(wallSegment);
        }
        
        
        // Add a head segment
        Transform headSegment = Instantiate(snakeSegmentPrefab, Vector3.zero + transform.position, Quaternion.identity, transform);
        snakeSegments.Add(headSegment);

        // Reset direction
        direction = Vector2.right;

        GenerateFood();
    }

    private void GrowSnake()
    {
        Transform newSegment = Instantiate(snakeSegmentPrefab, snakeSegments[snakeSegments.Count - 1].position, Quaternion.identity, transform);
        snakeSegments.Add(newSegment);
    }

    private void GenerateFood()
    {
        if (food != null)
        {
            Destroy(food.gameObject);
        }

        currentFoodPosition = new Vector2Int(Random.Range(-gridSize.x / 2, gridSize.x / 2), Random.Range(-gridSize.y / 2, gridSize.y / 2));
        food = Instantiate(foodSegmentPrefab, new Vector3(currentFoodPosition.x, currentFoodPosition.y, 0f) + transform.position, Quaternion.identity, transform);
    }
}
