using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Arkanoid
{
    public static class UIExtensions
    {
        public static T Nullify<T>(this T obj) where T : UnityEngine.Object
        {
            return obj == null ? null : obj;
        }

        public static void DestroyChilds(this Transform transform)
        {
            foreach (Transform child in transform)
                UnityEngine.Object.Destroy(child.gameObject);
        }
    }
}
