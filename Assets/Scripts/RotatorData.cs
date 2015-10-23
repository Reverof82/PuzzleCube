using UnityEngine;
using System.Collections;

public class RotatorData : MonoBehaviour
{
    public enum Rotation { x, y, z, n };
    public Rotation rotationAxis;
    bool active;

    // Use this for initialization
    void Start()
    {
        active = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Rotation RotationAxis
    {
        get { return rotationAxis; }
        set { rotationAxis = value; }
    }

    public bool Active
    {
        get { return active; }
        set { active = value; }
    }
}
