using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;

public class JobGenerateActor : MonoBehaviour
{
    private NativeArray<Vector3> velocities;
    private TransformAccessArray transformAccessArray;

    [SerializeField] private int ammountofActor;
    [SerializeField] private int swimChangeFrequency;

    [SerializeField] private Vector2 spawnBoundary;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;

    [SerializeField] private Transform actor;

    private PositionUpdateJob positionUpdateJob;
    private JobHandle positionUpdateJobHandle;

    void Start()
    {
        velocities = new NativeArray<Vector3>(ammountofActor, Allocator.Persistent);

        // 2
        transformAccessArray = new TransformAccessArray(ammountofActor);

        for (int i = 0; i < ammountofActor; i++)
        {

            float distanceX =
            Random.Range(-spawnBoundary.x / 2, spawnBoundary.x / 2);

            float distanceZ =
            Random.Range(-spawnBoundary.y / 2, spawnBoundary.y / 2);

            Vector3 spawnPoint = (transform.position + Vector3.up * 5f) + new Vector3(distanceX, 0, distanceZ);

            Transform t = Instantiate(actor, spawnPoint, Quaternion.identity);
            transformAccessArray.Add(t);
        }

        
    }

    void Update()
    {
        positionUpdateJob = new PositionUpdateJob()
        {
            objectVelocities = velocities,
            jobDeltaTime = Time.deltaTime,
            swimSpeed = this.moveSpeed,
            turnSpeed = this.turnSpeed,
            time = Time.time,
            swimChangeFrequency = this.swimChangeFrequency,
            center = Vector3.zero,
            bounds = spawnBoundary,
            seed = System.DateTimeOffset.Now.Millisecond
        };

        positionUpdateJobHandle = positionUpdateJob.Schedule(transformAccessArray);
    }

    void LateUpdate()
    {
        positionUpdateJobHandle.Complete();
    }

    private void OnDestroy()
    {
        if (!enabled)
            return;

        transformAccessArray.Dispose();
        velocities.Dispose();
    }


    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> objectVelocities;

        public Vector2 bounds;
        public Vector3 center;

        public float jobDeltaTime;
        public float time;
        public float swimSpeed;
        public float turnSpeed;
        public int swimChangeFrequency;

        public float seed;

        public void Execute(int i, TransformAccess transform)
        {
            var currentVelocity = objectVelocities[i];
            random randomGen = new random((uint)(i * time + 1 + seed));

            transform.position += transform.localToWorldMatrix.MultiplyVector(new Vector3(0, 0, 1))*swimSpeed *jobDeltaTime *randomGen.NextFloat(0.3f, 1.0f);

            if (currentVelocity != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), turnSpeed * jobDeltaTime);
            }

            var currentPosition = transform.position;
            var randomise = true;

            if (currentPosition.x > center.x + bounds.x / 2 ||
                currentPosition.x < center.x - bounds.x / 2 ||
                currentPosition.z > center.z + bounds.y / 2 ||
                currentPosition.z < center.z - bounds.y / 2)
            {
                var internalPosition = new Vector3(center.x + randomGen.NextFloat(-bounds.x / 2, bounds.x / 2) / 1.3f, 0, center.z + randomGen.NextFloat(-bounds.y / 2, bounds.y / 2) / 1.3f);

                currentVelocity = (internalPosition - currentPosition).normalized;
                objectVelocities[i] = currentVelocity;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), turnSpeed * jobDeltaTime * 2);

                randomise = false;
            }

            if (randomise)
            {
                if (randomGen.NextInt(0, swimChangeFrequency) <= 2)
                {
                    objectVelocities[i] = new Vector3(randomGen.NextFloat(-1f, 1f),
                    0, randomGen.NextFloat(-1f, 1f));
                }
            }
        }
    }

}
