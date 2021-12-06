using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    public GameObject playerNameInput;

    private static int highScore = 0;
    private static string highScorePlayerName = "";
    
    private bool m_Started = false;
    private static int m_Points;
    private static string playerName = "";
    private bool m_GameOver = false;
    
    void Awake()
    {
        if (Instance != null)
        {
            Instance.BrickPrefab = BrickPrefab;
            Instance.LineCount = LineCount;
            Instance.Ball = Ball;
            Instance.ScoreText = ScoreText;
            Instance.GameOverText = GameOverText;
            Instance.m_Started = false;
            m_Points = 0;
            Instance.m_GameOver = false;

            CreateBricks();
            LoadScore();

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void CreateBricks()
    {
        if (SceneManager.GetActiveScene().name == "main")
        {
            const float step = 0.6f;
            int perLine = Mathf.FloorToInt(4.0f / step);

            int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
            for (int i = 0; i < LineCount; ++i)
            {
                for (int x = 0; x < perLine; ++x)
                {
                    Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brick.PointValue = pointCountArray[i];
                    brick.onDestroyed.AddListener(Instance.AddPoint);
                }
            }
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "main")
        {
            if (!m_Started)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_Started = true;
                    float randomDirection = Random.Range(-1.0f, 1.0f);
                    Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                    forceDir.Normalize();

                    Ball.transform.SetParent(null);
                    Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
                }
            }
            else if (m_GameOver)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveScore();
    }

    public void StartGame()
    {
        playerName = playerNameInput.GetComponent<TMP_InputField>().text;

        SceneManager.LoadScene(1);
    }

    public class SaveData
    {
        public int highScore;
        public string highScorePlayer;
    }

    public void SaveScore()
    {
        if (m_Points >= highScore)
        {
            SaveData data = new SaveData();

            data.highScore = m_Points;
            data.highScorePlayer = playerName;

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        }
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.highScore;
            highScorePlayerName = data.highScorePlayer;

            HighScoreText.text = "High Score: " + highScore + " Name: " + highScorePlayerName;
        }
        else
        {
            highScore = 0;
            highScorePlayerName = "";
        }
    }
}
