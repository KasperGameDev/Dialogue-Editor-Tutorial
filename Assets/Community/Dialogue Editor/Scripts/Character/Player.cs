using UnityEngine;

public class Player : Character
{

    [Header("Charcater Details")]


    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Animator Parameters")]
    public int move;
    public float speed;

    public bool facingLeft = false;
    bool turn = false;
    public bool moveUp = true;
    public bool moveDown = true;

    [Header("UI Player Controller Buttons")]

    float horizontal;
    float vertical;

    public bool playerEnabled = true;

    [Header("Touch Actions")]
    public Vector2 touchPosition = new Vector2();

    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (playerEnabled)
        {
            Movement(horizontal, vertical);

            transform.position += Vector3.right * horizontal * speed * Time.deltaTime +
                Vector3.up * vertical * speed * Time.deltaTime;

            if (turn)
            {
                turn = false;
                transform.Rotate(0, 180, 0, Space.World);
            }
            //_currentWeapon.GetComponent<SpriteRenderer>().facingLeft = facingLeft;

            animator.SetInteger("Move", move);
            //animator.SetBool("Aim", crouch);
        }
    }

    public void Movement(float horizontal, float vertical)
    {

        Vector2 inputMovement = Vector2.zero;
        inputMovement.y = Input.GetAxis("Vertical");

        if (!moveUp && inputMovement.y > 0 || !moveDown && inputMovement.y < 0)
        {
            vertical = 0;
        }
        else
            vertical = inputMovement.y;

        if (horizontal != 0)
        {
            if (horizontal > 0)
            {
                move = 1;
                turn = facingLeft ? true : false;
                facingLeft = false;
            }

            else if (horizontal < 0)
            {
                move = -1;
                turn = facingLeft ? false : true;
                facingLeft = true;
            }
        }
        else if (vertical != 0)
        {
            move = 1;
        }
        else
        {
            move = 0;
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("BoundsUpper"))
        {
            moveUp = false;
            vertical = 0;
        }

        if (collision.gameObject.name.Equals("BoundsLower"))
        {
            moveDown = false;
            vertical = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("BoundsUpper") && !moveUp)
        {
            moveUp = true;
        }

        if (collision.gameObject.name.Equals("BoundsLower") && !moveDown)
        {
            moveDown = true;
        }
    }
}