using UnityEngine;

public class SDFSphere : SDFObject
{
    public float radius;
    

    public override (Vector3, Vector3) Bounds()
    {
        Vector3 min = new Vector3(-radius, -radius, -radius);
        Vector3 max = new Vector3(radius, radius, radius);

        min = Vector3.Scale(min, transform.localScale);
        max = Vector3.Scale(max, transform.localScale);
        min += transform.position;
        max += transform.position;
        return (min, max);
    }

    public override float  Evaluate(Vector3 pos)
	{
		


		Vector3 diff = transform.position - pos;

					// Set the value of this point in the terrainMap.
		return diff.magnitude - radius;

	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
