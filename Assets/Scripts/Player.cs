using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using QFSW.QC;
using UnityEngine.InputSystem;

namespace Kyota
{
    public class Player : MonoBehaviour
    { 
        [Header("Horizontal Movement")]
        [Command] [SerializeField] private float runMoveSpeed = 5f;
        [Command] [SerializeField] private float xDeceleration = 1f;
        [SerializeField] private float inputWaitTime = 0.5f;
        [SerializeField] private float gravity = 3f;
        [SerializeField] private float minYVelocity = -7f;
        [SerializeField] public bool gravityReversed = false;

        [Header("Jump")]
        [Command] [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpRemember = 0.15f;
        [SerializeField] private float groundedRemember = 0.1f;
        [Command] [SerializeField] private float topJumpThreshold = 0.3f;
        [Command] [SerializeField] private float topJumpMultiplier = 0.1f;
        [Command] [SerializeField] private float fallMultiplier = 1.1f;
        [Command] [SerializeField] private float lowJumpMultiplier = 7;
        [SerializeField] public int powerJumpCap = 3;
        [SerializeField] private float jumpedRemember = 0.1f;

        [Header("Dash")]
        [Command] [SerializeField] private float dashSpeed = 5f;
        [Command] [SerializeField] private float dashTime = 0.6f;

        [Header("Ground")]
        [SerializeField] private LayerMask groundLayer = default;
        [SerializeField] private LayerMask wallSlideLayer = default;
        [SerializeField] private LayerMask jumpRingLayer = default;
        [SerializeField] private Transform groundCheck = default;
        [SerializeField] private Transform jumpRingCheck = default;
        
        [Header("Wall Movement")]
        [SerializeField] private float wallCheckDistance = 0.14f;
        [SerializeField] private float wallSlideSpeed = 0.3f;
        [Command] [SerializeField] private Vector2 wallJumpForce = new Vector2(5f, 4.5f);
        [SerializeField] private Vector2 wallJumpQuickForce = new Vector2(3f, 5.5f);
        [SerializeField] private float wallJumpTime = 0.1f;
        [SerializeField] private float wallSlideRemember = 0.3f;

        [Header("Sliding")]
        [Command] [SerializeField] private float downSlopeXVelocity = 8.8f;
        [Command] [SerializeField] private float downSlopeXAcceleration = 1f;
        [Command] [SerializeField] private float upSlopeXVelocity = 2f;
        [Command] [SerializeField] private float upSlopeXDeceleration = 0.3f;
        [SerializeField] private float slidingGravity = 10f;
        [SerializeField] private Transform headCheck = default;

        [Header("Shooting")]
        [Command] [SerializeField] private float bulletForce = 18f;
        [SerializeField] private float shootCooldown = 0.2f;
        [SerializeField] private Transform firePoint = default;
        [SerializeField] private Rigidbody2D bulletPrefab = default;

        [Header("References")]
        [SerializeField] private TrailRenderer trail = default;
        [SerializeField] private GameObject powerJumpParticle = default;
        [SerializeField] private Animator powerMeterAnimator = default;
        [SerializeField] public TMP_InputField qcInputField = default;
        [SerializeField] private TrailRenderer dashTrail = default;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip normalJumpSound = default;
        [SerializeField] private AudioClip powerJumpSound = default;
        [SerializeField] private AudioClip slideActivate = default;
        [SerializeField] private AudioClip levelClearSound = default;
        [SerializeField] private AudioClip startGameSound = default;
        [SerializeField] private AudioClip[] gunSounds = default;
        [SerializeField] private AudioClip[] footstepSounds = default;
        [SerializeField] private AudioClip[] powerMeterSounds = default;
        
        [Header("Audio Volumes")]
        [SerializeField] private float footstepVolume = 1f;
        [SerializeField] private float gunVolume = 1f;
        [SerializeField] private float normalJumpVolume = 1f;
        [SerializeField] private float powerJumpVolume = 1f;
        [SerializeField] private float slideActiveVolume = 1f;
        [SerializeField] private float powerMeterVolume = 1f;
        [SerializeField] private float levelClearVolume = 1f;
        [SerializeField] private float startGameVolume = 1f;

        // Input
        public static PlayerInputs playerInputs;
        private bool slideInput = false;
        private bool jumpInput = false;
        private bool acceptInput = false;
        
        // References
        private Camera mainCamera;
        public Rigidbody2D rb;
        private Animator animator;
        private AudioSource soundPlayer;
        private SpriteRenderer spriteRenderer;
        
        // Timers
        private float jumpRememberLeft;
        private float jumpedRememberLeft;
        private float groundedRememberLeft;
        private float wallSlideRememberLeft;
        private float shootCooldownLeft;
        
        // Movement
        private bool facingRight = true;
        private bool isGrounded = false;
        private bool sliding = false;
        private bool wallSliding = false;
        private bool wallJumping = false;
        private bool wallJumpGravityEnabled = false;
        private bool dashing = false;
        [HideInInspector] public bool ringJumping = false;
        
        private Vector2 wallJumpDirection;
        private Vector2 slideJumpDirection;

        public int powerJumpsLeft = 0;
        private float momentumMultiplier;
        private float dashDirection;
        [Command] public float moveSpeed;
        [SerializeField] private float gravityScale;

        public bool Dashing
        {
            get => dashing;
            set
            {
                dashTrail.emitting = value;
                dashing = value;
                if (value) StartCoroutine(DisableDashing());
            }
        }
        
        public bool Sliding => sliding;

        public static Player instance;

        private float initialTime;


        private void Awake()
        {
            instance = this;
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            soundPlayer = GetComponent<AudioSource>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            if (playerInputs == null) playerInputs = new PlayerInputs();
            playerInputs.Land.Slide.started += _ => slideInput = true;
            playerInputs.Land.Slide.canceled += _ => slideInput = false;
            playerInputs.Land.Jump.started += _ => jumpInput = true;
            playerInputs.Land.Jump.canceled += _ => jumpInput = false;

            var checkpoints = Checkpoint.claimedCheckpoints;
            if (checkpoints.Count > 0)
            {
                Checkpoint checkpoint = GameObject.Find(checkpoints[checkpoints.Count - 1]).GetComponent<Checkpoint>();
                transform.position = checkpoint.newSpawnPos.position;
                
                facingRight = Mathf.Sign(checkpoint.newSpawnPos.localScale.x) == 1;
                UpsideDownManager.instance.SwitchSide(!checkpoint.upsideDown);
                UpdateFacingDirection();
            }
            else
            {
                UpsideDownManager.instance.SwitchSide(true);
                SpeedrunTimer.currentTime = 0;
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
            Invoke(nameof(EnableInput), inputWaitTime);
            
            if (SceneManager.GetActiveScene().buildIndex > 1)
            {
                soundPlayer.PlayOneShot(levelClearSound, levelClearVolume);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                soundPlayer.PlayOneShot(startGameSound, startGameVolume);
            }

            moveSpeed = runMoveSpeed;
        }

        private void Update()
        {
            if (!acceptInput) return;
            if (qcInputField.isFocused) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                Die();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                Checkpoint.claimedCheckpoints.Clear();
                Die();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Canvas speedrunTimer = FindObjectOfType<SpeedrunTimer>().GetComponent<Canvas>();
                speedrunTimer.enabled = !speedrunTimer.enabled;
            }
            
            // Timer Handling
            jumpRememberLeft -= Time.deltaTime;
            jumpedRememberLeft -= Time.deltaTime;
            groundedRememberLeft -= Time.deltaTime;
            wallSlideRememberLeft -= Time.deltaTime;
            shootCooldownLeft -= Time.deltaTime;

            // Aiming
            Vector2 lookDir = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            Vector2 gamepadInput = playerInputs.Land.GunRotation.ReadValue<Vector2>();
            if (gamepadInput == Vector2.zero) gamepadInput = Mathf.Sign(transform.localScale.x) == 1 ? Vector2.right : Vector2.left;
            if (Gamepad.all.Count > 0) lookDir = gamepadInput;
            lookDir.Normalize();
            firePoint.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f);

            // Grounded and sprite rotation
            Collider2D groundCollider = Physics2D.OverlapBox(groundCheck.position, 
                groundCheck.localScale, 0, groundLayer);
            isGrounded = groundCollider != null;
            if (isGrounded)
            {
                groundedRememberLeft = groundedRemember;

                float angle = groundCollider.transform.rotation.eulerAngles.z;
                if ((angle - 90) % 180 != 0) spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
                
                if (groundCollider.TryGetComponent(out GravityPlatform gravityPlatform))
                {
                    gravityPlatform.FallEveryPlatform();
                }
            }
            else spriteRenderer.transform.rotation = Quaternion.identity;

            // Dashing Input
            /*if (Input.GetKeyDown(KeyCode.Mouse1) && !Dashing)
            {
                moveSpeed = dashSpeed;
                Dashing = true;
            }*/
            
            // Jump Input
            bool onJumpPad = Physics2D.OverlapBox(jumpRingCheck.position, jumpRingCheck.localScale, 0, jumpRingLayer);
            if (playerInputs.Land.Jump.triggered && !onJumpPad)
            {
                jumpRememberLeft = jumpRemember;
            }

            // Slide Input
            bool wasSliding = sliding;
            bool bangHead = Physics2D.OverlapBox(headCheck.position, headCheck.localScale, 0, groundLayer);
            sliding = slideInput && isGrounded && rb.velocity.x != 0 && !Dashing;
            if (bangHead && wasSliding) sliding = true;
            if (sliding)
            {
                if (!wasSliding)
                {
                    soundPlayer.pitch = 1f;
                    soundPlayer.PlayOneShot(slideActivate, slideActiveVolume);
                    soundPlayer.Play();
                    momentumMultiplier = rb.velocity.x / runMoveSpeed;
                }
            }
            else if (wasSliding) soundPlayer.Stop();
            if (wallSliding)
            {
                if (!spriteRenderer.flipX) spriteRenderer.flipX = true;
            }
            else
            {
                if (spriteRenderer.flipX) spriteRenderer.flipX = false;
            }

            // Shoot Input
            if (playerInputs.Land.Shoot.triggered)
            {
                Shoot();
            }

            // Animator
            powerMeterAnimator.SetInteger("powerJumpsLeft", powerJumpsLeft);

            transform.localScale = new Vector3(transform.localScale.x, gravityReversed ? -1 : 1, 1);
        }

