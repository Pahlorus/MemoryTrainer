using UnityEngine;

namespace Utility
{
    public static class PhysicsExtensions
    {
        public static readonly RaycastHit2D[] SHARED_HITS = new RaycastHit2D[16];
        public static readonly Collider2D[] SHARED_COLLISIONS = new Collider2D[16];

        public static int Cast(this Collider2D collider, ContactFilter2D contactFilter = default)
        {
            return collider.Cast(Vector2.zero, contactFilter, SHARED_HITS, 0, true);
        }

        public static ContactFilter2D WithLayerMask(this ContactFilter2D contactFilter, LayerMask layerMask)
        {
            contactFilter.SetLayerMask(layerMask);
            return contactFilter;
        }

        public static ContactFilter2D WithDepth(this ContactFilter2D contactFilter, float depthDelta)
        {
            return WithDepth(contactFilter, -depthDelta, depthDelta);
        }

        public static ContactFilter2D WithDepth(this ContactFilter2D contactFilter, float minDepth, float maxDepth)
        {
            contactFilter.SetDepth(minDepth, maxDepth);
            return contactFilter;
        }

        public static ContactFilter2D WithNormalAngles(this ContactFilter2D contactFilter, float normalAngleDelta)
        {
            return WithNormalAngles(contactFilter, -normalAngleDelta, normalAngleDelta);
        }

        public static ContactFilter2D WithNormalAngles(this ContactFilter2D contactFilter, float minNormalAngle, float maxNormalAngle)
        {
            contactFilter.SetNormalAngle(minNormalAngle, maxNormalAngle);
            return contactFilter;
        }

        public static ContactFilter2D WithTriggers(this ContactFilter2D contactFilter, float minDepth, float maxDepth)
        {
            contactFilter.useTriggers = true;
            return contactFilter;
        }
    }
}
