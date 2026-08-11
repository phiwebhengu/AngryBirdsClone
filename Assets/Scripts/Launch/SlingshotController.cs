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

        [Header("Elastic Band")]
        [SerializeField] private Transform leftAnchor;
        [SerializeField] private Transform rightAnchor;
        [SerializeField] private LineRenderer leftBand;
        [SerializeField] private LineRenderer rightBand;

        private Bird currentBird;
        private Vector2 pullVector;
        private bool isDragging;

        public bool CanLaunch { get; private set; } = true;
        public static event System.Action<Bird> OnBirdLaunched;

        [Header("Bird Management")]
        [SerializeField] private int totalBirds = 3;
        public int birdsRemaining;
        private int birdsLaunched = 0;
        private GameManager gameManager;
        private void Start()
        {
            HideBands();
            birdsRemaining = totalBirds;
            gameManager = FindAnyObjectByType<GameManager>();
        }

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
            ShowBands();
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

            Vector3 birdPos = pivot.position + (Vector3)pullVector;
            currentBird.transform.position = birdPos;
            UpdateBands(birdPos);
        }

        private void Launch()
        {
            Vector2 launchVelocity = -pullVector * launchForceMultiplier;
            currentBird.Launch(launchVelocity);
            OnBirdLaunched?.Invoke(currentBird);
            HideBands();

            birdsLaunched++;
            birdsRemaining--;

            if (birdsRemaining <= 0)
            {

                CanLaunch = false;
                Debug.Log("All birds launched. No more birds remaining.");
                if (gameManager != null)
                {
                    gameManager.OnAllBirdsLaunched();
                }
            }

        }

        private void SpawnBirdAtPivot()
        {
            currentBird = Instantiate(birdPrefab, pivot.position, Quaternion.identity);
            currentBird.SetHeld(true);
        }

        private void ShowBands()
        {
            leftBand.positionCount = 2;
            rightBand.positionCount = 2;
        }

        private void HideBands()
        {
            leftBand.positionCount = 0;
            rightBand.positionCount = 0;
        }

        private void UpdateBands(Vector3 birdPos)
        {
            leftBand.SetPosition(0, leftAnchor.position);
            leftBand.SetPosition(1, birdPos);
            rightBand.SetPosition(0, rightAnchor.position);
            rightBand.SetPosition(1, birdPos);
        }

        private Vector2 GetPointerWorldPosition()
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            return Camera.main.ScreenToWorldPoint(screenPos);
        }
    

    public int GetRemainingBirdsCount()
        {
            return birdsRemaining;
        }

        
    } 
}