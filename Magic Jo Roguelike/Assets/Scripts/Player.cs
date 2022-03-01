using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f; //delay before level restart
    public int foodPoints = 10; //points to add when interacted food
    public int sodaPoints = 20; //points to add when interacted soda
    public int wallDamage = 1; //player damage to wall when attacked

    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

    protected override void Start() //overides start function of MovingObject script
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        base.Start();
    }

    private void OnDisable()
    {
        //store current food total when player object is disabled
        //to keep score when going to another level
        GameManager.instance.playerFoodPoints = food;
    }

    void Update()
    {
        if (!GameManager.instance.playerTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxis("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal,vertical);//call attempt move since player might interact with it
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //overrifes attempt move at movingobject script
        food--;//player moving reduces health

        //update food text to reflect current food
        foodText.text = "Food " + food;

        base.AttemptMove<T>(xDir, yDir);//call attempt move of MovingObject passes component T

        RaycastHit2D hit; //result of linecast in move

        if (Move(xDir, yDir, out hit))
        {
            //play walking sound
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver(); //player losing food. might be gameover

        GameManager.instance.playerTurn = false; //set to false because player turn over
    }

    protected override void OnCantMove<T>(T component)
    {
        //Set hitWall to equal the component passed in as a parameter.
        Wall hitWall = component as Wall;

        //Call the DamageWall function of the Wall we are hitting.
        hitWall.DamageWall(wallDamage);

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger("playerChop");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
        }

        else if (other.tag == "Food")
        {
            food += foodPoints;
            foodText.text = "+" + foodPoints + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }

        else if (other.tag == "Soda")
        {
            food += sodaPoints;
            foodText.text = "+" + sodaPoints + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food-=loss;

        foodText.text="-" + loss + " Food: " + food;

        CheckIfGameOver();
    }

    void CheckIfGameOver()
    {
        if(food<=0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
