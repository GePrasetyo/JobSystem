using UnityEngine;
using System.Collections.Generic;
using System;
using random = Unity.Mathematics.Random;

public class PositionUpdateJob : JobThread
{
    public List<Vector3> ObjectVelocities;

    public Vector2 Bounds;
    public Vector3 Center;

    public float JobDeltaTime;
    public float Time;
    public float MoveSpeed;
    public float TurnSpeed;
    public int MoveChangeFrequency;

    public float Seed;    

    public Action<Vector3[], Vector3[]> _return;

    protected override void ThreadFunction()
    {        
        for (int i = 0; i < ObjectVelocities.Count; i++)
        {
            Vector3 currentVelocity = ObjectVelocities[i];

            transform.position += transform.localToWorldMatrix.MultiplyVector(new Vector3(0, 0, 1)) * moveSpeed * jobDeltaTime * Random.Range(0.3f, 1.0f);

            if (currentVelocity != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), turnSpeed * jobDeltaTime);
            }

            Vector3 currentPosition = transform.position;

            var randomise = true;

            if (currentPosition.x > Center.x + Bounds.x / 2 || currentPosition.x < Center.x - Bounds.x / 2 || currentPosition.z > Center.z + Bounds.y / 2 || currentPosition.z < Center.z - Bounds.y / 2)
            {
                Vector3 internalPosition = new Vector3(Center.x + random.Range(-Bounds.x / 2, Bounds.x / 2) / 1.3f, 0, Center.z + Random.Range(-Bounds.y / 2, Bounds.y / 2) / 1.3f);

                currentVelocity = (internalPosition - currentPosition).normalized;
                ObjectVelocities[i] = currentVelocity;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), TurnSpeed * JobDeltaTime * 2);

                randomise = false;
            }

            if (randomise)
            {
                if (Random.Range(0, MoveChangeFrequency) <= 2)
                {
                    ObjectVelocities[i] = new Vector3(System.Random Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                }
            }
        }
    }
    protected override void OnFinished()
    {
        
    }
}