        private void Shoot()
        {
            if (shootCooldownLeft > 0) return;
            shootCooldownLeft = shootCooldown;
            
            Rigidbody2D bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
            
            Destroy(bullet.gameObject, 5f);
            
            soundPlayer.pitch = 1f;
            soundPlayer.PlayOneShot(gunSounds[Random.Range(0, gunSounds.Length)], gunVolume);
        }

        public void PlayPowerMeterSound(int index)
        {
            soundPlayer.PlayOneShot(powerMeterSounds[index], powerMeterVolume);
        }

        private void FixedUpdate()
        { 
            if (!acceptInput) return;
            
            // Quantum Console focused
            if (qcInputField.isFocused)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            // Grounded
            isGrounded = Physics2D.OverlapBox(groundCheck.position, 
                groundCheck.localScale, 0, groundLayer);
            
            animator.SetBool("sliding", sliding);
            
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * (gravityReversed ? -1 : 1));
            
            // Horizontal Movement
            if (!wallJumping && !sliding && !Dashing)
            {
                if (moveSpeed > runMoveSpeed)
                {
                    float localXDeceleration = xDeceleration * (moveSpeed - runMoveSpeed) * 0.5f;
                    moveSpeed = Mathf.Max(moveSpeed - localXDeceleration * Time.deltaTime, runMoveSpeed);
                }
                else moveSpeed = runMoveSpeed;
                
                rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y);
                animator.SetBool("running", rb.velocity.x != 0);
            }

