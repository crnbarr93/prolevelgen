using UnityEngine;

namespace UnitySampleAssets._2D
{

    public class PlatformerCharacter2D : MonoBehaviour
    {
        private bool facingRight = true; // For determining which way the player is currently facing.

        [SerializeField] private float maxSpeed = 10f; // The fastest the player can travel in the x axis.
        [SerializeField] private float jumpForce = 400f; // Amount of force added when the player jumps.	

        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;
                                                     // Amount of maxSpeed applied to crouching movement. 1 = 100%

        [SerializeField] private bool airControl = false; // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask whatIsGround; // A mask determining what is ground to the character
        [SerializeField] private bool allowDJ = true;
        private bool doubleJ = false;

        private Transform groundCheck; // A position marking where to check if the player is grounded.
        private float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool grounded = false; // Whether or not the player is grounded.
        private Transform ceilingCheck; // A position marking where to check for ceilings
        private float ceilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Transform rightWallCheck;
        private Transform leftWallCheck;
        private float wallCheckRadius = 0.2f;
        private bool rightWall = false;
        private bool leftWall = false;

        private Animator anim; // Reference to the player's animator component.

		Transform playerGraphics;	//Reference to graphics so we can change direction


        private void Awake()
        {
            // Setting up references.
            groundCheck = transform.Find("GroundCheck");
            ceilingCheck = transform.Find("CeilingCheck");
            rightWallCheck = transform.Find("wallCheckRight");
            leftWallCheck = transform.Find("wallCheckLeft");
            anim = GetComponent<Animator>();
			playerGraphics = transform.Find ("pixelman");
			if (playerGraphics == null) {
				Debug.LogError ("Error, no graphics found");
			}
        }


        private void FixedUpdate()
        {
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
            rightWall = Physics2D.OverlapCircle(rightWallCheck.position, wallCheckRadius, whatIsGround);
            leftWall = Physics2D.OverlapCircle(leftWallCheck.position, wallCheckRadius, whatIsGround);
            anim.SetBool("Ground", grounded);

            //if(rightWall) print("WALL ON RIGHT");
            //if(leftWall) print("WALL ON LEFT");

            // Set the vertical animation
            anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
        }


        public void Move(float move, bool crouch, bool jump)
        {

            //Double jump off wall
            if (!grounded){ //If mid air
                if(jump){
                    if (rightWall) { //If close to wall on right
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-jumpForce, jumpForce)); //Jump backwards
                        Flip(); 
                        airControl = false; //Disable air control after jumping off wall
                        doubleJ = false;
                    }
                    if(leftWall){ //Similarly for wall on left
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(jumpForce, jumpForce));
                        Flip();
                        airControl = false;
                        doubleJ = false;
                    }
                    if(!(rightWall || leftWall || doubleJ) && allowDJ && airControl){
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, jumpForce));
                        doubleJ = true;
                    }
                }
            }

            if(grounded){
                if (!airControl) airControl = true;
                if (doubleJ) doubleJ = false;
            }

            // If crouching, check to see if the character can stand up
            if (!crouch && anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
                    crouch = true;
            }

            // Set whether or not the character is crouching in the animator
            anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (grounded || airControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*crouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                GetComponent<Rigidbody2D>().velocity = new Vector2(move*maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !facingRight)
                    // ... flip the player.
                    Flip();
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && facingRight)
                    // ... flip the player.
                    Flip();
            }
            // If the player should jump...
            if (grounded && jump && anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                grounded = false;
                anim.SetBool("Ground", false);
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
            }

        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
			Vector3 theScale = playerGraphics.localScale;
            theScale.x *= -1;
			playerGraphics.localScale = theScale;
        }
    }
}