using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
	public float turnDelay = 0.1f;
	public int playerFoodPoints = 100;
	public static GameManager instance = null;
	public bool playerTurn = true;

	private Text levelText;
	[SerializeField]private GameObject levelImage;
	private BoardManager boardScript;
	private int level = 1;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup = true;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();

		InitGame();
	}


	//this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

	//call functions everytime scene is loaded
	static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		instance.level++;//harder per load
		instance.InitGame();
	}


	void InitGame()//initializes game per level
	{
		doingSetup = true; //player cant move while setting up
		//LEVEL TEXT
		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Floor " + level;
		levelImage.SetActive(true);

		//hidelevel invoke to hide levelimage after delay
		Invoke("HideLevelImage", levelStartDelay);
		enemies.Clear();
		
		boardScript.SetupScene(level);
	}


	void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}

	void Update()
	{
		if(playerTurn || enemiesMoving || doingSetup)
		{
			return;
		}
		StartCoroutine(MoveEnemies());
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true; //if true, player cant move

		yield return new WaitForSeconds(turnDelay);

		if(enemies.Count == 0)
		{
			yield return new WaitForSeconds(turnDelay);
		}

		for(int i = 0; i< enemies.Count; i++)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playerTurn = true;
		enemiesMoving = false;
	}

	public void GameOver()
	{
		levelText.text = "After " + level + "floors, you died";

		levelImage.SetActive(true);
		enabled = false; //disable gamemanager
	}
	
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}
}
