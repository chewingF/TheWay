using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Chronos;
using cakeslice;
using UnityEngine.PostProcessing;

[RequireComponent(typeof(TransformPathMaker))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

[ExecuteInEditMode]
public class PlayerBehaviour : BaseBehaviour
{

    [HideInInspector]
    public Transform RightHand, aimHelper, aimHelperSpine;

    public enum Direction
    {
        Forward,
        Back,
        Up,
        Down,
        Left,
        Right
    }

    [HideInInspector]
    public Animator playerAnimator;

    [HideInInspector]
    public RagdollHelper ragdollh;

    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public CapsuleCollider capsuleC;

    [HideInInspector]
    public IKControl ikControll;

    [HideInInspector]
    public TransformPathMaker pathMaker;

    [HideInInspector]
    public Controller controller;

    [HideInInspector]
    public CameraBehaviour cameraBehaviour;

    [HideInInspector]
    public Transform transformToRotate;

    [HideInInspector]
    public Vector3 moveAxis;

    [HideInInspector]
    public Rigidbody[] boneRb;

    [HideInInspector]
    public Transform hipsParent;

    private PhysicMaterial pM;

    //Camera
    [HideInInspector]
    public Transform cameraParent, cam;

    [HideInInspector]
    public Transform[] camPivot = new Transform[2];

    //public UI_Manager keybindingMenu;
    //public GameObject menu;


    [Header("Player Settings")]
    [Range(0, 100)]
    public float maxLife = 100;
    public float life = 100;

    [Range(0, 200)]
    public float maxEnergy = 200;
    public float energy = 200;

    [Range(0, 200)]
    public float energyRegenerateRate = 1;
    private bool regening = false;
    [SerializeField]
    public float crouchSpeed = 1f, walkSpeed = 2.3f, runSpeed = 4.6f, aimWalkSpeed = 1.5f;
    public bool dead;
    [HideInInspector]
    public bool crouch;
    [HideInInspector]
    public bool aim;
    public float jumpForce = 15;
    public int bagLimit = 5;
    public float switchWeaponTime = .5f;
    public bool ragdollWhenFall = true;
    private float characterHeight = 1;
    //Aubrey
    public bool invinsible;
    [Range(0, 2)]
    public float crouchHeight = 0.75f;
    [Range(-1, 1)]
    public float bellyOffset = 0;

    [Header("Change this if holding weapons looks weird")]
    public Direction spineFacingDirection;

    [Header("Audio")]
    public AudioClip footStepAudio;

    //[HideInInspector]
    public AudioSource audioSource;

    //FOOTSTEP STUFF
    [HideInInspector]
    public Transform leftFoot, rightFoot;
    private bool leftCanStep, rightCanStep;
    [HideInInspector]
    public float factor;

    //WEAPON STUFF
    public event Action OnWeaponSwitch;
    private bool equippedbefore;
    private float climbY;
    private float xAxis, yAxis;
    [HideInInspector]
    public Quaternion rotationAux, aimRotationSpineAux, aimRotationAux;

    private float lean;
    [HideInInspector]
    public float recoil;
    private float _capsuleSize;
    private float currentMovementState;
    private float runKeyPressed;
    private AnimatorStateInfo currentAnimatorState;
    [HideInInspector]
    //stopKeyInput by Aubrey
    public bool grounded, inMoveState, climbing, climbHit, switchingWeapons, halfSwitchingWeapons, stopKeyInput;
    private GameObject collidedWith;

    [HideInInspector]
    public List<WeaponBase> weapons = new List<WeaponBase>();

    [HideInInspector]
    public WeaponBase currentWeapon;

    [HideInInspector]
    public int currentWeaponID;

    [HideInInspector]
    public bool equippedWeapon;

    [HideInInspector]
    public Transform leftHandInWeapon, rightHandInWeapon;

    Quaternion startSpineRot = new Quaternion(0, 0, 0, 1);

    private bool useVCR;
    private InputVCR vcr;

    // Other
    [Header("Other - Checking Stuff")]
    public bool isActive = true;
    public bool isClone = false;


    [Header("Harry wall walker try")]
    public float jumpRange = 1;  // range to detect target wall
    public Vector3 surfaceNormal; // current surface normal
    public Vector3 myNormal; // character normal
    private bool jumpingToWall = false; // flag &quot;I'm jumpingToWall to wall&quot;
    private float distGround; // distance from character position to ground
    //private float deltaGround = 0.2f; // character is grounded up to this distance
    //private float vertSpeed = 0; // vertical jump current speed
    private float lerpSpeed = 10; // smoothing speed
    private float moveSpeed = 0; // move speed
    public float gravity = 10; // gravity acceleration	
    //private float turnSpeed = 90; // turning speed (degrees/second)
    //private GameObject animator;
    public bool wallWalkerAbility = false;
    private bool justJumpToWall;
    private bool inTrigger;
    public Text wallWalkerAllowedText;
    private RaycastHit inFrontHit;
    //private bool jumping = false;
    public Transform myTransform;
	public bool WallWalkerAllowed;

    [Header("Layers to Ignore")]
    public LayerMask triggerLayerMask;

    // Health & Energy Change Numbers
    [Header("Health & Energy Change Numbers")]
    public GameObject healthChangeNumbers;
    public GameObject energyChangeNumbers;
    private GameObject newHealthNumbers;

    // Stuff for Abilities
    private GameObject gameManager;
    private GameObject ability1, ability2, ability3, ability4;

    [HideInInspector]
    public bool ability1MenuOpen = false, ability2MenuOpen = false, ability3MenuOpen = false, ability4MenuOpen = false;
    private bool closeAbility1Menu = false;

    private List<GameObject> ability1Slots = new List<GameObject>();
    //private List<GameObject> ability2Slots = new List<GameObject>();
    //private List<GameObject> ability3Slots = new List<GameObject>();
    //private List<GameObject> ability4Slots = new List<GameObject>();

    [HideInInspector]
    public GameObject abilityItemPrefab;

    private float ability1HoldTimer;
    private bool aimToggleEffect = false;

    [HideInInspector]
    public bool subtitlesActive = false;
    [HideInInspector]
    public bool interactPressed = false;



    public void Awake()
    {
        Transform root = transform;
        while (root.parent != null)
            root = root.parent;
        vcr = root.GetComponent<InputVCR>();
        useVCR = vcr != null;
    }


    public void Start()
    {
        if (Application.isPlaying)
        {
            if (!isClone)
            {
                //By Aubrey
                stopKeyInput = false;
                maxEnergy = energy;

                factor = 0.45f;
                startSpineRot = aimHelperSpine.rotation;
                Cursor.lockState = CursorLockMode.Locked;
                rotationAux = new Quaternion(0, 0, 0, 1);
                aimRotationAux = rotationAux;
                aimRotationSpineAux = rotationAux;
                life = maxLife;
                energy = maxEnergy;
                dead = false;
                crouch = false;
                halfSwitchingWeapons = true;
            }

            //By Aubrey, put melee weapon as a child of right hand instead of aim helper to allow melee combat animation
            RightHand = playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);

            foreach (Rigidbody r in boneRb)
            {
                BoxCollider bc = r.GetComponent<BoxCollider>();
                SphereCollider[] sc = r.GetComponents<SphereCollider>();
                if (bc != null)
                {
                    Physics.IgnoreCollision(capsuleC, bc);
                }
                if (sc != null)
                {
                    foreach (SphereCollider s in sc)
                    {
                        Physics.IgnoreCollision(capsuleC, s);
                    }
                }
            }

            pM = capsuleC.material;

            //from HARRY
            //animator = GameObject.Find("Player/Animator");
            myNormal = transform.up; // normal starts as character up direction
            GetComponent<Rigidbody>().freezeRotation = true; // disable physics rotation
            distGround = capsuleC.height / 2 - capsuleC.center.y;// distance from transform.position to ground

            try
            {
                wallWalkerAllowedText = GameObject.Find("walker_a_active").GetComponent<Text>();
            }
            catch (NullReferenceException)
            {

            }
            // Harry define myTransform
            myTransform = transform;

            // By Nathan, for ability stuff
            if (GameObject.Find("GameManager"))
                gameManager = GameObject.Find("GameManager");
            else
                Debug.Log("Game Manager not found!");

            ability1 = gameManager.transform.FindDeepChild("Ability1").gameObject;
            foreach (Transform item in ability1.transform.FindDeepChild("RadialMenu"))
            {
                ability1Slots.Add(item.gameObject);
            }
        }
    }
    void FootStepAudio()
    {
        if (!grounded) { return; }
        if (climbing) { return; }
        float dist = Vector3.Distance(leftFoot.position, rightFoot.position);
        if (dist > factor)
        {
            leftCanStep = true;
        }
        if (leftCanStep && dist < factor)
        {
            leftCanStep = false;

            audioSource.PlayOneShot(footStepAudio);
        }

    }

    void Update()
    {
        if (!Application.isPlaying) { return; }

        if (Input.GetMouseButtonDown(0) && gameObject.name == "Player" && !subtitlesActive)
        {
            if (!GameObject.Find("GameManager").GetComponent<PauseMenu>().pauseMenuActive)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // if (this.gameObject.name == "Player") // By Nathan, fixes clone error not having objects attached
        // {
        //     if (!Application.isPlaying||menu.activeSelf) { return; } // By Harry, stopping input when menu is up

        //     if (Input.GetMouseButtonDown(0) && keybindingMenu.isActive == false)
        //     {
        //         Cursor.lockState = CursorLockMode.Locked;
        //     }
        // }

        PlayerMovement();
        GroundCheck();
        Gravity();
        FootStepAudio();
        AbilitiesCheck();
        //Debug.Log("state: " + playerAnimator.state

        if (vcr.GetButtonDown("Interact") && isClone)
            interactPressed = true;
        else
            interactPressed = false;

        if (this.gameObject.name == "Player" && hardInput.GetKeyDown("ToggleEffects"))
        {
            this.gameObject.transform.FindDeepChild("Main Camera").GetComponent<PostProcessingBehaviour>().enabled = !this.gameObject.transform.FindDeepChild("Main Camera").GetComponent<PostProcessingBehaviour>().enabled;
        }
    }

    void AbilitiesCheck() // By Nathan
    {
        // ==================================== Ability 1 Stuff (Weapons) ====================================
        #region Ability 1
        // Updateing Ring Items
        if (((ability1Slots.Count == 0 && weapons.Count > 0) || ability1MenuOpen) && !isClone)
        {
            float seperationTemp;

            // Dynamicaly Adds and arranges the weapon Items based on the current weapon count
            if (ability1Slots.Count < weapons.Count)
            {
                for (int i = ability1Slots.Count; i < weapons.Count; i++)
                {
                    ability1Slots.Add(Instantiate(abilityItemPrefab));
                    ability1Slots[i].transform.FindDeepChild("Text").GetComponent<Text>().text = (i + 1).ToString();
                    ability1Slots[i].transform.SetParent(ability1.transform.FindDeepChild("RadialMenu"), false);
                    if (ability1MenuOpen)
                    {
                        ability1Slots[i].GetComponent<Animate>().DoAnimation();
                    }
                }

                seperationTemp = 360 / ability1Slots.Count;
                float currentTemp = 0;

                // Evenly Seperates Items 
                for (int i = 0; i < ability1Slots.Count; i++)
                {
                    ability1Slots[i].gameObject.transform.rectTransform().localEulerAngles = new Vector3(0, 0, currentTemp);
                    currentTemp -= seperationTemp;
                }
            }

            // Rotates the items so the selected one is visable
            seperationTemp = 360 / ability1Slots.Count;
            float ringNewRot = seperationTemp * (currentWeaponID);
            float ringRotation = Mathf.Lerp(ability1.transform.FindDeepChild("RadialMenu_Blur").transform.localEulerAngles.z, ringNewRot, time.deltaTime * 5);
            ability1.transform.FindDeepChild("RadialMenu_Blur").transform.localEulerAngles = new Vector3(0, 0, ringRotation);
        }

        // Closes the radial menu when mouse buttons are clicked
        if (((vcr.GetButtonDown("Aim") || vcr.GetButtonDown("Shoot")) && ability1MenuOpen) && !isClone)
        {
            closeAbility1Menu = true;
            ability1HoldTimer = 0.5f;
        }

        // Resets hold timer
        if (vcr.GetButtonDown("Num1") && !ability1MenuOpen)
        {
            ability1HoldTimer = 0;
        }
        // Opens the radial menu
        if (vcr.GetButton("Num1") || closeAbility1Menu)
        {
            if (ability1Slots.Count > 0)
            {
                ability1HoldTimer += 1.0f * time.deltaTime;
                //Debug.Log(ability1HoldTimer);

                if (ability1HoldTimer > 0.3f && ability1HoldTimer < 1f)
                {
                    if (!isClone)
                    {
                        ability1.transform.Find("AbilityHolder_Blur").gameObject.GetComponent<Animate>().DoAnimation();
                        ability1.transform.FindDeepChild("RadialMenu_Blur").gameObject.GetComponent<Animate>().DoAnimation();
                        for (int i = 0; i < ability1Slots.Count; i++)
                        {
                            ability1Slots[i].GetComponent<Animate>().DoAnimation();
                        }
                    }
                    ability1HoldTimer = 2;
                    ability1MenuOpen = !ability1MenuOpen;
                    closeAbility1Menu = false;
                }
            }
        }
        // Toggles weapon active or not
        if (vcr.GetButtonUp("Num1") && ability1HoldTimer > 0.01f)
        {
            if (!ability1MenuOpen && ability1HoldTimer < 2 && weapons.Count > 0)
            {
                if (!switchingWeapons)
                {
                    EquipWeaponToggle();
                }
            }
        }
        #endregion
        // =============================== Ability 2 Stuff (Time Manipulation) ===============================
        #region Ability 2

        #endregion
        // ==================================== Ability 3 Stuff (Movement) ===================================
        #region Ability 3

        #endregion
        // ====================================== Ability 4 Stuff (???) ======================================
        #region Ability 4

        #endregion


        // Wall Walker Ability
        if (vcr.GetButtonDown("WallWalkerAbility"))
        {
			if (!jumpingToWall && WallWalkerAllowed)
            {
                wallWalkerAbility = !wallWalkerAbility;
                //Vector3 worldNormal = new Vector3 (0, 1, 0);
                //if (myNormal != worldNormal){
                DropDownCheck();
            }
        }
    }

    void DropDownCheck()
    {
        if (wallWalkerAbility)
        {
            return;
        }
        Vector3 worldNormal = new Vector3(0, 1, 0);
        if (worldNormal != myNormal)
        {
            if (time.timeScale > 0)
            {
                time.rigidbody.isKinematic = true; // disable physics while jumpingToWall
            }
            Vector3 orgPos = myTransform.position;
            Quaternion orgRot = myTransform.rotation;
            Quaternion orgRotA = transformToRotate.rotation;
            Vector3 dstPos = orgPos + myNormal * capsuleC.height; // will jump to 0.5 above wall

            //Vector3 posToDetect = transformToRotate.position + transformToRotate.up * capsuleC.height / 2;
            //modeified by harry to find out infornt hit

			Ray ray = new Ray(dstPos, -worldNormal);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, capsuleC.height, triggerLayerMask))
			{

				dstPos.y = hit.point.y;
			}
			else
			{

				dstPos.y -= capsuleC.height;

			}


            Vector3 myForward = Vector3.Cross(myTransform.right, worldNormal);
            Vector3 myForwardA = Vector3.Cross(transformToRotate.right, worldNormal);
            Quaternion dstRot = Quaternion.LookRotation(myForward, worldNormal);
            Quaternion dstRotA = Quaternion.LookRotation(myForwardA, worldNormal);
            StartCoroutine(jumpTime(orgPos, orgRot, orgRotA, dstPos, dstRot, dstRotA, worldNormal));
        }
    }


	public void SetHaveWallWalker(bool answer)
	{
		WallWalkerAllowed = answer;
	}

    //Body Aim
    void LateUpdate()
    {
        if (!Application.isPlaying) { return; }

        // if (this.gameObject.name == "Player") // By Nathan, fixes clone error not having objects attached
        // {
        //     if (!Application.isPlaying||menu.activeSelf) { return; } // By Harry, stopping input when menu is up
        // }

        //By Aubrey
        if (equippedWeapon)
        {
            if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
            {
                if (!playerAnimator.GetBool("Slash1") && !playerAnimator.GetBool("Slash2"))
                //&& !playerAnimator.GetBool("Slash3"))
                {
                    currentWeapon.transform.FindDeepChild("Edge").GetComponent<MeleeWeaponEdge>().CoroutineSetSwingfalseAfter(0f);
                }
                else
                {
                    currentWeapon.SetWeaponSwinging(true);
                }
            }
        }

        recoil = Mathf.Lerp(recoil, 0, 10 * time.deltaTime);
        if (dead) { return; }


        Vector3 spineOffset = cam.forward - cam.up / 5;

        Vector3 armsOffset = cam.forward;

        if (aim)
        {
            if (!aimToggleEffect && !isClone)
            {
                gameManager.transform.FindDeepChild("AimEffect").GetComponent<Animate>().DoAnimation();
                aimToggleEffect = true;
            }
            if (!cameraBehaviour.aimIsRightSide)
            {
                armsOffset -= cam.right / 3;
            }
            spineOffset.y = Mathf.Clamp(spineOffset.y, -0.15f, 0.15f);

            if (SomethingInFront())
            {
                spineOffset.y = Mathf.Clamp(spineOffset.y, 0, 0f);
                armsOffset.y = Mathf.Clamp(armsOffset.y, 0, 1);
            }
            if (SomethingInFrontAim(2) && currentWeapon && !currentWeapon.reloadProgress)
            {
                if (cameraBehaviour.aimIsRightSide)
                {
                    lean = -.2f;
                }
                else
                {
                    lean = .2f;
                }
            }
            else
            {
                lean = 0;
            }
            if (crouch)
            {
                armsOffset.y = Mathf.Clamp(armsOffset.y, -.7f, 0.4f);
                if (!cameraBehaviour.aimIsRightSide)
                {
                    spineOffset -= cam.right * .3f;
                }
                else
                {
                    spineOffset += cam.right * .3f;
                }
                if (!SomethingInFront())
                {
                    spineOffset.y = Mathf.Clamp(spineOffset.y, -.5f, -.5f);
                }
            }

            aimRotationAux = Quaternion.Lerp(aimRotationAux, Quaternion.LookRotation((transformToRotate.position + armsOffset + cam.up * recoil / 10) - transformToRotate.position), 10 * time.deltaTime);
            aimRotationSpineAux = Quaternion.Lerp(aimRotationSpineAux, Quaternion.LookRotation((transformToRotate.position + spineOffset + cam.up * recoil / 5) - transformToRotate.position) * new Quaternion(0, 0, lean, 1) * startSpineRot, 10 * time.deltaTime);
        }
        else
        {
            if (aimToggleEffect && !isClone)
            {
                gameManager.transform.FindDeepChild("AimEffect").GetComponent<Animate>().DoAnimation();
                aimToggleEffect = false;
            }
            lean = 1;

            aimRotationSpineAux = Quaternion.Lerp(aimRotationSpineAux, aimHelperSpine.rotation, 20 * time.deltaTime);

            Vector3 _off = Vector3.zero;

            if (spineFacingDirection == Direction.Forward)
            {
                _off = aimHelperSpine.forward;
            }
            else if (spineFacingDirection == Direction.Back)
            {
                _off = -aimHelperSpine.forward;
            }
            else if (spineFacingDirection == Direction.Up)
            {
                _off = aimHelperSpine.up;
            }
            else if (spineFacingDirection == Direction.Down)
            {
                _off = -aimHelperSpine.up;
            }
            else if (spineFacingDirection == Direction.Left)
            {
                _off = -aimHelperSpine.right;
            }
            else if (spineFacingDirection == Direction.Right)
            {
                _off = aimHelperSpine.right;
            }
            _off.y = Mathf.Clamp(_off.y, 0, 5);
            if (crouch)
            {
                _off -= transformToRotate.right * 0.3f;
            }
            aimRotationAux = Quaternion.Lerp(aimRotationAux, Quaternion.LookRotation((aimHelper.position + _off) - aimHelper.position), 10 * time.deltaTime);
        }
        if (!playerAnimator.enabled) { return; }

        aimHelperSpine.rotation = aimRotationSpineAux;

        //By Aubrey, do something while melee combat
        //if (this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slash1"))
        //{
        //    //stopKeyInput = true;
        //}
        //else
        //{
        //    //stopKeyInput = false;
        //}

        if (energy < maxEnergy)
        {
            if (!regening)
            {
                StartCoroutine(EnergyRegen(0.8f));
            }
        }
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    //By Aubrey, make wait for second possible in this function
    void PlayerMovement()
    {

        if (dead)
        {
            return;
        }

        AnimatorMovementState();
        Climb();
        RagdollWhenFall();
        //StandUp();

        xAxis = controller.xAxis;
        yAxis = controller.yAxis;

        if (ragdollh.state == RagdollHelper.RagdollState.blendToAnim)
        {
            transformToRotate.localPosition = Vector3.Lerp(transformToRotate.localPosition, Vector3.zero, 20 * time.deltaTime);
        }

        if (equippedWeapon && !climbing && !switchingWeapons)
        {
            if (vcr.GetButton("Shoot"))
            {
                if (energy - currentWeapon.consumesEnergy > 0)
                {
                    //by Aubrey, play slash animation if using melee weapon
                    if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
                    {
                        //by Aubrey, stop melee animation repeat
                        if (vcr.GetButtonDown("Shoot"))
                        {
                            energy -= currentWeapon.consumesEnergy;
                            if (!playerAnimator.GetBool("Slash1") && !playerAnimator.GetBool("Slash2"))
                            //&& !playerAnimator.GetBool("Slash3"))
                            {
                                playerAnimator.SetTrigger("Slash1T");
                                playerAnimator.SetBool("Slash1", true);
                                currentWeapon.SetWeaponSwinging(true);
                                StartCoroutine(SetAnimBool("Slash1", false, 0.4f));
                            }
                            else if (playerAnimator.GetBool("Slash1"))
                            {
                                playerAnimator.SetTrigger("Slash2T");
                                playerAnimator.SetBool("Slash2", true);
                                playerAnimator.SetBool("Slash1", false);
                                currentWeapon.SetWeaponSwinging(true);
                                StartCoroutine(SetAnimBool("Slash2", false, 0.7f));
                            }
                            //else if (playerAnimator.GetBool("Slash2"))
                            //{
                            //    playerAnimator.SetTrigger("Slash3T");
                            //    playerAnimator.SetBool("Slash3", true);
                            //    playerAnimator.SetBool("Slash2", false);
                            //    currentWeapon.SetWeaponSwinging(true);
                            //    StartCoroutine(SetAnimBool("Slash3", false, 0.7f));
                            //}
                        }
                    }
                    currentWeapon.Shoot();
                }
            }
        }
        //by Aubrey, allow aim and attack while falling
        if (//inMoveState && 
            !climbing)
        {
            Vector3 orientedX = xAxis * cam.right;
            Vector3 orientedY = yAxis * cam.forward;

            //orientedX.y = 0;
            //orientedY.y = 0;

            moveAxis = orientedY + orientedX;
            Vector3 lookForward = cam.forward;

            //added by harry
            lookForward -= Vector3.Project(lookForward, myNormal);
            moveAxis -= Vector3.Project(moveAxis, myNormal);

            //Debug.Log ("TRACK 1:" + justJumpToWall);
            if (justJumpToWall)
            {
                //Debug.Log ("TRACK:" + justJumpToWall);
                rotationAux = Quaternion.LookRotation(lookForward, myNormal);
                justJumpToWall = false;
            }

            if (aim)
            {
                if (!cameraBehaviour.aimIsRightSide && crouch)
                {
                    lookForward -= cam.right / 2;
                }

                //added by harry
                rotationAux = Quaternion.LookRotation(lookForward, myNormal);
                //lookForward.y = 0;
                //rotationAux = Quaternion.LookRotation((transformToRotate.position + lookForward) - transformToRotate.position);
            }
            transformToRotate.rotation = Quaternion.Lerp(playerAnimator.transform.rotation, rotationAux, 10 * time.deltaTime);

            if (moveAxis != Vector3.zero)
            {
                if (!aim)
                {
                    rotationAux = Quaternion.LookRotation(moveAxis, myNormal);
                    //rotationAux = Quaternion.LookRotation((transformToRotate.position + moveAxis) - transformToRotate.position);
                }
            }

            //SPEED CHANGE
            float _speed = 0;

            moveSpeed = _speed; //add from Harry


            if (currentMovementState < 0.5f)
            {
                _speed = crouchSpeed * (playerAnimator.GetFloat("Move") * 2); //By Nathan, makes movement smoother and fits better with animations
            }
            else if (currentMovementState < 1.5f)
            {
                if (runKeyPressed > 1.5f && !crouch && !aim)
                {
                    _speed = runSpeed * (playerAnimator.GetFloat("Move") / 2); //By Nathan, makes movement smoother and fits better with animations
                }
                else
                {
                    _speed = walkSpeed * playerAnimator.GetFloat("Move"); //By Nathan, makes movement smoother and fits better with animations
                }
            }
            else if (currentMovementState < 2.5f)
            {
                if (runKeyPressed > 1.5f)
                {
                    _speed = aimWalkSpeed * (playerAnimator.GetFloat("Move") / 1.25f); //By Nathan, makes movement smoother and fits better with animations
                }
                else
                {
                    _speed = aimWalkSpeed * playerAnimator.GetFloat("Move"); //By Nathan, makes movement smoother and fits better with animations
                }
            }

            //Debug.Log(this.gameObject.name + "Move speed = " + _speed);

            if (!SomethingInFront())
            {
                if (!stopKeyInput)
                {
                    if (vcr.GetButton("Sprint") && moveAxis != Vector3.zero)
                    {

                        LerpSpeed(2);
                    }
                    else
                    {

                        LerpSpeed(1);
                    }

                    //By Aubrey, allow air movement
                    //if (grounded)
                    //{
                    Vector3 moveSpeed;

                    moveSpeed = moveAxis.normalized * _speed;

                    //Debug.Log(this.gameObject.name + "Move speed = " + moveSpeed);

                    //rb.velocity = new Vector3(moveSpeed.x, rb.velocity.y, moveSpeed.z);
                    //added by harry
                    //Debug.Log("move here");
                    rb.velocity = moveSpeed - Vector3.Project(moveSpeed, myNormal) + Vector3.Project(rb.velocity, myNormal);

                    //Debug.Log(this.gameObject.name + "Velocity = " + rb.velocity);
                    //}
                }
            }
            else
            {
                if (!wallWalkerAbility)
                {
                    //By harry, make movement allowed even there is a wall in front.
                    Vector3 moveSpeed;

                    //Debug.Log ("moveAxis: " + moveAxis.normalized + "new moveAxis: " + (moveAxis - Vector3.Project (moveAxis, inFrontHit.normal)).normalized);
                    moveSpeed = (moveAxis.normalized - Vector3.Project(moveAxis.normalized, inFrontHit.normal)) * _speed;
                    rb.velocity = moveSpeed - Vector3.Project(moveSpeed, myNormal) + Vector3.Project(rb.velocity, myNormal);
                }
                else
                {
                    /*
					Ray ray;
					RaycastHit hit;
					Transform myTransform = transform;

					ray = new Ray(myTransform.position, transformToRotate.forward);
					if (Physics.Raycast(ray, out hit, jumpRange))
					{ // wall ahead?
						if (hit.collider.tag == "Climbable")
						{
							JumpToWall(hit.point + transform.up * (capsuleC.radius + 0.5f), hit.normal); // yes: jump to the wall
							return;
						}
					}*/
                    if (inFrontHit.collider.tag == "walkable" && grounded)
                    {
                        JumpToWall(inFrontHit.point - transform.up * (capsuleC.height / 2 - capsuleC.radius - 0.5f), inFrontHit.normal); // yes: jump to the wall
                        return;
                    }
                }

                /*
            if (aim)
            {
                //By Aubrey, allow air movement
                //if (grounded)
                //{
                //Vector3 moveSpeed;
                moveSpeed = moveAxis.normalized * _speed;
                rb.velocity = moveSpeed - Vector3.Project(moveSpeed, myNormal) + Vector3.Project(rb.velocity, myNormal);

                //rb.velocity = new Vector3(moveSpeed.x, rb.velocity.y, moveSpeed.z);
                //}
            }*/
            }
        }

        //added by harry
        if (wallWalkerAllowedText != null)
        {
            wallWalkerAllowedText.text = "Wall walker ability: " + wallWalkerAbility
            + "\nWall in front can jump on: " + wallWalkerAllowed();
        }

        Jump();
        Crouch();
        if (ability1MenuOpen)
            SwitchWeapon();
        PickUpWeapon();

        if (!switchingWeapons)
        {
            //By Aubrey
            if (!stopKeyInput)
            {
                if ((vcr.GetButtonDown("EquiptWeapon")) && weapons.Count > 0)
                {

                    //EquipWeaponToggle();
                }
                if (equippedWeapon)
                {
                    if (vcr.GetButtonDown("Reload"))
                    {
                        currentWeapon.Reload();
                    }
                }
            }
        }

        Aim();

        //By Aubrey, disable aim for melee weapon
        if (equippedWeapon)
        {
            if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
            {
                aim = false;
            }
        }
    }


    public void EquipWeaponToggle()
    {
        equippedWeapon = !equippedWeapon;

        if (weapons.Count > 0)
        {
            if (equippedWeapon)
            {
                currentWeapon = GetCurrentWeapon();
                leftHandInWeapon = currentWeapon.leftHand;
                rightHandInWeapon = currentWeapon.rightHand;
            }
            currentWeapon.ToggleRenderer(equippedWeapon);
        }
        if (OnWeaponSwitch != null)
        {
            OnWeaponSwitch();
        }

        //By Aubrey, activate and deactivate edge collider depend on wether holding weapon
        if (equippedWeapon)
        {
            if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
            {
                currentWeapon.SetEdgeCollider(true);
                currentWeapon.SetPlayerHolding(true);
            }
        }
        else
        {
            if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
            {
                currentWeapon.SetEdgeCollider(false);
                currentWeapon.SetPlayerHolding(false);
            }
        }
    }

    void SwitchWeapon()
    {
        int numberPressed = 0;
        bool pressed = false;
        for (int i = 0; i < 8; i++)
        {
            //By Aubrey
            if (!stopKeyInput)
            {
                if (vcr.GetButtonDown("Num" + (i + 1)))
                {
                    ability1HoldTimer = 0;
                    numberPressed = i;
                    pressed = true;
                }
                if (vcr.GetAxis("ScrollUp", "ScrollDown", 1f) != 0)
                {
                    if (vcr.GetAxis("ScrollUp", "ScrollDown", 0.5f) > 0)
                    {
                        numberPressed = -1;
                    }
                    else
                    {
                        numberPressed = -2;
                    }
                    pressed = true;
                    //Debug.Log("The Number is " + numberPressed);
                }
            }
        }
        if (pressed)
        {
            if (numberPressed < weapons.Count && !GetCurrentWeapon().reloadProgress)
            {
                if (numberPressed == -1)
                {
                    numberPressed = currentWeaponID + 1;
                    if (numberPressed >= weapons.Count)
                    {
                        numberPressed = 0;
                    }
                }
                else if (numberPressed == -2)
                {
                    numberPressed = currentWeaponID - 1;
                    if (numberPressed < 0)
                    {
                        numberPressed = weapons.Count - 1;
                    }
                }

                if (vcr.GetButton("Num" + (numberPressed + 1)))
                {
                    closeAbility1Menu = true;
                    ability1HoldTimer = 0.5f;
                }

                //By Aubrey, deactivate edge collider
                if (equippedWeapon)
                {
                    if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
                    {
                        currentWeapon.SetEdgeCollider(false);
                        currentWeapon.SetPlayerHolding(false);
                    }
                }
                if (numberPressed != currentWeaponID)
                {
                    if (!switchingWeapons)
                    {
                        StartCoroutine(WeaponSwitchProgress(numberPressed));
                    }
                }
                if (!equippedWeapon)
                {
                    EquipWeaponToggle();
                }
                //By Aubrey, activate edge collider
                if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
                {
                    currentWeapon.SetEdgeCollider(true);
                    currentWeapon.SetPlayerHolding(true);
                }

            }
        }
        else
        {
            closeAbility1Menu = false;
        }
    }

    IEnumerator WeaponSwitchProgress(int numberP)
    {
        switchingWeapons = true;
        halfSwitchingWeapons = false;
        yield return new WaitForSeconds(switchWeaponTime / 2);
        halfSwitchingWeapons = true;
        if (currentWeapon)
        {
            currentWeapon.ToggleRenderer(false);
        }
        currentWeaponID = numberP;
        if (!isClone)
        {
            ability1.transform.FindDeepChild("AbilityHolder").transform.Find("Text").GetComponent<Text>().text = (currentWeaponID + 1).ToString();
        }
        currentWeapon = GetCurrentWeapon();
        OnWeaponSwitch();
        if (equippedWeapon)
        {
            currentWeapon.ToggleRenderer(true);

            leftHandInWeapon = currentWeapon.leftHand;
            rightHandInWeapon = currentWeapon.rightHand;

            //By Aubrey, activate edge collider
            if (currentWeapon.shootingMode == WeaponBase.ShootingMode.Melee)
            {
                currentWeapon.SetEdgeCollider(true);
                currentWeapon.SetPlayerHolding(true);
            }
        }
        yield return new WaitForSeconds(switchWeaponTime);
        switchingWeapons = false;
    }

    void PickUpWeapon()
    {
        if (vcr.GetButtonDown("PickUpWeapon") && weapons.Count < bagLimit || controller.automaticPickUp && weapons.Count < bagLimit)
        {
            if (collidedWith != null)
            {
                if (collidedWith.tag == "Weapon" || collidedWith.tag == "MeleeWeapon")
                {
                    WeaponBase _c = collidedWith.GetComponent<WeaponBase>();
                    bool alreadyHave = false;
                    int id = 0;
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        if (weapons[i].weapon == _c.weapon)
                        {
                            alreadyHave = true;
                            id = i;
                        }
                    }
                    if (alreadyHave)
                    {
                        weapons[id].reloadBullets += _c.reloadBullets + _c.currentAmmo;
                        Destroy(_c.gameObject);
                    }
                    else
                    {
                        if (_c.shootingMode != WeaponBase.ShootingMode.Melee)
                        {
                            _c.transform.parent = aimHelper;
                        }
                        else
                        {
                            _c.transform.parent = RightHand;
                        }
                        _c.pB = this;
                        _c.PutInInventory();
                        _c.ToggleRenderer(false);
                        weapons.Add(_c);
                    }
                    collidedWith = null;
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyWeapon")
        {
            if (!invinsible)
            {
                invinsible = true;
                Damage(7);
                playerAnimator.SetTrigger("GotHit");
                StartCoroutine(SetInvinsibleBool(false, 0.7f));
            }
        }
        // if (collision.gameObject.tag == "Trigger")
        // {
        //     Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        // }
    }


    void OnCollisionStay(Collision col)
    {
        collidedWith = col.gameObject;
    }

    void OnCollisionExit()
    {
        collidedWith = null;
    }

    void AnimatorMovementState()
    {
        currentAnimatorState = playerAnimator.GetCurrentAnimatorStateInfo(0);
        inMoveState = currentAnimatorState.IsName("Grounded");
        playerAnimator.SetBool("Grounded", grounded);
        playerAnimator.SetFloat("Speed", runKeyPressed);
        playerAnimator.SetBool("HoldingWeapon", equippedWeapon);
        currentMovementState = playerAnimator.GetFloat("State");

        if (!stopKeyInput)
        {
            if (crouch)
            {
                playerAnimator.SetFloat("State", Mathf.Lerp(currentMovementState, 0, 5 * time.deltaTime));
            }
            else if (aim)
            {
                playerAnimator.SetFloat("State", Mathf.Lerp(currentMovementState, 2, 5 * time.deltaTime));
            }
            else
            {
                playerAnimator.SetFloat("State", Mathf.Lerp(currentMovementState, 1, 5 * time.deltaTime));
            }


            if (!SomethingInFront())
            {
                float _m = Mathf.Clamp01(Mathf.Abs(xAxis) + Mathf.Abs(yAxis));
                playerAnimator.SetFloat("Move", Mathf.Lerp(playerAnimator.GetFloat("Move"), _m * runKeyPressed, 10 * time.deltaTime));

                playerAnimator.SetFloat("AxisX", Mathf.Lerp(playerAnimator.GetFloat("AxisX"), xAxis, 10 * time.deltaTime));
                playerAnimator.SetFloat("AxisY", Mathf.Lerp(playerAnimator.GetFloat("AxisY"), yAxis, 10 * time.deltaTime));
            }
            else
            { // else condition added by harry
              //test1
              /*
              float _m = Mathf.Clamp01(Mathf.Abs(xAxis) + Mathf.Abs(yAxis));
              playerAnimator.SetFloat("Move", Mathf.Lerp(playerAnimator.GetFloat("Move"), _m * runKeyPressed, 10 * time.deltaTime));
              */
              //test2

                Vector3 moveForward = xAxis * (cam.right - Vector3.Project(cam.right, myNormal)) + yAxis * (cam.forward - Vector3.Project(cam.forward, myNormal));
                Vector3 rotatedHitNormal = Quaternion.Euler(GameObject.Find("Z").transform.rotation.x, GameObject.Find("Y").transform.rotation.y, 0) * inFrontHit.normal;
                Vector3 newMoveForward = moveForward - Vector3.Project(moveForward, rotatedHitNormal);
                //Debug.Log ("rotatedHitNormal: " + rotatedHitNormal + "moveforward: " + moveForward + "/newMovewForward: " + newMoveForward);
                Vector3 newForward = Vector3.Project(newMoveForward, cam.forward);
                Vector3 newRight = Vector3.Project(newMoveForward, cam.right);
                float newYAxis = newForward.y;
                float newXAxis = newRight.x;

                float _m = Mathf.Clamp01(Mathf.Abs(newXAxis) + Mathf.Abs(newYAxis));
                playerAnimator.SetFloat("Move", Mathf.Lerp(playerAnimator.GetFloat("Move"), _m * runKeyPressed, 10 * time.deltaTime));


                //Debug.Log ("tracker: Xaxis: " + xAxis + "/yAxis" + yAxis +"/newX: " + newXAxis + "/newY: " + newYAxis +"/_m: " + _m);

                playerAnimator.SetFloat("AxisX", newXAxis);//Mathf.Lerp(playerAnimator.GetFloat("AxisX"), newXAxis, 10 * time.deltaTime));
                playerAnimator.SetFloat("AxisY", newYAxis);//Mathf.Lerp(playerAnimator.GetFloat("AxisY"), newYAxis, 10 * time.deltaTime));
            }
        }
        else
        {
            if (aim)
            {
                float _yAxis = yAxis;
                _yAxis = Mathf.Clamp(_yAxis, -1, 0);

                float _m = Mathf.Clamp01(Mathf.Abs(xAxis) + Mathf.Abs(_yAxis));

                playerAnimator.SetFloat("Move", Mathf.Lerp(playerAnimator.GetFloat("Move"), _m * runKeyPressed, 10 * time.deltaTime));
                playerAnimator.SetFloat("AxisX", Mathf.Lerp(playerAnimator.GetFloat("AxisX"), xAxis, 10 * time.deltaTime));
                playerAnimator.SetFloat("AxisY", Mathf.Lerp(playerAnimator.GetFloat("AxisY"), _yAxis, 10 * time.deltaTime));
            }
            else
            {
                playerAnimator.SetFloat("Move", Mathf.Lerp(playerAnimator.GetFloat("Move"), 0, 5 * time.deltaTime));
                playerAnimator.SetFloat("AxisX", Mathf.Lerp(playerAnimator.GetFloat("AxisX"), 0, 10 * time.deltaTime));
                playerAnimator.SetFloat("AxisY", Mathf.Lerp(playerAnimator.GetFloat("AxisY"), 0, 10 * time.deltaTime));
            }
        }
    }

    public bool SomethingInFront()
    {
        Vector3 posToDetect = transformToRotate.position + transformToRotate.up * capsuleC.height / 2;
        //modeified by harry to find out infornt hit
        Ray ray = new Ray(posToDetect, transformToRotate.forward);
        //if (Physics.Raycast(ray, out inFrontHit, capsuleC.radius + .2f, layerMask))
        if (Physics.Raycast(posToDetect, transformToRotate.forward, 0.5f, triggerLayerMask)) // By Nathan, for smoother movement with ignore trigger layer enabled
        {
            return Physics.Raycast(ray, out inFrontHit, capsuleC.radius + .2f);
        }
        else
        {
            return Physics.Raycast(ray, out inFrontHit, capsuleC.radius + .2f, triggerLayerMask);
        }

        //return Physics.Raycast(posToDetect, transformToRotate.forward, 0.5f);
    }

    public bool SomethingInFrontAim(float distance)
    {
        Vector3 camF = cam.forward;
        camF.y = 0;
        Vector3 offset = -cam.right * 0.15f;
        if (!cameraBehaviour.aimIsRightSide)
        {
            offset = cam.right * 0.15f;
        }
        Vector3 posToDetect = transformToRotate.position + transformToRotate.up * .5f;
        return Physics.Raycast(posToDetect, camF + offset, distance, triggerLayerMask) && !Physics.Raycast(posToDetect + (offset * -5), camF + (offset * -5), distance, triggerLayerMask);
    }

    public void Aim()
    {
        //By Aubrey, allow aiming in the air
        if (!climbing //&& grounded
        && !ragdollh.ragdolled)
        {

            if (equippedWeapon)
            {
                WeaponBase _currentW = currentWeapon;


                if (controller.aimMode == Controller.AimMode.Hold)
                {
                    aim = vcr.GetButton("Aim") || vcr.GetButton("Shoot");
                }
                else
                {
                    if (!_currentW.reloadProgress && !_currentW.shootProgress)
                    {
                        if (vcr.GetButtonDown("Aim"))
                        {
                            aim = !aim;
                        }
                    }
                    else
                    {

                        aim = true;
                    }
                }
                if (currentWeapon.shootingMode != WeaponBase.ShootingMode.Melee)
                {
                    if (vcr.GetButtonDown("Aim"))
                    {
                        _currentW.AimAudio();
                    }
                    if (vcr.GetButtonUp("Aim"))
                    {
                        _currentW.AimAudio();
                    }
                }
            }
            else
            {
                if (controller.aimMode == Controller.AimMode.Hold)
                {
                    aim = vcr.GetButton("Aim");
                }
                else
                {
                    if (vcr.GetButtonDown("Aim"))
                    {
                        if (currentWeapon != null)//By Aubrey,  disable aim with melee weapon
                        {
                            aim = !aim;
                        }
                    }
                }
            }
        }

        if (equippedWeapon)
        {
            if (!aim)
            {
                currentWeapon.MoveTo(transformToRotate);
            }
            else
            {
                //By Aubrey, put melee weapon as a child of right hand instead of aim helper
                if (currentWeapon.shootingMode != WeaponBase.ShootingMode.Melee)
                {
                    currentWeapon.MoveTo(aimHelper);
                }
                else
                {
                    currentWeapon.MoveTo(RightHand);
                }

            }
            aimHelper.rotation = aimRotationAux;
        }
    }

    public void Crouch()
    {

        if (controller.canCrouch)
        {
            bool somethingAbove = Physics.Raycast(playerAnimator.GetBoneTransform(HumanBodyBones.Head).position + transform.up * .5f, transform.up, 1.4f, triggerLayerMask);
            //By Aubrey
            if (!stopKeyInput)
            {
                if (vcr.GetButtonDown("Crouch"))
                {
                    //Debug.Log("Crouching");
                    if (crouch && !somethingAbove)
                    {
                        crouch = false;
                    }
                    else
                    {
                        crouch = true;
                    }
                }
                if (vcr.GetButtonDown("Jump"))
                {
                    if (crouch && !somethingAbove)
                    {
                        crouch = false;
                    }
                }
                // if (somethingAbove)
                // {
                //     crouch = true;
                // }
                if (crouch)
                {
                    _capsuleSize = Mathf.Lerp(_capsuleSize, crouchHeight, 5 * time.deltaTime);
                }
                else
                {
                    _capsuleSize = Mathf.Lerp(_capsuleSize, characterHeight, 5 * time.deltaTime);
                }
                capsuleC.center = new Vector3(0, .9f * _capsuleSize, 0);
                capsuleC.height = 1.8f * _capsuleSize;
            }
        }

    }

    public void Jump()
    {
        bool canJumpBasedOnWeapon = true;

        //By Aubrey, allow reload jump
        //if (currentWeapon != null)
        //{
        //    if (currentWeapon.reloadProgress)
        //    {
        //        canJumpBasedOnWeapon = false;
        //    }
        //}
        //By Aubrey, allow jump while aiming
        if (grounded && inMoveState && !climbing && !crouch && !climbHit //&& !aim 
            && !ragdollh.ragdolled && canJumpBasedOnWeapon)
        {
            //By Aubrey
            if (!stopKeyInput)
            {
                if (vcr.GetButtonDown("Jump"))
                {
                    //Debug.Log("Jumping");
                    //playerAnimator.SetTrigger("Jump Forward");
                    //added by Harry
                    /*
                    if (wallWalkerAbility)
                    {
                        Ray ray;
                        RaycastHit hit;
                        Transform myTransform = transform;

                        ray = new Ray(myTransform.position, transformToRotate.forward);
                        if (Physics.Raycast(ray, out hit, jumpRange))
                        { // wall ahead?
                            if (hit.collider.tag == "Climbable")
                            {
                                JumpToWall(hit.point + transform.up * (capsuleC.radius + 0.5f), hit.normal); // yes: jump to the wall
                                return;
                            }
                        }
                    }*/

                    //above added by Harry
                    playerAnimator.SetTrigger("Jump Forward");
                    //Vector3 worldNormal = new Vector3(0, 1, 0);
                    if (wallWalkerAbility)// && (myNormal != worldNormal))
                    {
                        wallWalkerAbility = false;
                        DropDownCheck();
                    }
                    else
                    {
                        if (moveAxis != Vector3.zero && !SomethingInFront())
                        {
                            rb.velocity = myNormal * jumpForce + transformToRotate.forward * 4;
                        }
                        else
                        {
                            rb.velocity = myNormal * jumpForce / 1.1f;
                        }
                    }
                }
            }

        }
    }


    //added by Harry 
    private void JumpToWall(Vector3 point, Vector3 normal)
    {

        // jump to wall
        jumpingToWall = true; // signal it's jumpingToWall to wall
        if (time.timeScale > 0)
        {
            time.rigidbody.isKinematic = true; // disable physics while jumpingToWall
        }
        Vector3 orgPos = myTransform.position;
        Quaternion orgRot = myTransform.rotation;
        Quaternion orgRotA = transformToRotate.rotation;
        Vector3 dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
        Vector3 myForward = Vector3.Cross(myTransform.right, normal);
        Vector3 myForwardA = Vector3.Cross(transformToRotate.right, normal);
        Quaternion dstRot = Quaternion.LookRotation(myForward, normal);
        Quaternion dstRotA = Quaternion.LookRotation(myForwardA, normal);

        StartCoroutine(jumpTime(orgPos, orgRot, orgRotA, dstPos, dstRot, dstRotA, normal));

        //jumptime
    }

    private IEnumerator jumpTime(Vector3 orgPos, Quaternion orgRot, Quaternion orgRotA, Vector3 dstPos,
        Quaternion dstRot, Quaternion dstRotA, Vector3 normal)
    {

        for (float t = 0.0f; t < 1.0f;)
        {
            t += time.deltaTime;
            myTransform.position = Vector3.Lerp(orgPos, dstPos, t);
            myTransform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            transformToRotate.rotation = Quaternion.Slerp(orgRotA, dstRotA, t);
            //cam.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            //myTransform.rotation = transformToRotate.rotation;
            //animator.transform.position = Vector3.Lerp(orgPos, dstPos, t);
            //animator.transform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            yield return null; // return here next frame
        }
        myNormal = normal; // update myNormal
        if (time.timeScale > 0)
        {
            time.rigidbody.isKinematic = false; // enable physics
        }
        jumpingToWall = false; // jumpingToWall to wall finished
        justJumpToWall = true;
        //Debug.Log ("TRACK 2:" + justJumpToWall);

    }
    //above added by Harry


    public void Climb()
    {
        bool canClimbBasedOnWeapon = true;

        if (currentWeapon != null)
        {

            if (currentWeapon.reloadProgress)
            {
                canClimbBasedOnWeapon = false;
            }
        }


        RaycastHit hit;

        Vector3 climbRayPos = transform.position + transformToRotate.forward * 0.45f + transformToRotate.up * 2.1f * characterHeight;

        if (Physics.Raycast(climbRayPos, -transform.up, out hit, 1.8f, triggerLayerMask) && !ragdollh.ragdolled && canClimbBasedOnWeapon)
        {
            climbHit = true;

            climbY = hit.point.y;
            float dist = climbY - transform.position.y;
			//added bu harry in case climb into crack of two objects.
			bool areaToStand = true;
			if (Physics.Raycast (hit.point+transformToRotate.forward * 0.2f, transform.up, capsuleC.height-0.5f)) {
				areaToStand = false;
				Debug.Log ("no area");
			}

			if (hit.collider.tag == "Climbable" && areaToStand)
            {
                //By Aubrey
                if (!stopKeyInput)
                {
                    if ((controller.automaticClimb || vcr.GetButtonDown("Jump")))
                    {

                        if (pathMaker.play == false)
                        {
                            equippedbefore = equippedWeapon;
                            if (equippedWeapon)
                            {
                                EquipWeaponToggle();
                            }
                            if (dist > 1f)
                            {
                                climbing = true;
                                aim = false;
                                playerAnimator.SetTrigger("Climb");

                                pathMaker.pointsTime[0] = Vector3.Distance(transform.position, pathMaker.points[0]);
                                pathMaker.points[0].y = climbY - 1.5f;

                                pathMaker.pointsTime[1] = 1;
                                pathMaker.points[1].y = climbY + 0.8f;
                                pathMaker.points[1].z = 1f;

                                pathMaker.pointsTime[2] = 1;
                                pathMaker.points[2].y = climbY + 1.3f;
                                pathMaker.points[2].z = 1f;
                                pathMaker.Play();
                                return;
                            }
                        }
                    }

                }
            }
            else
            {
                climbHit = false;
            }
            if (climbing)
            {
                ikControll.LerpHandWeight(1f, 3f);
                ikControll.leftHandPos = hit.point + transformToRotate.right * -0.3f + transformToRotate.forward * -0.3f;
                ikControll.rightHandPos = hit.point + transformToRotate.right * 0.3f + transformToRotate.forward * -0.3f;
            }
        }
        else
        {
            climbHit = false;
            climbing = false;
            ikControll.LerpHandWeight(0f, 5);
            if (equippedbefore)
            {
                equippedbefore = false;
                EquipWeaponToggle();
            }
        }
    }

    public void StandUp()
    {
        if (boneRb[0].transform.parent == null && ragdollh.ragdolled)
        {
            transform.position = boneRb[0].position;
        }
        if (ragdollh.ragdolled)
        {
            //RaycastHit h;
            //By Aubrey
            //if (vcr.GetButtonDown("Jump") && Physics.SphereCast(transform.position + transform.up * 1, 0.2f, -transform.up, out h, 3f))
            //{
            stopKeyInput = true;
            invinsible = true;
            ToggleRagdoll();
            StartCoroutine(SetStopKeyBool(false, 1.9f));
            StartCoroutine(SetInvinsibleBool(false, 2f));
            //}
        }
    }

    public void ToggleRagdoll()
    {
        if (boneRb[0].velocity.magnitude > 1) { return; }
        bool ragdoll = !ragdollh.ragdolled;
        foreach (Rigidbody r in boneRb)
        {

            if (ragdoll == false)
            {
                capsuleC.enabled = true;

                r.isKinematic = true;
                r.velocity = Vector3.zero;

                boneRb[0].transform.parent = hipsParent;

                cameraParent.parent = transform;
                ragdollh.ragdolled = false;
            }
            else
            {
                //if (equippedWeapon)
                //{
                //    EquipWeaponToggle();
                //}
                crouch = false;
                ragdollh.ragdolled = true;
                aim = false;
                pathMaker.Reset();
                rb.useGravity = false;

                r.isKinematic = false;
                r.velocity = rb.velocity * 1.5f;
                playerAnimator.SetFloat("Move", 0);
                playerAnimator.enabled = false;

                rb.velocity = Vector3.zero;
                rb.isKinematic = true;

                capsuleC.enabled = false;

                boneRb[0].transform.parent = null;
                cameraParent.parent = null;
            }
        }
    }

    public void RagdollWhenFall()
    {
        if (!ragdollh.ragdolled && ragdollWhenFall)
        {
            if (rb.velocity.y < -15)
            {
                ToggleRagdoll();
            }
        }
    }

    public void Damage(float amount)
    {
        if (!dead)
        {
            life -= amount;

            if (gameObject.tag == "RecordedPlayer")
                GameObject.Find("Player").GetComponent<PlayerBehaviour>().Damage(amount);

            if (gameObject.tag == "Player")
            {
                // Spawns the gained Health numbers randomly above the HealthBar
                newHealthNumbers = Instantiate(healthChangeNumbers.gameObject);
                newHealthNumbers.transform.SetParent(gameManager.transform.FindDeepChild("HealthBar"), false);
                newHealthNumbers.transform.localPosition = new Vector3(UnityEngine.Random.Range(130.0f, -60.0f), UnityEngine.Random.Range(100.0f, 130.0f), 0);
                newHealthNumbers.transform.FindDeepChild("Text").GetComponent<Text>().text = "-" + Mathf.Round(amount).ToString();
            }

            if (life <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        dead = true;
        life = 0;
        if (!ragdollh.ragdolled)
        {
            ToggleRagdoll();
        }
    }

    public void Revive()
    {
        if (dead)
        {
            Start();
            ToggleRagdoll();
        }
    }


    // override ground check by Harry
    void GroundCheck()
    {
        RaycastHit hit;

        //if (Physics.Raycast(ray, out hit)) // use it to update myNormal and isGrounded

        if (Physics.SphereCast(transformToRotate.position + myNormal * 2, .15f, -myNormal, out hit, 2.5f))
        {
            grounded = true;
            if (hit.collider.tag == "walkable")
            {
                surfaceNormal = hit.normal;
            }
            else
            {
                //Vector3 worldNormal = new Vector3 (0, 1, 0);
                if (wallWalkerAbility)//myNormal != worldNormal)
                {
                    wallWalkerAbility = false;
                    DropDownCheck();
                    return;
                }
                surfaceNormal = myNormal;
            }
            if (moveAxis == Vector3.zero || ragdollh.state == RagdollHelper.RagdollState.blendToAnim)
            {
                pM.staticFriction = 3;
                pM.dynamicFriction = 3;
            }
            else
            {
                pM.staticFriction = 0;
                pM.dynamicFriction = 0;
            }
        }
        else
        {
            grounded = false;
            pM.staticFriction = 0;
            pM.dynamicFriction = 0;

            // assume usual ground normal to avoid "falling forever"
            //surfaceNormal = Vector3.up;
        }
        if (wallWalkerAbility)
        {
            //Debug.Log("track: "+grounded);
            myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed * time.deltaTime);
            // find forward direction with new myNormal:
            Vector3 myForward = Vector3.Cross(myTransform.right, myNormal);
            // align character to the new myNormal while keeping the forward direction:
            Quaternion targetRot = Quaternion.LookRotation(myForward, myNormal);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed * time.deltaTime);
            // move the character forth/back with Vertical axis:
            myTransform.Translate(0, 0, Input.GetAxis("Vertical") * moveSpeed * time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (time.timeScale > 0)
        {
            // apply constant weight force according to character normal:
            time.rigidbody.AddForce(-gravity * time.rigidbody.mass * myNormal);
        }
    }

    /*
    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + transform.up * 2, .15f, -transform.up, out hit, 2.5f))
        {
            grounded = true;
            if (moveAxis == Vector3.zero || ragdollh.state == RagdollHelper.RagdollState.blendToAnim)
            {
                pM.staticFriction = 3;
                pM.dynamicFriction = 3;
            }
            else
            {
                pM.staticFriction = 0;
                pM.dynamicFriction = 0;
            }
        }
        else
        {
            grounded = false;
            pM.staticFriction = 0;
            pM.dynamicFriction = 0;
        }
        //Debug.Log("track: "+grounded);
    }
    */
    void LerpSpeed(float final)
    {
        runKeyPressed = Mathf.Lerp(runKeyPressed, final, 10 * time.deltaTime);
    }

    void Gravity()
    {
        //added from harry
        //test2
        if (ragdollh.state == RagdollHelper.RagdollState.animated)
        {
            Vector3 velocity = rb.velocity;
            velocity -= 10F * time.deltaTime * myNormal;
            //Debug.Log ("Track"+myNormal);
            rb.velocity = velocity;
        }
        //test1 GetComponent<Rigidbody>().AddForce(-gravity * GetComponent<Rigidbody>().mass * myNormal);
        /*
        if (ragdollh.state == RagdollHelper.RagdollState.animated)
        {
            Vector3 velocity = rb.velocity;
            velocity.y -= 10F * time.deltaTime;
            rb.velocity = velocity;
        }*/
    }



    public WeaponBase GetCurrentWeapon()
    {
        if (weapons.Count > 0)
        {
            return weapons[currentWeaponID];
        }
        else
        {
            return null;
        }
    }

    //Add by harry
    public bool wallWalkerAllowed()
    {
        Ray ray;
        RaycastHit hit;

        ray = new Ray(transformToRotate.position, transformToRotate.forward);
        //Debug.Log ("track: " + myTransform.forward);
        if ((Physics.Raycast(ray, out hit, jumpRange, triggerLayerMask)) && wallWalkerAbility)
        { // wall ahead?
            if (hit.collider.tag == "walkable")
            {
                return true;
            }
        }
        return false;
    }

    //trigger events added by harry
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log ("trigger enter");
        inTrigger = true;
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log ("trigger exit");
        inTrigger = false;
    }

    public bool isInTrigger()
    {
        return inTrigger;
    }

    IEnumerator SetAnimBool(String param, bool stat, float time)
    {
        yield return new WaitForSeconds(time);

        playerAnimator.SetBool(param, stat);
    }

    IEnumerator SetInvinsibleBool(bool stat, float time)
    {
        yield return new WaitForSeconds(time);

        invinsible = stat;
    }

    IEnumerator SetStopKeyBool(bool stat, float time)
    {
        yield return new WaitForSeconds(time);

        stopKeyInput = stat;
    }

    IEnumerator EnergyRegen(float time)
    {
        regening = true;
        energy += energyRegenerateRate;
        if (gameObject.tag == "Player")
        {
            AudioManager.instance.Play("EnergyRegen");
        }
        yield return new WaitForSeconds(time);
        regening = false;

    }
    IEnumerator StandUpIn(float time)
    {
        yield return new WaitForSeconds(time);
        StandUp();

    }

    public void StandUpAfter(float time)
    {
        StartCoroutine(StandUpIn(time));
    }

    public void ResetRigidBody()
    {
        //rb.isKinematic = false;
        //rb.useGravity = false;
        time.rigidbody.isKinematic = false;
        time.rigidbody.useGravity = false;
    }

}