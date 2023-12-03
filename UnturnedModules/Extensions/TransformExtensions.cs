using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Hydriuk.UnturnedModules.Extensions
{
    public static class TransformExtensions
    {
        public static BarricadeDrop? ToBarricade(this Transform transform)
        {
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(transform);

            if (drop != null)
                return drop;

            InteractableDoorHinge doorHinge = transform.GetComponent<InteractableDoorHinge>();

            return BarricadeManager.FindBarricadeByRootTransform(doorHinge?.door?.transform);
        }

        public static StructureDrop? ToStructure(this Transform transform)
        {
            return StructureManager.FindStructureByRootTransform(transform);
        }
    }
}