            // Jump
            if (jumpRememberLeft > 0 && (groundedRememberLeft > 0 || wallSlideRememberLeft > 0 || powerJumpsLeft > 0))
            {
                ringJumping = false;
                soundPlayer.pitch = Random.Range(0.9f, 1.1f);
                if (wallSlideRememberLeft > 0) // Wall jump
                {
                    wallJumping = true;
                    float xSign = facingRight == wallSliding ? -1f : 1f;
                    wallSlideRememberLeft = 0f;
                    StartCoroutine(DisableWallJumping());
                    soundPlayer.PlayOneShot(normalJumpSound, normalJumpVolume);
                    if (wallSliding && facingRight == (Input.GetAxisRaw("Horizontal") == 1f))
                    {
                        wallJumpGravityEnabled = true;
                        StartCoroutine(DisableWallJumpGravity());
                        rb.velocity = new Vector2(wallJumpQuickForce.x * xSign, wallJumpQuickForce.y);
                    }
                    else rb.velocity = new Vector2(wallJumpForce.x * xSign, wallJumpForce.y);
                }
                else
                {
                    jumpedRememberLeft = jumpedRemember;
                    if (groundedRememberLeft <= 0) // Power Jump
                    {
                        powerJumpsLeft--;
                        Instantiate(powerJumpParticle, trail.transform.position, Quaternion.identity);
                        soundPlayer.PlayOneShot(powerJumpSound, powerJumpVolume);
                    }
                    else // Normal Jump
                    {
                        soundPlayer.PlayOneShot(normalJumpSound, normalJumpVolume);
                    }
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }

                jumpRememberLeft = 0f;
                groundedRememberLeft = 0f;
                sliding = false;
                Dashing = false; // ?
            }
            
