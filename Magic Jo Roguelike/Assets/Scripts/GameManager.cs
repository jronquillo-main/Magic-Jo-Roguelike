using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		
	using UnityEngine.UI;					
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						
		public float turnDelay = 0.1f;							
		public int playerFoodPoints = 100;						
		public static GameManager instance = null;				
		[HideInInspector] public bool playersTurn = true;		
		
		
		private Text levelText;								
		private GameObject levelImage;							
		private BoardManager boardScript;						
		private int level = 1;									
		private List<Enemy> enemies;							
		private bool enemiesMoving;								
		private bool doingSetup = true;							
		
		
		
		//Awake is always called before any Start functions
		void Awake()
		{
            if (instance == null)
			{
                instance = this;
			}
            else if (instance != this)
			{
                Destroy(gameObject);//singleton so only 1 game manager allowed
			}

			DontDestroyOnLoad(gameObject);
			
			enemies = new List<Enemy>();
			boardScript = GetComponent<BoardManager>();
			
			InitGame(); //initialize level
		}

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            SceneManager.sceneLoaded += OnSceneLoaded; //called everytime scene is loaded
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;
            instance.InitGame();
        }

		
		//Initializes the game for each level.
		void InitGame()
		{
			doingSetup = true;
			levelImage = GameObject.Find("LevelImage");
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			levelText.text = "Floor " + level;
			levelImage.SetActive(true);
			
			Invoke("HideLevelImage", levelStartDelay);
			enemies.Clear();
			boardScript.SetupScene(level);
			
		}
		
		
		void HideLevelImage()
		{
			levelImage.SetActive(false); //hide level image to show map
			doingSetup = false; //false so player can move
		}
		
		//Update is called every frame.
		void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup)
			{
				return;
			}
			//Start moving enemies.
			StartCoroutine (MoveEnemies());
		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			levelText.text = "After " + level + " floors, you died.";
			levelImage.SetActive(true);

			enabled = false;
		}
		
		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);
			
			//If there are no enemies spawned (IE in first level):
			if(enemies.Count == 0) 
			{
				yield return new WaitForSeconds(turnDelay);
			}
			
			//Loop through List of Enemy objects.
			for(int i = 0; i < enemies.Count; i++)
			{
				enemies[i].MoveEnemy();
				yield return new WaitForSeconds(enemies[i].moveTime);
			}

			//end turn
			playersTurn = true;
			enemiesMoving = false;
		}
	}
}

