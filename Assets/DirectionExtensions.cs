using System;
using UnityEngine;

namespace DefaultNamespace
{
    public static class DirectionExtensions
    {
        public static Vector3 GetVector(this Direction direction)
        {
            return direction switch
            {
                Direction.up => Vector3.up,
                Direction.down => Vector3.down,
                Direction.right => Vector3.right,
                Direction.left => Vector3.left,
                Direction.backwards => Vector3.back,
                Direction.forward => Vector3.forward,
                _ => throw new Exception("Invalid input direction")
            };
        }
    }
}