            if (sliding)
            {
                float angle = Mathf.Abs(spriteRenderer.transform.rotation.eulerAngles.z);
                if (angle > 180) angle = Mathf.Abs(360 - angle);
                if (angle != 0)
                {
                    if (!gravityReversed && rb.velocity.y < 0 || gravityReversed && rb.velocity.y > 0)
                    {
                        moveSpeed = Mathf.Min(moveSpeed + downSlopeXAcceleration * Time.deltaTime, downSlopeXVelocity);
                    }
                    else
                    {
                        moveSpeed = Mathf.Min(moveSpeed - upSlopeXDeceleration * Time.deltaTime, upSlopeXVelocity);
                    }
                }
                
                float moveInput = Input.GetAxisRaw("Horizontal");
                float direction = moveInput == 0 ? Mathf.Sign(momentumMultiplier) : moveInput;
                
                rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
                gravityScale = slidingGravity;
            }
            else if (Dashing)
            {
                rb.velocity = new Vector2(dashSpeed * dashDirection, 0);
                gravityScale = 0;
            }
 
            if (ringJumping)
            {
                gravityScale = 3;
            }

            if (!sliding)
            {
                if (!isGrounded)
                {
                    if (Mathf.Abs(rb.velocity.y) <= topJumpThreshold && jumpedRememberLeft <= 0 && !ringJumping)
                    {
                        gravityScale = topJumpMultiplier;
                    }
                    else if (rb.velocity.y < 0)
                    {
                        gravityScale = fallMultiplier;
                    }
                    else if (rb.velocity.y > 0 && !jumpInput)
                    {
                        gravityScale = !ringJumping ? lowJumpMultiplier : 1.5f;
                        if (wallJumpGravityEnabled) gravityScale = 1.5f;
                    }
                    else gravityScale = 1f;
                }
                else gravityScale = 0.5f;
            }

            if (rb.velocity.x > 0 && !facingRight || rb.velocity.x < 0 && facingRight)
            {
                facingRight = rb.velocity.x > 0;
                UpdateFacingDirection();
            }
            
            bool wallHit = Physics2D.Raycast(transform.position, 
                facingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallSlideLayer);
            bool noMoveInput = Input.GetAxisRaw("Horizontal") == 0;
            if (wallHit && !isGrounded && !noMoveInput)
            {
                wallSliding = true;
                wallSlideRememberLeft = wallSlideRemember;
            }
            else wallSliding = false;
            Debug.DrawRay(transform.position, Vector3.right * (facingRight ? wallCheckDistance : -wallCheckDistance), Color.red);

            if (wallSliding)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideSpeed, float.MaxValue));
            }

            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y + -gravity * gravityScale * Time.deltaTime, minYVelocity));
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + -gravity * gravityScale * Time.deltaTime);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * (gravityReversed ? -1 : 1));

            animator.SetBool("wallSliding", wallSliding);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("yVelocity", rb.velocity.y);
        }

        private void UpdateFacingDirection()
        {
            float newXScaleSign = facingRight ? 1f : -1f;
            Vector3 newScale = transform.localScale;
            
            newScale.x = Mathf.Abs(newScale.x) * newXScaleSign;
            transform.localScale = newScale;
        }

        private IEnumerator DisableWallJumping()
        {
            yield return new WaitForSeconds(wallJumpTime);
            wallJumping = false;
        }

        private IEnumerator DisableWallJumpGravity()
        {
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => isGrounded || !wallJumpGravityEnabled || wallSliding);
            wallJumpGravityEnabled = false;
        }

        public void SetDashing(bool value, bool inverted, bool rightSide)
        {
            Dashing = value;
            dashDirection = (rightSide ? -1 : 1) * (inverted ? -1 : 1);
        }

        private IEnumerator DisableDashing()
        {
            yield return new WaitForSeconds(dashTime);
            Dashing = false;
        }

        public IEnumerator RingJumpingTimer(Vector2 force)
        {
            rb.velocity = new Vector2(rb.velocity.x, force.y);
            ringJumping = true;
            yield return new WaitUntil(() => isGrounded || !ringJumping);
            ringJumping = false;
        }

        private void EnableInput()
        {
            acceptInput = true;
        }

        public void OnTeleportPowerupShot(Vector2 newPos)
        {
            transform.position = newPos;
            trail.Clear();
            groundedRememberLeft = 0f;
            //rb.velocity = Vector2.zero;
            // Make player jump again according to jumpRememberLeft (input) maybe?
            if (jumpedRememberLeft > 0)
            {
                powerJumpsLeft--;
                Instantiate(powerJumpParticle, trail.transform.position, Quaternion.identity);
                soundPlayer.PlayOneShot(powerJumpSound, powerJumpVolume);
            }
        }

        public void Die()
        {
            SceneLoader.ReloadCurrentScene();
        }

        private void PlayFootstep()
        {
            soundPlayer.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)], footstepVolume);
        }

        private void OnEnable()
        {
            playerInputs.Enable();
        }

        private void OnDisable()
        {
            playerInputs.Disable();
        }
    }
}
