using System.Collections.Generic;
using UnityEngine;

public class GenerateActorOnly : MonoBehaviour
{
    private List<Vector3> velocities;
    public List<Transform> TransformActor;

    [SerializeField] private int ammountofActor;
    [SerializeField] private int moveChangeFrequency;

    [SerializeField] private Vector2 spawnBoundary;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;

    [SerializeField] private GameObject actor;

    //private PositionUpdate positionUpdate;
    public bool withClass;

    void Start()
    {
        velocities = new List<Vector3>(ammountofActor);
        TransformActor = new List<Transform>(ammountofActor);

        for (int i = 0; i < ammountofActor; i++)
        {
            var distanceX = Random.Range(-spawnBoundary.x / 2, spawnBoundary.x / 2);
            var distanceZ = Random.Range(-spawnBoundary.y / 2, spawnBoundary.y / 2);

            Vector3 spawnPoint = (transform.position + Vector3.up * 5f) + new Vector3(distanceX, 0, distanceZ);
            var a = Instantiate(actor, spawnPoint, Quaternion.identity);

            velocities.Add(spawnPoint);
            TransformActor.Add(a.transform);
        }
    }

    void Update()
    {
        if (withClass)
            return;

        var positionUpdate = new PositionUpdate()
        {
            objectVelocities = velocities,
            jobDeltaTime = Time.deltaTime,
            moveSpeed = this.moveSpeed,
            turnSpeed = this.turnSpeed,
            time = Time.time,
            moveChangeFrequency = this.moveChangeFrequency,
            center = Vector3.zero,
            bounds = spawnBoundary,
            seed = System.DateTimeOffset.Now.Millisecond
        };

        for (int i = 0; i < TransformActor.Count; i++)
            positionUpdate.Execute(i, TransformActor[i]);
    }

    struct PositionUpdate
    {
        public List<Vector3> objectVelocities;

        public Vector2 bounds;
        public Vector3 center;

        public float jobDeltaTime;
        public float time;
        public float moveSpeed;
        public float turnSpeed;
        public int moveChangeFrequency;

        public float seed;

        public void Execute(int i, Transform transform)
        {
            Vector3 currentVelocity = objectVelocities[i];

            transform.position += transform.localToWorldMatrix.MultiplyVector(new Vector3(0, 0, 1)) * moveSpeed * jobDeltaTime * Random.Range(0.3f, 1.0f);

            if (currentVelocity != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), turnSpeed * jobDeltaTime);
            }

            Vector3 currentPosition = transform.position;

            var randomise = true;

            if (currentPosition.x > center.x + bounds.x / 2 || currentPosition.x < center.x - bounds.x / 2 || currentPosition.z > center.z + bounds.y / 2 || currentPosition.z < center.z - bounds.y / 2)
            {
                Vector3 internalPosition = new Vector3(center.x + Random.Range(-bounds.x / 2, bounds.x / 2) / 1.3f, 0, center.z + Random.Range(-bounds.y / 2, bounds.y / 2) / 1.3f);

                currentVelocity = (internalPosition - currentPosition).normalized;
                objectVelocities[i] = currentVelocity;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), turnSpeed * jobDeltaTime * 2);

                randomise = false;
            }

            if (randomise)
            {
                if (Random.Range(0, moveChangeFrequency) <= 2)
                {
                    objectVelocities[i] = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                }
            }
        }
    }
}
