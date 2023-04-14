using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager: MonoBehaviour
{
    public ParticleDataList particleDataList;
    public ObjectDataList objectDataList;
    public TextAsset jsonFile;
    public TextAsset objectFile;

    void Start() {
        Debug.Log("Loading data");
        particleDataList = JsonUtility.FromJson<ParticleDataList>(this.jsonFile.text);
        objectDataList = JsonUtility.FromJson<ObjectDataList>(this.objectFile.text);
    }

    void Update() {

    }

    public List<ParticleData> GetParticlesFor(string condition = "", string location = "") {
        List<ParticleData> _list = new();
        _list.AddRange(this.particleDataList.particles);
        List<ParticleData> filtered = _list.FindAll(particle => (particle.condition == condition && particle.location == location));

        return filtered;
    }

    public float GetQuantityFor(string condition = "", string location = "", string type = "") {
        List<ParticleData> _list = new();
        _list.AddRange(this.particleDataList.particles);
        return _list.Sum(particle => (particle.condition == condition && particle.location == location && particle.microplastic_type == type) ? (float)(particle.freq * particle.proportion) : 0.0f );
    }

    public float[] GetSizeFor(string condition = "", string location = "", string type = "") {
        double minSize = 150000;
        double maxSize = 0;
        List<ParticleData> _list = new();
        _list.AddRange(this.particleDataList.particles);
        foreach(ParticleData particle in _list) {
            if(particle.condition == condition && particle.location == location && particle.microplastic_type == type) {
                if(particle.min_size < minSize) { minSize = particle.min_size; }
                if(particle.max_size > maxSize) { maxSize = particle.max_size; }
            }
        }
        if(minSize == 150000) { minSize = 0; }
        float[] arr = { (float)minSize, (float)maxSize};
        return arr;
    }

    public ObjectData getRandomObject() {
        var index = Random.Range(0, objectDataList.objects.Length);
        return objectDataList.objects[index];
    }  
}
