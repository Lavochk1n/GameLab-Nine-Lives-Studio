using Quarantine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int score = 0;
    private float timeLeft = 60f;
    private float difficulty = 100; 


    [SerializeField] private float newGameTime = 210f;

    [SerializeField][Range(50.0f, 200.0f)] private float newGameDifficulty = 100f;
    [SerializeField][Range(1f, 1.50f)] private float difficultyIncrease = 1.1f;
    
    public PlayerBehaviour playerBehaviour1, playerBehaviour2;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    ////////////////////////////////// SCORE ////////////////////////////

    public void IncreaseScore(int amount)
    {
        score += amount;
    }

    public int GetScore()
    {
        return score;
    }

    ////////////////////////////////// TIME ////////////////////////////

    public void DecreaseTime()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft < 0)
        {
            QuarentineManager.Instance.AmbulanceArrival();
            timeLeft = newGameTime;

        }
    }

    public void AddTime(float amount)
    {
        
        timeLeft += amount; 
        if (timeLeft > newGameTime)
        {
            timeLeft = newGameTime;
        }
    }

    public float GetTotalGameTime()
    {
        return newGameTime;
    }


    public float GetTimeLeft()
    {
        return timeLeft;
    }


    ////////////////////////////////// Difficulty ////////////////////////////


    public float IncreaseDifficulty()
    {
        return difficulty *= difficultyIncrease;
    }

    public float GetDifficultyRatio()
    {
        return difficulty/100;
    }

    
    

    public void Reset()
    {
        score = 0;
        difficulty = newGameDifficulty;
        timeLeft = newGameTime; 
    }
}
