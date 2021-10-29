using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float forceWallSlide;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = 0.015f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask whatGround;
    [SerializeField] private ParticleSystem runParticle;
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private GameObject grave;
    [SerializeField] private SwitchYonInLevel SYIL;

    private float radiusGroundCheck = .1f;
    private Vector2 sizewallCheck = new Vector2(.03f, .6f);
    private bool grounded;
    private bool wall;
    private int jumpCount;
    private bool wallCheckRevers;
    private Rigidbody2D rigitbody;
    private Vector3 r_Velocity = Vector3.zero;

    private Animator _animator;
    private int _directionX;
    private StateCharacter _stateCharacter;
    private bool _jump;
    private bool _dontMove;
    private bool _death;

    [HideInInspector] public Animator animator {get => _animator; set => _animator = value; }
    [HideInInspector] public int directionX { get => _directionX; set => _directionX = value; }
    [HideInInspector] public StateCharacter stateCharacter { get => _stateCharacter; set => _stateCharacter = value; }
    [HideInInspector] public bool jump { get => _jump; set => _jump = value; }
    [HideInInspector] public bool dontMove { get => _dontMove; set => _dontMove = value; }
    [HideInInspector] public bool death { get => _death; set => _death = value; }

    //forclimb
    private float sizeLineForClimbCheck = .8f;
    private float positionYcheckGroundForClimb = -.3f;
    private float positionYcheckFreeForClimb = .5f;
    private Vector2 i_TargetPosition;
    private Coroutine climbCoroutine;

    private Transform startPosition;
    

    private void Awake()
    {
        I.c_Controller = this;

        _directionX = 0;
        _stateCharacter = StateCharacter.Idle;
        rigitbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        jumpParticle.Stop();
    }

    private void FixedUpdate()
    {
        if (_death)
            return;
        CheckGround();
        CheckWall();

        if (_stateCharacter == StateCharacter.Run || _stateCharacter == StateCharacter.Idle 
            || _stateCharacter == StateCharacter.Jump || _stateCharacter == StateCharacter.Fall)
            Move();
        if (_stateCharacter == StateCharacter.WallSlide)
            WallSlide();
        if (_jump && _stateCharacter != StateCharacter.Climb)
            Jump();

        //падение
        if (rigitbody.velocity.y < 0 && !grounded && _stateCharacter != StateCharacter.Fall && _stateCharacter != StateCharacter.WallSlide)
        {
            StateAnimation(false, true, false, false);//jump fall wallslide climb
            _stateCharacter = StateCharacter.Fall;
        }

        if (_stateCharacter == StateCharacter.Run && !runParticle.isPlaying)
            runParticle.Play();
        else if (_stateCharacter != StateCharacter.Run && runParticle.isPlaying)
            runParticle.Stop();
    }

    private void Update()
    {
        _animator.SetFloat("Speed", Mathf.Abs(_directionX));
    }

    private void Move()
    {
        int newDir = _directionX;
        if ((_stateCharacter == StateCharacter.Jump || _stateCharacter == StateCharacter.Fall) && !_dontMove)
            newDir = (int)transform.localScale.x;
        Vector3 targetVelocity = rigitbody.velocity;
        targetVelocity.x = newDir * speed * Time.fixedDeltaTime;
        rigitbody.velocity = Vector3.SmoothDamp(rigitbody.velocity, targetVelocity, ref r_Velocity, movementSmoothing);
    }

    public void Jump(bool another = false)
    {
        if (!_jump && !another)
            return;
        if (grounded)
            grounded = false;

        if (another)
            jumpCount = 1;
        //если прыжок третий или больше -> прерываем
        if (jumpCount < 2)
            jumpCount++;
        else
            return;

        I.audioManager.Play("Jump");

        if(_stateCharacter == StateCharacter.WallSlide)
        {
            WallCheckFlip();
        }

        if(jumpCount == 2)
        {
            jumpParticle.transform.position = transform.position + Vector3.down * .5f;
            jumpParticle.Play();
        }
        _stateCharacter = StateCharacter.Jump;
        StateAnimation(true, false, false, false);
        ZeroVelocityY();
        rigitbody.AddForce(new Vector2(0f, jumpForce));
        _jump = false;
    }

    private void WallSlide()
    {
        rigitbody.velocity = new Vector2(rigitbody.velocity.x, Mathf.Clamp(rigitbody.velocity.y, -forceWallSlide, 3));
    }

    public void Stop(bool finish = false)
    {
        if(_stateCharacter == StateCharacter.Run)
            _stateCharacter = StateCharacter.Idle;
        _directionX = 0;
    }
    public void Run(bool finish = false)
    {
        if (_stateCharacter == StateCharacter.Idle)
            _stateCharacter = StateCharacter.Run;
        _directionX = (int)transform.localScale.x;
    }

    private void StopClimb()
    {
        rigitbody.bodyType = RigidbodyType2D.Dynamic;
        StateAnimation(false, false, false, false);
        if (_directionX != 0)
            _stateCharacter = StateCharacter.Run;
        else
            _stateCharacter = StateCharacter.Idle;
        transform.position = new Vector2(transform.position.x + .4f * _directionX, transform.position.y + .5f);
    }

    private void CheckGround()
    {
        bool wasGrounded = grounded;
        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, radiusGroundCheck, whatGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Ground")
            {
                grounded = true;
                jumpCount = 0;
                continue;
            }
        }

        if (grounded)
        {
            if(_stateCharacter == StateCharacter.Jump)
            {
                _animator.SetBool("isJumping", false);
                _stateCharacter = StateCharacter.Run;
            }
            else if (_stateCharacter == StateCharacter.Fall)
            {
                I.audioManager.Play("Hit");
                _animator.SetBool("isFalling", false);
                if(_directionX != 0)
                    _stateCharacter = StateCharacter.Run;
                else
                    _stateCharacter = StateCharacter.Idle;

            }
            else if (_stateCharacter == StateCharacter.WallSlide)
            {
                _animator.SetBool("WallSlide", false);
                WallCheckFlip();
                if (_directionX != 0)
                    _stateCharacter = StateCharacter.Run;
                else
                    _stateCharacter = StateCharacter.Idle;
            }
            if (_dontMove)//если откртыли подсказку hint становится true, а так же если персонаж в воздухе он дальше не летит 
                _dontMove = false;
        }
    }

    private void CheckWall()
    {
        wall = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(wallCheck.position, sizewallCheck, 0, whatGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Ground")
            {
                wall = true;
                continue;
            }
        }

        if (wall && grounded && _stateCharacter != StateCharacter.WallSlide && _stateCharacter != StateCharacter.Climb)
            Flip();
        else if (wall && !grounded && _stateCharacter != StateCharacter.Climb && _stateCharacter != StateCharacter.WallSlide)
        {
            jumpCount = 0;
            _jump = false;
            //CLIMBING START
            if(CheckGroundForClimb())
            {
                ZeroVelocityY();
                StateAnimation(false, false, false, true);
                _stateCharacter = StateCharacter.Climb;
                rigitbody.bodyType = RigidbodyType2D.Static;
                transform.position = i_TargetPosition;
                StartCoroutine(DelayClimb());
            }
            //WALL SLIDE START
            if(_stateCharacter != StateCharacter.Climb)
            {
                _stateCharacter = StateCharacter.WallSlide;
                StateAnimation(false, false, true, false);
                WallCheckFlip();
                Flip();
            }
        }
        else if(!wall && _stateCharacter == StateCharacter.WallSlide)
        {
            _stateCharacter = StateCharacter.Run;
            _animator.SetBool("WallSlide", false);
            WallCheckFlip();
        }
    }

    private bool CheckGroundForClimb()
    {
        bool _checkGround = false;
        bool _checkFree = false;
        bool _bugzz = false;
        for (int j = 0; j < 2; j++)
        {
            Vector2 original = transform.position + Vector3.up * positionYcheckGroundForClimb;
            if (j == 1)
                original = transform.position + Vector3.up * -positionYcheckGroundForClimb;
            RaycastHit2D[] hit = Physics2D.RaycastAll(original, Vector2.right * _directionX, sizeLineForClimbCheck, whatGround);
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider.tag == "Ground")
                {
                    _checkGround = true;
                    i_TargetPosition.x = hit[i].point.x - .21f * _directionX;
                    continue;
                }
            }
            if (_checkGround)
                continue;
        }
        RaycastHit2D[] hit2 = Physics2D.RaycastAll(transform.position + Vector3.up * positionYcheckFreeForClimb, Vector2.right * _directionX, sizeLineForClimbCheck, whatGround);
        for (int i = 0; i < hit2.Length; i++)
        {
            if (hit2[i].collider.tag == "Ground")
            {
                _checkFree = true;
                continue;
            }
        }
        RaycastHit2D[] hit3 = Physics2D.RaycastAll(transform.position + new Vector3(sizeLineForClimbCheck * _directionX, positionYcheckFreeForClimb), Vector2.down, 1, whatGround);
        for (int i = 0; i < hit3.Length; i++)
        {
            if (hit3[i].collider.tag == "Ground")
            {
                i_TargetPosition.y = hit3[i].point.y;
                _bugzz = true;
                continue;
            }
        }
        if (_checkGround && !_checkFree && _bugzz)
            return true;
        return false;
    }

    private void WallCheckFlip()
    {
        wallCheckRevers = !wallCheckRevers;
        Vector2 wc = wallCheck.localPosition;
        wc.x = -wc.x;
        wallCheck.localPosition = wc;
    }

    private void Flip()
    {
        Vector3 _scale = transform.localScale;
        _scale.x *= -1;
        _directionX *= -1;
        transform.localScale = _scale;
    }

    private void ZeroVelocityY()
    {
        Vector2 _velocity = rigitbody.velocity;
        _velocity.y = 0;
        rigitbody.velocity = _velocity;
    }

    private void StateAnimation(bool jumpAnim, bool fallAnim, bool wallSlideAnim, bool climbAnim)
    {
        _animator.SetBool("isJumping", jumpAnim);
        _animator.SetBool("isFalling", fallAnim);
        _animator.SetBool("WallSlide", wallSlideAnim);
        _animator.SetBool("isClimbing", climbAnim);
    }

    private IEnumerator DelayClimb()
    {
        yield return new WaitForSeconds(.215f);
        StopClimb();
    }

    public void Respawn()
    {
        rigitbody.bodyType = RigidbodyType2D.Dynamic;
        transform.position = I.levelManager.positionRespawn;
        rigitbody.velocity = Vector2.zero;
        _directionX = 0;
        _death = false;
    }

    public void Death()
    {
        I.camera.StartCameraShake();
        rigitbody.velocity = Vector2.zero;
        _death = true;
        _directionX = 0;
        SYIL.Death(transform.position);

        if (transform.localScale.x != I.levelManager.direction)
        {
            Flip();
        }
        if (_stateCharacter == StateCharacter.Climb)
        {
            StopClimb();
            StopCoroutine(climbCoroutine);
        }
        if (wallCheckRevers)
            WallCheckFlip();
        StateAnimation(false, false, false, false);
        _stateCharacter = StateCharacter.Idle;
        _jump = false;
        rigitbody.bodyType = RigidbodyType2D.Static;
        transform.position = Vector3.right * 228;
    }
}

public enum StateCharacter
{
    Idle,
    Run,
    Jump,
    Fall,
    WallSlide,
    Climb,
    Deat,
    None
}