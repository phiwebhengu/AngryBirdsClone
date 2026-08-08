using UnityEngine;

namespace CloneGame.Launch
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bird : MonoBehaviour
    {
        private Rigidbody2D rb;
        public bool IsFlying { get; private set; }

        private void Awake() => rb = GetComponent<Rigidbody2D>();

        public void SetHeld(bool held)
        {
            rb.bodyType = held ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            if (held) rb.linearVelocity = Vector2.zero;
        }

        public void Launch(Vector2 velocity)
        {
            SetHeld(false);
            rb.linearVelocity = velocity;
            IsFlying = true;
        }
    }
}