using UnityEngine;

public class GiantAsteroidController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 6f;

    public void SetTarget(Transform t) => target = t;

    private void Awake()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}

