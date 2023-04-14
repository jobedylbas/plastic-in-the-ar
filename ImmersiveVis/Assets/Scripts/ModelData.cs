using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleData
{
    public string location;
    public string condition;
    public string microplastic_type;
    public int min_size;
    public int max_size;
    public double proportion;
    public double freq;
}

[System.Serializable]
public class ParticleDataList {
    public ParticleData[] particles;
}

[System.Serializable]
public class ObjectData
{
    public string objectName;
    public string color;
    public string type;
    public string composition;
    public int minSize;
    public int maxSize;
}

[System.Serializable]
public class ObjectDataList {
    public ObjectData[] objects;
}