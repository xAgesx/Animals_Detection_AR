using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ARContentManager : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject buttonsCanvas;
    public GameObject vidHolder;

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private AudioClip currentSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTargetFound(AudioClip sound, VideoClip video)
    {
        buttonsCanvas.SetActive(true);  // Affiche les boutons
        currentSound = sound;            // Stocke le son
        videoPlayer.clip = video;        // Prépare la vidéo
                                         // Arrête tout média en cours
        videoPlayer.Stop();
        vidHolder.SetActive(false);
        audioSource.Stop();
    }
    public void OnTargetLost()
    {
        buttonsCanvas.SetActive(false);  // Cache les boutons
        vidHolder.SetActive(false);      // Cache la vidéo
        audioSource.Stop();              // Arrête le son
        videoPlayer.Stop();              // Arrête la vidéo
    }
    public void ToggleSound()
    {
        if (audioSource.isPlaying && audioSource.clip == currentSound)
        {
            audioSource.Stop();  // Si le son joue → l'arrêter
        }
        else
        {
            audioSource.clip = currentSound;
            audioSource.loop = true;  // Répéter en boucle
            audioSource.Play();       // Jouer le son
        }
    }
    public void ToggleVideo()
    
    {   
        Debug.Log("Video Player : "+videoPlayer);
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();          // Si vidéo joue → l'arrêter
            vidHolder.SetActive(false);  // Cacher le panneau
        }
        else
        {
            vidHolder.SetActive(true);   // Afficher le panneau
            videoPlayer.Play();          // Jouer la vidéo
        }
    }

}
