using UnityEngine;

public class ActorMovement : MonoBehaviour
{
    public Vector3 objectVelocities;

    public Vector2 bounds;
    public Vector3 center;

    public float jobDeltaTime;
    public float time;
    public float moveSpeed;
    public float turnSpeed;
    public int moveChangeFrequency;

    public float seed;

    void Update()
    {        
        jobDeltaTime = Time.deltaTime;
        time = Time.time;
        seed = System.DateTimeOffset.Now.Millisecond;

        Move();
    }

    public void Move()
    {
        Vector3 currentVelocity = objectVelocities;

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
            objectVelocities = currentVelocity;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), turnSpeed * jobDeltaTime * 2);

            randomise = false;
        }

        if (randomise)
        {
            if (Random.Range(0, moveChangeFrequency) <= 2)
            {
                objectVelocities = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            }
        }
    }
}
