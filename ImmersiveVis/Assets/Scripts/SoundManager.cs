using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource envSound;
    public AudioSource rainSound;

    public void ShouldPlayRain(bool should) {
        if(should) { rainSound.Play(); }
        else { rainSound.Stop(); }
    }

    public void ShouldPlayRemoteEnv(bool should) {
        if(should) { envSound.Play(); }
        else { envSound.Stop(); }
    }
}
