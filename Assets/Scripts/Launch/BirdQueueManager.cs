using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CloneGame.Launch
{
    public class BirdQueueManager : MonoBehaviour
    {
        [SerializeField] private SlingshotController slingshot;
        [SerializeField] private Bird birdPrefab;
        [SerializeField] private Transform[] waitingSlots;
        [SerializeField] private int totalBirds = 4;
        [SerializeField] private float reloadHopSpeed = 6f;
        [SerializeField] private float despawnDelay = 2.5f;

        private readonly Queue<Bird> waitingBirds = new Queue<Bird>();
        private Bird birdMovingToSlot;
        private Transform birdMovingTarget;

        public int RemainingBirds => waitingBirds.Count + (birdMovingToSlot != null ? 1 : 0);

        private void Start()
        {
            for (int i = 0; i < slingshot.BirdsRemaining; i++)
            {
                Transform slot = waitingSlots[Mathf.Min(i, waitingSlots.Length - 1)];
                Bird bird = Instantiate(birdPrefab, slot.position, Quaternion.identity);
                bird.SetHeld(true);
                waitingBirds.Enqueue(bird);
            }

            LoadNextBird();
        }

        private void Update()
        {
            if (birdMovingToSlot != null)
            {
                birdMovingToSlot.transform.position = Vector3.MoveTowards(
                    birdMovingToSlot.transform.position,
                    birdMovingTarget.position,
                    reloadHopSpeed * Time.deltaTime);

                if (Vector3.Distance(birdMovingToSlot.transform.position, birdMovingTarget.position) < 0.01f)
                {
                    birdMovingToSlot = null;
                }
            }
        }

        private void OnEnable() => SlingshotController.OnBirdLaunched += HandleBirdLaunched;
        private void OnDisable() => SlingshotController.OnBirdLaunched -= HandleBirdLaunched;

        private void HandleBirdLaunched(Bird bird)
        {
            bird.OnSettled += () =>
            {
                LoadNextBird();
                StartCoroutine(DespawnAfterDelay(bird, despawnDelay));
            };
        }

        private IEnumerator DespawnAfterDelay(Bird bird, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (bird != null) Destroy(bird.gameObject);
        }

        private void LoadNextBird()
        {
            if (slingshot.BirdsRemaining <= 0 || waitingBirds.Count == 0)  
            { 
                Debug.Log("No more birds to load"); 
                return;  
            }  

            if (waitingBirds.Count == 0) return;

            Bird next = waitingBirds.Dequeue();
            slingshot.LoadBird(next);
            ShuffleQueueForward();
        }

        private void ShuffleQueueForward()
        {
            int i = 0;
            foreach (Bird bird in waitingBirds)
            {
                Transform target = waitingSlots[Mathf.Min(i, waitingSlots.Length - 1)];
                if (i == 0)
                {
                    birdMovingToSlot = bird;
                    birdMovingTarget = target;
                }
                i++;
            }
        }
    }
}