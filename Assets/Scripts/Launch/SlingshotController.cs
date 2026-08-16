using UnityEngine;
using UnityEngine.InputSystem;

namespace CloneGame.Launch
{
    public class SlingshotController : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private float maxPullDistance = 2.5f;
        [SerializeField] private float minPullDistance = 0.3f;
        [SerializeField] private float launchForceMultiplier = 10f;

        [Header("Elastic Band")]
        [SerializeField] private Transform leftAnchor;
        [SerializeField] private Transform rightAnchor;
        [SerializeField] private LineRenderer leftBand;
        [SerializeField] private LineRenderer rightBand;
        [SerializeField] private Color bandColor = new Color(0.35f, 0.2f, 0.1f);

        [Header("Bird Management")]
        [SerializeField] private int totalBirds = 3;

        private Bird currentBird;
        private Vector2 pullVector;
        private bool isDragging;
        private int birdsLaunched = 0;
        private GameManager gameManager;

        public bool CanLaunch { get; private set; } = true;
        public int BirdsRemaining => totalBirds - birdsLaunched;
        public static event System.Action<Bird> OnBirdLaunched;

        private void Awake()
        {
            gameManager = FindAnyObjectByType<GameManager>();
            EnsureBandMaterial(leftBand);
            EnsureBandMaterial(rightBand);
            HideBands();
        }

        private void EnsureBandMaterial(LineRenderer band)
        {
            if (band.sharedMaterial != null) return;
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = bandColor;
            band.material = mat;
            band.startWidth = 0.08f;
            band.endWidth = 0.08f;
        }

        private void Update()
        {
            if (!CanLaunch || !isDragging) return;
            UpdateAim(GetPointerWorldPosition());
        }

        public void OnPointerPress(InputAction.CallbackContext context)
        {
            if (context.started) OnDragStart();
            else if (context.canceled) OnDragEnd();
        }

        private void OnDragStart()
        {
            if (!CanLaunch || currentBird == null) return;
            isDragging = true;
            ShowBands();
        }

        private void OnDragEnd()
        {
            if (!isDragging) return;
            isDragging = false;

            if (pullVector.magnitude < minPullDistance)
            {
                currentBird.transform.position = pivot.position;
                HideBands();
                return;
            }

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
            CanLaunch = false;
            currentBird = null;

            if (BirdsRemaining <= 0)
            {
                Debug.Log("All birds launched. No more birds remaining.");
                gameManager?.OnAllBirdsLaunched();
            }
        }

        public void LoadBird(Bird bird)
        {
            if (BirdsRemaining <= 0)
            {
                Debug.Log("No birds remaining to load");
                return;
            }
            currentBird = bird;
            currentBird.transform.position = pivot.position;
            currentBird.SetHeld(true);
            CanLaunch = true;
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
            return BirdsRemaining;
        }
    }
}