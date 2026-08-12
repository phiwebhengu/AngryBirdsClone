using UnityEngine;

namespace CloneGame.Launch
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bird : MonoBehaviour
    {
        [SerializeField] private float minRotationSpeed = 0.5f;
        [SerializeField] private float settleVelocityThreshold = 0.15f;
        [SerializeField] private float settleTimeRequired = 1f;

        private Rigidbody2D rb;
        private float settleTimer;
        private bool hasSettled;

        public bool IsFlying { get; private set; }
        public event System.Action OnSettled;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void FixedUpdate()
        {
            if (!IsFlying || hasSettled) return;

            if (rb.linearVelocity.magnitude >= minRotationSpeed)
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                rb.MoveRotation(angle);
            }

            if (rb.linearVelocity.magnitude < settleVelocityThreshold)
            {
                settleTimer += Time.fixedDeltaTime;
                if (settleTimer >= settleTimeRequired)
                {
                    hasSettled = true;
                    OnSettled?.Invoke();
                }
            }
            else
            {
                settleTimer = 0f;
            }
        }

        public void SetHeld(bool held)
        {
            rb.bodyType = held ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            if (held)
            {
                rb.linearVelocity = Vector2.zero;
                IsFlying = false;
                hasSettled = false;
                settleTimer = 0f;
            }
        }

        public void Launch(Vector2 velocity)
        {
            SetHeld(false);
            rb.linearVelocity = velocity;
            IsFlying = true;
        }
    }
}