using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CloneGame.Launch
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private float followSpeed = 4f;
        [SerializeField] private float returnSpeed = 3f;
        [SerializeField] private float minVelocityToFollow = 1f;

        [Header("Zoom")]
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 15f;
        [SerializeField] private float baseZoom = 6f;
        [SerializeField] private float maxFlightZoom = 10f;
        [SerializeField] private float zoomLerpSpeed = 2f;
        [SerializeField] private float scrollSensitivity = 0.05f;

        [Header("Level Intro Pan")]
        [SerializeField] private bool playIntroPan = true;
        [SerializeField] private float introPanDuration = 1.2f;
        [SerializeField] private float introHoldDuration = 0.6f;

        private Vector3 homePosition;
        private float manualZoomOffset;
        private Bird trackedBird;
        private Camera cam;
        private bool introPanPlaying;

        private void Awake()
        {
            cam = Camera.main;
            homePosition = transform.position;
            cam.orthographicSize = baseZoom;
        }

        private void Start()
        {
            if (playIntroPan)
            {
                introPanPlaying = true;
                StartCoroutine(IntroPanRoutine());
            }
        }

        private void OnEnable() => SlingshotController.OnBirdLaunched += HandleBirdLaunched;
        private void OnDisable() => SlingshotController.OnBirdLaunched -= HandleBirdLaunched;

        private void HandleBirdLaunched(Bird bird) => trackedBird = bird;

        private void Update()
        {
            if (!introPanPlaying) HandleManualZoom();
        }

        private void HandleManualZoom()
        {
            if (Mouse.current == null) return;
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                manualZoomOffset -= scroll * scrollSensitivity;
                manualZoomOffset = Mathf.Clamp(manualZoomOffset, minZoom - baseZoom, maxZoom - baseZoom);
            }
        }

        private void LateUpdate()
        {
            if (introPanPlaying) return;

            bool isActivelyFlying = trackedBird != null
                && trackedBird.IsFlying
                && trackedBird.CurrentSpeed >= minVelocityToFollow;

            if (isActivelyFlying)
            {
                Vector3 targetPos = trackedBird.transform.position;
                targetPos.z = transform.position.z;
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

                float distanceFromHome = Vector3.Distance(transform.position, homePosition);
                float t = Mathf.Clamp01(distanceFromHome / 12f);
                float targetZoom = Mathf.Lerp(baseZoom, maxFlightZoom, t) + manualZoomOffset;
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomLerpSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, homePosition, returnSpeed * Time.deltaTime);
                float targetZoom = baseZoom + manualZoomOffset;
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomLerpSpeed * Time.deltaTime);
            }

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        private IEnumerator IntroPanRoutine()
        {
            GameObject[] pigs = GameObject.FindGameObjectsWithTag("Pig");
            if (pigs.Length == 0)
            {
                introPanPlaying = false;
                yield break;
            }

            Vector3 farthest = pigs[0].transform.position;
            foreach (GameObject pig in pigs)
            {
                if (pig.transform.position.x > farthest.x)
                    farthest = pig.transform.position;
            }
            Vector3 panTarget = new Vector3(farthest.x, homePosition.y, homePosition.z);

            yield return new WaitForSeconds(0.3f);

            float t = 0f;
            while (t < introPanDuration)
            {
                t += Time.deltaTime;
                float p = t / introPanDuration;
                transform.position = Vector3.Lerp(homePosition, panTarget, p);
                cam.orthographicSize = Mathf.Lerp(baseZoom, maxFlightZoom, p);
                yield return null;
            }

            yield return new WaitForSeconds(introHoldDuration);

            t = 0f;
            while (t < introPanDuration)
            {
                t += Time.deltaTime;
                float p = t / introPanDuration;
                transform.position = Vector3.Lerp(panTarget, homePosition, p);
                cam.orthographicSize = Mathf.Lerp(maxFlightZoom, baseZoom, p);
                yield return null;
            }

            transform.position = homePosition;
            cam.orthographicSize = baseZoom;
            introPanPlaying = false;
        }

        public void SkipToHome()
        {
            StopAllCoroutines();
            introPanPlaying = false;
            transform.position = homePosition;
            cam.orthographicSize = baseZoom + manualZoomOffset;
        }
    }
}