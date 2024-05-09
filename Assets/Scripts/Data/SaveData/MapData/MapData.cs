using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SaveData
{
    [System.Serializable]
    public class MapData
    {
    }

    [System.Serializable]
    public class BiomeData
    {
        public Chunk[] chunk;
        public bool ocean;
        public byte biomeType;
    }
    [System.Serializable]
    public class Chunk
    {
        public int[] x;
        public int[] z;
    }
}
