using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class Wall : MonoBehaviour
    {
        public AudioClip chopSound1;                
        public AudioClip chopSound2;                
        public Sprite dmgSprite;                   
        public int hp = 3;                         

        private SpriteRenderer spriteRenderer;      

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        //DamageWall is called when the player attacks a wall.
        public void DamageWall(int loss)
        {
            SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
            spriteRenderer.sprite = dmgSprite;
            hp -= loss;
            if (hp <= 0)
            {
                //Disable the gameObject.
                gameObject.SetActive(false);
            }
        }
    }

}