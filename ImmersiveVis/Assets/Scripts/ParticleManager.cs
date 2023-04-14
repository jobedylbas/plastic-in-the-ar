using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject prefabParticleSystem;
    public GameObject rainPrefab;
    public GameObject rain;
    public GameObject snowPrefab;
    public GameObject snow;
    public List<GameObject> defaultParticleSystems = new List<GameObject>();
    public List<GameObject> particleSystems = new List<GameObject>();
    public Material ropeMaterial;
    public GameObject textAboutParticle;
    ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[10000];
    public List<GameObject> textObjects = new List<GameObject>();

    public void ClearNewParticles() {
        foreach(GameObject particleSystem in particleSystems) {
            Destroy(particleSystem);
        }

        this.particleSystems = new();
    }

    public void ClearDefaultParticles() {
        ClearTexts();
        foreach(GameObject particleSystem in defaultParticleSystems) {
            Destroy(particleSystem);
        }

        this.defaultParticleSystems = new();
    }

    public void ClearTexts() {
        foreach(GameObject textObject in textObjects) {
            Destroy(textObject);
        }

        this.textObjects = new();
    }

    public void ShouldShowRain(bool should) {
        var ps = rain.transform.GetChild(0).GetComponent<ParticleSystem>();
        if(should) {
            ps.Play();
        } else {
            ps.Stop();
        }
    }

    public void ShouldShowSnow(bool should) {
        var ps = snow.transform.GetChild(0).GetComponent<ParticleSystem>();
        if(should) {
            ps.Play();
        } else {
            ps.Stop();
        }
    }


    public void CreateParticleSystemFor(ParticleData particle) {
        var position = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(0.5f, 1.8f), Random.Range(-3.0f, 3.0f));
        var gs = Instantiate(prefabParticleSystem, transform.position, Quaternion.identity);
        gs.transform.position = position;
        
        var ps = gs.transform.GetChild(0).GetComponent<ParticleSystem>();
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        var emission = ps.emission;
        emission.rateOverTime = (float)(particle.freq * particle.proportion) * 15;
        var main = ps.main;
        main.startSize = new ParticleSystem.MinMaxCurve(particle.min_size/5000.0f, particle.max_size/5000.0f);
        psr.material.color = GetRandomColor();
        main.startLifetime = 15.0f;
        var shape = ps.shape;
        shape.radius = 1.0f;

        switch(particle.microplastic_type) {
        case "fragments":
            gs.tag = "Fragment";
            psr.mesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
            break;
        case "granules":
            gs.tag = "Granule";
            psr.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            break;
        case "films":
            gs.transform.tag = "Film";
            psr.mesh = ParticleManager.CreateDoubleSidedTriangle();
            break;
        default:
            gs.tag = "Fibre";
            main.startSize3D = true;
            main.startSizeX = particle.max_size/25000.0f;
            main.startSizeY = new ParticleSystem.MinMaxCurve(particle.min_size/5000.0f, particle.max_size/5000.0f);
            main.startSizeZ = particle.max_size/25000.0f;

            psr.mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            psr.material = ropeMaterial;
            break;
        }

        defaultParticleSystems.Add(gs);

        // InvokeRepeating("ChangeColor", 0.5f, 0.5f);
    }

    public void CreateParticleSystemFor(ObjectData objectData, Vector3 at) {
        var gs = Instantiate(prefabParticleSystem, at, Quaternion.identity);
        
        var ps = gs.transform.GetChild(0).GetComponent<ParticleSystem>();
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        var emission = ps.emission;
        var main = ps.main;
        main.loop = false;
        main.startLifetime = 5.0f;
        main.startSize = new ParticleSystem.MinMaxCurve(objectData.minSize/5000.0f, objectData.maxSize/5000.0f);
        switch(objectData.type) {
        case "Fragment":
            psr.mesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
            psr.material.color = GetColorBy(objectData.color);
            break;
        case "Granule":
            psr.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            psr.material.color = GetColorBy(objectData.color);
            
            break;
        case "Film":
            psr.mesh = ParticleManager.CreateDoubleSidedTriangle();
            psr.material.color = GetColorBy(objectData.color);
            break;
        default:
            main.startSize3D = true;
            main.startSizeX = objectData.maxSize/25000.0f;
            main.startSizeY = new ParticleSystem.MinMaxCurve(objectData.minSize/5000.0f, objectData.maxSize/5000.0f);
            main.startSizeZ = objectData.maxSize/25000.0f;

            psr.mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            psr.material = ropeMaterial;
            break;
        }

        particleSystems.Add(gs);
    }

    public void ScaleParticles(float value) {
        foreach(GameObject particleSystem in defaultParticleSystems) {
            var ps = particleSystem.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps.transform.localScale *= value;
        }
    }

    void Start() {
        rain = Instantiate(rainPrefab);
        var ps = rain.transform.GetChild(0).GetComponent<ParticleSystem>();
        rain.transform.position = new Vector3(0.0f, 4.0f, 0.0f);
        ps.Stop();

        snow = Instantiate(snowPrefab);
        ps = snow.transform.GetChild(0).GetComponent<ParticleSystem>();
        snow.transform.position = new Vector3(0.0f, 4.0f, 0.0f);
        ps.Stop();

        // InvokeRepeating("ClearNewParticles", 15.0f, 15.0f);
    }

    void Update() {
        foreach(GameObject textObject in textObjects) {
            textObject.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private static Mesh CreateTriangleMesh() {
         Vector3[] vertices = {
             new Vector3(-1.0f, -1.0f, 0),
             new Vector3(1.0f, -1.0f, 0),
             new Vector3(0f, 1.0f, 0)
         };
 
         Vector2[] uv = {
             new Vector2(0, 0),
             new Vector2(1, 0),
             new Vector2(0.5f, 1)
         };
 
         int[] triangles = { 0, 1, 2 };
 
         var mesh = new Mesh();
         mesh.vertices = vertices;
         mesh.uv = uv;
         mesh.triangles = triangles;
         mesh.RecalculateBounds();
         mesh.RecalculateNormals();
         mesh.RecalculateTangents();

         return mesh;
     }

    private static Mesh CreateDoubleSidedTriangle() {
        var mesh = new Mesh();
        Vector3 p0 = new Vector3(0,0,0);
        Vector3 p1 = new Vector3(1,0,0);
        Vector3 p2 = new Vector3(0.5f,0,Mathf.Sqrt(0.75f));
        Vector3 p3 = new Vector3(0.5f,Mathf.Sqrt(0.75f),Mathf.Sqrt(0.75f)/3);
        
        mesh.vertices = new Vector3[]{p0,p1,p2,p3};
        mesh.triangles = new int[]{
            0,1,2,
            0,2,3,
            2,1,3,
            0,3,1
        };
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }


    private Color GetRandomColor() {
        Color[] colors = { Color.blue, Color.cyan, Color.green, Color.magenta, Color.red };
        var index = Random.Range(0, 5);
        return colors[index];
     }

    public void StopParticles() {
        ClearNewParticles();
        foreach(GameObject particleSystem in defaultParticleSystems) {
            var ps = particleSystem.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps.Pause();
        }
    }

    public void PlayParticles() {
        ClearTexts();
        ClearNewParticles();
        foreach(GameObject particleSystem in defaultParticleSystems) {
            var ps = particleSystem.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps.Play();
        }
    }

    private Color GetColorBy(string name) {
        switch(name) {
        case "red": return Color.red;
        case "blue": return Color.blue;
        case "green": return Color.green;
        case "magenta": return Color.magenta;
        case "cyan": return Color.cyan;
        case "black": return Color.black;
        case "yellow": return Color.yellow;
        default: return Color.white;
        }
    }

    public void GenerateTextForParticles() {
        foreach(GameObject m_System in defaultParticleSystems) {
            var ps = m_System.transform.GetChild(0).GetComponent<ParticleSystem>();
            var psr = ps.GetComponent<ParticleSystemRenderer>();
            var color = psr.material.color;
            int numParticlesAlive = ps.GetParticles(m_Particles);
            for (int i = 0; i < numParticlesAlive; i++)
            {
                GameObject textAbout = Instantiate(textAboutParticle);
                var transform = textAbout.transform;
                var pos = m_Particles[i].position;
                float size = m_Particles[i].GetCurrentSize(ps);
                transform.localScale *= 0.009f;
                pos.y += 0.5f;
                transform.position = pos;
                textAbout.transform.rotation = Camera.main.transform.rotation;

                var desc = textAbout.GetComponent<AboutParticle>();

                desc.title.text = m_System.tag;

                desc.from.text = GetComposition(m_System.tag);

                desc.type.text = GetPossiblyFrom(m_System.tag);

                desc.size.text = (int)(size * 5000) + " μm";

                if(m_System.tag == "Fibre") {
                    desc.title.color = Color.yellow;
                    desc.from.color = Color.yellow;
                    desc.type.color = Color.yellow;
                    desc.size.color = Color.yellow;
                } else {
                    desc.title.color = color;
                    desc.from.color = color;
                    desc.type.color = color;
                    desc.size.color = color;
                }

                this.textObjects.Add(textAbout);
            }
        }
    }

    public string GetComposition(string type) {
        string[] fibers = { "Polyester", "Nylon"};
        string[] fragments = { "Polycarbonate", "High-density Polyethylene", "Polyvinyl chloride", "Nylon", "Acrylonitrile butadiene styrene"};
        string[] films = {"Polyethylene", "Low-density Polyethylene", "Polyethylene terephthalate", "Polyvinylidene chloride"};
        string[] granules = { "Polyethylene", "High-density Polyethylene", "Low-density Polyethylene", "Polypropylene", "Polyurethane", "Polyvinyl chloride"};

        switch(type) {
        case "Fragment":
            return fragments[Random.Range(0, fragments.Length)];
            break;
        case "Granule":
            return granules[Random.Range(0, granules.Length)];
            break;
        case "Film":
            return films[Random.Range(0, films.Length)];
            break;
        default:
            return fibers[Random.Range(0, fibers.Length)];
            break;
        }
    }

    public string GetPossiblyFrom(string type) {
        string[] fibers = { "Toothbrush bristles", "Fishing line", "Fishing net", "Sweater"};
        string[] fragments = { "Engine", "CD", "Eyeframes", "Milk jug", "Tupperware", "Computer keyboard", "Computer printer"};
        string[] films = {"Supermarket bag", "Shower curtain", "Soda bottle", "Drinking straw", "Plastic wrap" };
        string[] granules = { "Plastic bottle", "atio furniture", "Detergent bottle", "Bottle cap", "Yoghurt container", "Car bumper", "Tupperware","Disposable cup", "Phone case", "Car tyre"};

        switch(type) {
        case "Fragment":
            return fragments[Random.Range(0, fragments.Length)];
            break;
        case "Granule":
            return granules[Random.Range(0, granules.Length)];
            break;
        case "Film":
            return films[Random.Range(0, films.Length)];
            break;
        default:
            return fibers[Random.Range(0, fibers.Length)];
            break;
        }
    }
}
