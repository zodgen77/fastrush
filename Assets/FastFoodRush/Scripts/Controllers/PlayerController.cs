using UnityEngine;

namespace CryingSnow.FastFoodRush
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Tooltip("Base movement speed of the player")]
        private float baseSpeed = 3.0f;

        [SerializeField, Tooltip("Speed at which the player rotates")]
        private float rotateSpeed = 360f;

        [SerializeField, Tooltip("Base capacity for the player's stack")]
        private int baseCapacity = 5;

        [SerializeField, Tooltip("Array of footstep sound clips")]
        private AudioClip[] footsteps;

        [SerializeField, Tooltip("Reference to the player's stack (for holding items)")]
        private WobblingStack stack;

        [SerializeField, Tooltip("Target for the left hand position in IK")]
        private Transform leftHandTarget;

        [SerializeField, Tooltip("Target for the right hand position in IK")]
        private Transform rightHandTarget;


        public WobblingStack Stack => stack; // Property to access the stack
        public int Capacity { get; private set; } // The current capacity of the player's stack

        private Animator animator; // Reference to the player's Animator
        private CharacterController controller; // Reference to the CharacterController for movement
        private AudioSource audioSource; // Reference to the AudioSource for playing sound effects

        private float moveSpeed; // Current movement speed based on upgrades
        private Vector3 movement; // Player's movement direction
        private Vector3 velocity; // Player's velocity (for gravity)
        private bool isGrounded; // Flag to check if the player is grounded

        private float IK_Weight; // Weight of the IK for hand positioning

        const float gravityValue = -9.81f; // Gravity constant

        void Awake()
        {
            animator = GetComponent<Animator>(); // Initialize the Animator
            controller = GetComponent<CharacterController>(); // Initialize the CharacterController
            audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
        }

        void Start()
        {
            RestaurantManager.Instance.OnUpgrade += UpdateStats; // Subscribe to the upgrade event
            UpdateStats(); // Update player stats based on upgrades
        }

        void Update()
        {
            isGrounded = controller.isGrounded; // Check if the player is grounded

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0f; // Reset vertical velocity when grounded
            }

            movement.x = SimpleInput.GetAxis("Horizontal");
            movement.z = SimpleInput.GetAxis("Vertical");
            movement = (Quaternion.Euler(0, 45, 0) * movement).normalized; // Normalize and rotate movement for diagonal input

            controller.Move(movement * Time.deltaTime * moveSpeed); // Move the character

            if (movement != Vector3.zero)
            {
                var lookRotation = Quaternion.LookRotation(movement); // Calculate the desired rotation based on movement
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed); // Rotate towards the direction
            }

            velocity.y += gravityValue * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime); // Apply gravity

            animator.SetBool("IsMoving", movement != Vector3.zero);
        }

        void UpdateStats()
        {
            // Update the movement speed based on the speed upgrade level
            int speedLevel = RestaurantManager.Instance.GetUpgradeLevel(Upgrade.PlayerSpeed);
            moveSpeed = baseSpeed + (speedLevel * 0.2f);

            // Update the stack capacity based on the capacity upgrade level
            int capacityLevel = RestaurantManager.Instance.GetUpgradeLevel(Upgrade.PlayerCapacity);
            Capacity = baseCapacity + (capacityLevel * 3);
        }

        public void OnStep(AnimationEvent animationEvent)
        {
            // Trigger footstep sound based on the animation event
            if (animationEvent.animatorClipInfo.weight < 0.5f) return; // Ensure the animation is halfway through

            audioSource.clip = footsteps[Random.Range(0, footsteps.Length)]; // Pick a random footstep sound
            audioSource.Play(); // Play the footstep sound
        }

        void OnAnimatorIK()
        {
            // Gradually adjust IK weight based on the stack height
            IK_Weight = Mathf.MoveTowards(IK_Weight, Mathf.Clamp01(stack.Height), Time.deltaTime * 3.5f);

            // Set the IK position and rotation for the left hand
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IK_Weight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IK_Weight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            // Set the IK position and rotation for the right hand
            if (rightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, IK_Weight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, IK_Weight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
        }
    }
}
