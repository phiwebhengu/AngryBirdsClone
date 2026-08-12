using CloneGame.Launch;
using Unity.VisualScripting;
using UnityEngine;



namespace CloneGame.Launch
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 4f;
        [SerializeField] private float returnSpeed = 3f;
        [SerializeField] private float minVelocityToFollow = 1f;
        private bool isDraggingCamera;
        [SerializeField] private Vector3 homePosition = new Vector3(-16.9f, 0f, -10f);
        private Bird trackedBird;

        private void Awake() => homePosition = new Vector3(-16.9f, 0f, -10f);

        private void OnEnable() => SlingshotController.OnBirdLaunched += HandleBirdLaunched;
        private void OnDisable() => SlingshotController.OnBirdLaunched -= HandleBirdLaunched;

        private void HandleBirdLaunched(Bird bird) => trackedBird = bird;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SkipToHome();
                
            }
        }
        private void LateUpdate()
        {
            if (trackedBird != null && trackedBird.IsFlying)
            {
                Vector3 targetPos = trackedBird.transform.position;
                targetPos.z = transform.position.z;
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, homePosition, returnSpeed * Time.deltaTime);
            }
        }

        public void SkipToHome()
        {
            transform.position = homePosition;
        }
       private void OnGUI()
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                float zoomDelta = Event.current.delta.y * 0.1f;
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + zoomDelta, 3f, 15f);
                Event.current.Use();
            }
        }
    }
    }
