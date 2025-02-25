using System;

namespace Shared
{
    [Serializable]

    public class ThingData
    {
        public string defName;
        public string materialDefName;
        public int quantity;
        public string quality;

        public bool isMinified;
        public int hitpoints;

        public string[] position;
        public int rotation;

        public float growthTicks;
    }
}