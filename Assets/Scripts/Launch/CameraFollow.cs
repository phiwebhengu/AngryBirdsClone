using UnityEngine;

namespace CloneGame.Launch
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 4f;
        [SerializeField] private float returnSpeed = 3f;
        [SerializeField] private float minVelocityToFollow = 1f;

        private Vector3 homePosition;
        private Bird trackedBird;

        private void Awake() => homePosition = transform.position;

        private void OnEnable() => SlingshotController.OnBirdLaunched += HandleBirdLaunched;
        private void OnDisable() => SlingshotController.OnBirdLaunched -= HandleBirdLaunched;

        private void HandleBirdLaunched(Bird bird) => trackedBird = bird;

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
    }
}