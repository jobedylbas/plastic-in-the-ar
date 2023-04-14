using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MicroplasticType {
    Fiber,
    Film,
    Granule,
    Fragment
}

public class Microplastic {
    public MicroplasticType type;
    public float minSize, maxSize;

    public Microplastic(MicroplasticType type, float minSize, float maxSize) {
        this.type = type;
        this.minSize = minSize;
        this.maxSize = maxSize;
    }
}
