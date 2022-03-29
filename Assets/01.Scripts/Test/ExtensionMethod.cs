using UnityEngine;

namespace TheGreatGGM
{
    public static class ExtensionMethod
    {
        public static float GGM(this Transform trm)
        {
            Vector3 pos = trm.position;
            return pos.x + pos.y + pos.z;
        }
    }
}
