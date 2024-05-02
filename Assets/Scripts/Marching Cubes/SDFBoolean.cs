using UnityEngine;

public class SDFBoolean : SDFObject
{
    public SDFObject obj1;
    public SDFObject obj2;

    public float smoothnessFactor;


    SDFObject[] children;

    public void Start()
    {
        children = new SDFObject[] {obj1, obj2};
    }

    public JoinType joinType;
    public override (Vector3, Vector3) Bounds()
    {
        (Vector3 obj1Min, Vector3 obj1Max) = obj1.Bounds();
        (Vector3 obj2Min, Vector3 obj2Max) = obj2.Bounds();

        return (Vector3.Min(obj1Min, obj2Min), Vector3.Max(obj1Max, obj2Max));  
    }

    private float SmoothUnion(float d1, float d2, float k)
    {
        float h = Mathf.Max(k - Mathf.Abs(d1 - d2), 0.0f);
        return Mathf.Min(d1, d2) - h * h * 0.25f / k;
    }

    public override float Evaluate(Vector3 point)
    {
        float obj1Dist = obj1.Evaluate(point);
        float obj2Dist = obj2.Evaluate(point);

        if (joinType == JoinType.Union)
        {
            return SmoothUnion(obj1Dist, obj2Dist, smoothnessFactor);

        }
        else if (joinType == JoinType.Intersection)
        {
            return -SmoothUnion(-obj1Dist, -obj2Dist, smoothnessFactor);
        }
        else if (joinType == JoinType.Difference)
        {
            return -SmoothUnion(-obj1Dist, obj2Dist, smoothnessFactor);

        }
        else
        {
            return 0;
        }
    }

    
}

public enum JoinType {
    Union,
    Intersection,
    Difference
}
