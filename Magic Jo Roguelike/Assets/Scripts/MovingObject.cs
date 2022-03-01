using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer ;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    private bool isMoving;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        inverseMoveTime = 1f/moveTime;
    }

    //move true if able to move false if not
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir,yDir);

        boxCollider.enabled = false;

        hit = Physics2D.Linecast(start,end,blockingLayer);

        boxCollider.enabled = true;

        //check if nothing hit by linecast 
        if(hit.transform == null && !isMoving)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        
        //if something hit, return false(meaning there is obstacle on the way). move unsuccessful
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        rb2D.MovePosition(end);
        isMoving = false;
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir) 
        where T: Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir,yDir,out hit);

        if(hit.transform == null)
        {
            return;
        }
        T hitComponent = hit.transform.GetComponent<T>();

        if(!canMove && hitComponent!=null)//object blocked by interactable
        {
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component) 
        where T : Component;
}
