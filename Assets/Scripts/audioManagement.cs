using UnityEngine.Audio;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class audioManagement : MonoBehaviour
{
    public Sound[] sounds;

    public static audioManagement instance;
    
    // Start is called before the first frame update
    void Awake(){

        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds){ 
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }  
    }

    void Start (){
         FindObjectOfType<audioManagement>().Play("backgroundMusic");
    }

    public void Play (string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: "+ name + " not found");
            return;
        }
        if (name.Equals("hitNPC3"))
        {
            float randPitch = Random.Range(0.95f, 1.3f);
            s.source.pitch = randPitch;
        }

        s.source.Play();
    }

    public void Pause (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: "+ name + " not found");
            return;
        }
        s.source.Stop();
    }
}
