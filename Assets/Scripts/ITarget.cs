using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    Plane GetPlane ();
    bool Raycast (Ray ray, out Vector3 normal);
}
