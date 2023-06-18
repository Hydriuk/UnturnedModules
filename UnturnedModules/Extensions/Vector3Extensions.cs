using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Hydriuk.UnturnedModules.Extensions
{
    public static class Vector3Extensions
    {
        public static float[] Serialize(this Vector3 vector)
        {
            return new float[] { vector.x, vector.y, vector.z };
        }

        public static Vector3 Deserialize(this float[] vector)
        {
            return new Vector3(vector[0], vector[1], vector[2]);
        }
    }
}
