using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public enum State {
    Intro,
    Experience,
    Dashboard
}

public class ExperienceManager: MonoBehaviour {
    [SerializeField] private CanvasGroup homescreen;
    public MainScreen mainScreenPrefab;
    public MainScreen mainScreen;
    bool didPressBegin = false;
    bool didDisappearHomeScreen = false;
    public bool didPressToggleData = false;
    public State state = State.Intro;
    public Environment env = new Environment();
    public ParticleManager particleManager;
    public SoundManager soundManager;
    public DataManager dataManager;

    public Camera cam;
    public ARSessionOrigin originObject;
    private float initialDistance;
    private bool isPlayingParticles = true;
    public void ChangeEnvironment(Environment newEnvironment) {
        this.env = newEnvironment;

        DidChangeEnv();
    }

    public void begin() {
        didPressBegin = true;
        ChangeEnvironment(new Environment());
    }

    void Start() {
        mainScreenPrefab.manager = this;
    }

    void Update() {
        if(didPressBegin && homescreen.alpha > 0) {
            homescreen.alpha -= Time.deltaTime * 1.5f;
        }
        if(homescreen.alpha <= 0 && !didDisappearHomeScreen) {
            didDisappearHomeScreen = true;
            mainScreen = Instantiate(mainScreenPrefab);
        }
        
        if(Input.touchCount == 1 && didPressBegin) { 
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began && !mainScreen.isShowingDashboard && touch.position.y >= 300 && touch.position.y <= 2000) {

                if(!isPlayingParticles) {  
                    // string[] layerNames = { "Sight", "OtherLayerName", "OneMoreLayerName" };

                    // RaycastHit hit;
                    // Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                    // if (Physics.SphereCast(ray, 0.1f, out hit, Mathf.Infinity)) {
                    //     Debug.Log(hit.transform.root.gameObject);
                    // }
                

                    // var pref = (GameObject)Resources.Load("Prefabs/SortingBall");
                    // var sortingBallPrefab = pref;
                    // GameObject sortingBall = Instantiate(sortingBallPrefab);
                    // sortingBall.transform.position = Camera.main.transform.position;
                    // // CheeseCollision sc = sortingBall.AddComponent<CheeseCollision>() as CheeseCollision;
                    // //set bullet direction
                    // Rigidbody rbSortingBall = sortingBall.GetComponent<Rigidbody>();
                    // rbSortingBall.AddForce(ray.direction * 10.0f);
                }
                else {
                    var defaultGravity = 9.81f;
                    var objData = dataManager.getRandomObject();
                    var objPrefab = (GameObject)Resources.Load("Prefabs/" + objData.objectName);                    
                    var obj = Instantiate(objPrefab, cam.transform.position, Quaternion.identity);
                    var position = cam.transform.position + (cam.transform.forward * 1.0f);
                    position.y = position.y - 0.1f;
                    obj.transform.localScale = obj.transform.localScale / 10;
                    var objPosition = obj.transform.position;
                    objPosition = position;
                    CollisionBehavior sc = obj.AddComponent<CollisionBehavior>() as CollisionBehavior;
                    sc.particleManager = this.particleManager;
                    sc.objectData = objData;
                    Rigidbody r = obj.GetComponent<Rigidbody>();
                    var direction = cam.transform.forward;
                    direction = RotateTowardsUp(direction, 45);
                    var gravity = defaultGravity * 0.9f * Vector3.up;
                    r.AddForce(-gravity);

                    r.AddForce(direction * 28.0f);
                }
            }

            if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0); 
            var touchOne = Input.GetTouch(1);

            // if one of the touches Ended or Canceled do nothing
            if(touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled  
            || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled) 
            {
                return;
            }

            // It is enough to check whether one of them began since we
            // already excluded the Ended and Canceled phase in the line before
            if(touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                // track the initial values
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                // initialScale = objectImRotating.transform.localScale;
            }
            // else now is any other case where touchZero and/or touchOne are in one of the states
            // of Stationary or Moved
            else
            {
                // otherwise get the current distance
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                // A little emergency brake ;)
                if(Mathf.Approximately(initialDistance, 0)) return;

                // get the scale factor of the current distance relative to the inital one
                var factor = (currentDistance / initialDistance);
                
                // apply the scale
                // instead of a continuous addition rather always base the 
                // calculation on the initial and current value only
                if(factor > 0) {
                    particleManager.ScaleParticles(factor * 0.001f);
                    mainScreen.SetScale(factor * 2000);
                }
                // cam.transform.position = cam.transform.position + (cam.transform.forward * factor);
                // objectImRotating.transform.localScale = initialScale * factor;
            }
            
        }
            
        }
    }

    Vector3 RotateTowardsUp(Vector3 start, float angle)
    {
        // if you know start will always be normalized, can skip this step
        start.Normalize();

        Vector3 axis = Vector3.Cross(start, Vector3.up);

        // handle case where start is colinear with up
        if (axis == Vector3.zero) axis = Vector3.right;

        return Quaternion.AngleAxis(angle, axis) * start;
    }

    public void DidTapOnWeather(int at) {
        this.env.weather = (EnvironmentWeather)at;
        particleManager.ShouldShowRain(at == 1);
        particleManager.ShouldShowSnow(at == 2);
        
        soundManager.ShouldPlayRain(at == 1);
        
        DidChangeEnv();
    }

    public void DidTapOnLocationButton(int at) {
        this.env.type = (EnvironmentType)at;
        soundManager.ShouldPlayRemoteEnv(at == 2);
        DidChangeEnv();
    }

    public void DidChangeEnv() {
        particleManager.ClearDefaultParticles();
        particleManager.ClearNewParticles();
        List<ParticleData> particles = dataManager.GetParticlesFor(env.weather.ToString(), env.type.ToString());
        
        foreach (ParticleData particle in particles)
        {   
            particleManager.CreateParticleSystemFor(particle);
        }
    }
    
    public float GetQuantityFor(int type = 0) {
        string microplasticType = MicroplasticType(type);
        return dataManager.GetQuantityFor(env.weather.ToString(), env.type.ToString(), microplasticType);
    }

    public float[] GetSizeFor(int type = 0) {
        string microplasticType = MicroplasticType(type);
        return dataManager.GetSizeFor(env.weather.ToString(), env.type.ToString(), microplasticType);
    } 

    public string MicroplasticType(int type = 0) {
        switch(type) {
        case 0: return "fibres";
        case 1: return "films";
        case 2: return "granules";
        default: return "fragments";
        }
    }

    public void StopParticles() {
        isPlayingParticles = false;
        particleManager.StopParticles();
        particleManager.GenerateTextForParticles();
    }

    public void PlayParticles() {
        isPlayingParticles = true;
        particleManager.PlayParticles();
    }
} 