using UnityEngine;

namespace CloneGame.Launch
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bird : MonoBehaviour
    {
        [SerializeField] private float minRotationSpeed = 0.5f;

        private Rigidbody2D rb;
        public bool IsFlying { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void FixedUpdate()
        {
            if (!IsFlying) return;
            if (rb.linearVelocity.magnitude < minRotationSpeed) return;

            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            rb.MoveRotation(angle);
        }

        public void SetHeld(bool held)
        {
            rb.bodyType = held ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            if (held)
            {
                rb.linearVelocity = Vector2.zero;
                IsFlying = false;
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