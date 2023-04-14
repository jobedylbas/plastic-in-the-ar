using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnvironmentType {
    Default = 0,
    Urban,
    Remote
}

public enum EnvironmentWeather {
    Default = 0,
    Rain,
    Snow
}

public enum EnvironmentTime {
    Now = 0,
    Future
}

public class Environment {
    public EnvironmentType type = EnvironmentType.Default;
    public EnvironmentWeather weather = EnvironmentWeather.Default;
    public EnvironmentTime time = EnvironmentTime.Now;

    public Environment() { }

    public Environment(EnvironmentType type, EnvironmentWeather weather, EnvironmentTime time) {
        this.type = type;
        this.weather = weather;
        this.time = time;
    }
}