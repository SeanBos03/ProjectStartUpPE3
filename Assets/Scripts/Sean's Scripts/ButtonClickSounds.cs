using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ButtonClickSounds : MonoBehaviour
{
    public List<Button> theButtons;
    public GameObject audioSourceButtonsObject;
    public GameObject audioSourceMusicObject;
    public GameObject audioSourcebuttonsHoverObject;
    public AudioClip soundClipclick;
    AudioSource audioSourceButtons;
    public AudioClip soundClipclickHover;
    AudioSource audioSourceButtonsHover;
    public AudioClip soundClipMusic;
    AudioSource audioSourceMusic;
    void Start()
    {
        audioSourceMusic = audioSourceMusicObject.GetComponent<AudioSource>();
        audioSourceMusic.clip = soundClipMusic;
        audioSourceMusic.Play();
        audioSourceButtons = audioSourceButtonsObject.GetComponent<AudioSource>();
        audioSourceButtons.clip = soundClipclick;
        audioSourceButtonsHover = audioSourcebuttonsHoverObject.GetComponent<AudioSource>();
        audioSourceButtonsHover.clip = soundClipclickHover;

        foreach (Button button in  theButtons)
        {
            button.onClick.AddListener(() => PlayClickSound());
        }

        foreach (Button button in theButtons)
        {
            EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((data) => PlayHoverSound());
            eventTrigger.triggers.Add(entry);

            
        }
    }

    void PlayHoverSound()
    {
        audioSourceButtonsHover.Play();
    }

    void PlayClickSound()
    {
        audioSourceButtons.Play();
    }
}