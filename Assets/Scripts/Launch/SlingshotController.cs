using UnityEngine;
using UnityEngine.InputSystem;

namespace CloneGame.Launch
{
    public class SlingshotController : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Bird birdPrefab;
        [SerializeField] private float maxPullDistance = 2.5f;
        [SerializeField] private float launchForceMultiplier = 8f;

        private Bird currentBird;
        private Vector2 pullVector;
        private bool isDragging;

        public bool CanLaunch { get; private set; } = true;

        private void Update()
        {
            if (!CanLaunch || !isDragging) return;
            UpdateAim(GetPointerWorldPosition());
        }

        public void OnPointerPress(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnDragStart();
            }
            else if (context.canceled)
            {
                OnDragEnd();
            }
        }

        private void OnDragStart()
        {
            if (!CanLaunch) return;
            isDragging = true;
            SpawnBirdAtPivot();
        }

        private void OnDragEnd()
        {
            if (!isDragging) return;
            isDragging = false;
            Launch();
        }

        private void UpdateAim(Vector2 pointerWorld)
        {
            Vector2 offset = pointerWorld - (Vector2)pivot.position;
            offset = Vector2.ClampMagnitude(offset, maxPullDistance);
            pullVector = offset;
            currentBird.transform.position = pivot.position + (Vector3)pullVector;
        }

        private void Launch()
        {
            Vector2 launchVelocity = -pullVector * launchForceMultiplier;
            currentBird.Launch(launchVelocity);
            CanLaunch = false;
        }

        private void SpawnBirdAtPivot()
        {
            currentBird = Instantiate(birdPrefab, pivot.position, Quaternion.identity);
            currentBird.SetHeld(true);
        }

        private Vector2 GetPointerWorldPosition()
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            return Camera.main.ScreenToWorldPoint(screenPos);
        }
    }
}