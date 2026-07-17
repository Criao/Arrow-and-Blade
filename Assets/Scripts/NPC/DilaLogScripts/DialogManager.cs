using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    public bool isDilaogueActive;

    [Header("Quest System")]
    public Accept acceptScript;

    private DialogueSO currentDialogue;
    private int dialogueIndex;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        HideCanvas();

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        if (!IsDialogueValid(dialogueSO))
        {
            Debug.LogWarning("Cannot start dialogue because the dialogue data is missing or has no lines.");
            EndDialogue();
            return;
        }

        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDilaogueActive = true;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        ShowCanvas();
        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (!isDilaogueActive || currentDialogue == null)
        {
            return;
        }

        if (dialogueIndex < currentDialogue.lines.Length)
        {
            ShowDialogue();
            return;
        }

        if (currentDialogue.hasQuest)
        {
            ShowQuestChoices();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        isDilaogueActive = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (acceptScript != null)
        {
            acceptScript.HideChoices();
        }

        HideCanvas();
        currentDialogue = null;
        dialogueIndex = 0;
    }

    public void StartCloseAnimation(Animator anim, GameObject obj)
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        StartCoroutine(CloseAndHide(anim, obj));
    }

    private void ShowDialogue()
    {
        if (currentDialogue == null || dialogueIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[dialogueIndex];
        if (line == null)
        {
            dialogueIndex++;
            AdvanceDialogue();
            return;
        }

        if (portrait != null)
        {
            portrait.sprite = line.speaker != null ? line.speaker.portrait : null;
        }

        if (actorName != null)
        {
            actorName.text = line.speaker != null ? line.speaker.actorName : "";
        }

        if (dialogueText != null)
        {
            dialogueText.text = line.text;
        }

        DialogueAudio.PlaySpeaker(line.speaker);
        dialogueIndex++;
    }

    private void ShowQuestChoices()
    {
        if (acceptScript == null)
        {
            Debug.LogError("Accept reference is missing on DialogueManager.");
            EndDialogue();
            return;
        }

        acceptScript.ShowChoices(currentDialogue);
    }

    private bool IsDialogueValid(DialogueSO dialogueSO)
    {
        return dialogueSO != null && dialogueSO.lines != null && dialogueSO.lines.Length > 0;
    }

    private void ShowCanvas()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideCanvas()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator CloseAndHide(Animator anim, GameObject obj)
    {
        if (anim != null && anim.gameObject.activeInHierarchy)
        {
            anim.Play("Close");
            yield return new WaitForSeconds(0.5f);
        }

        if (obj != null)
        {
            obj.SetActive(false);
        }
    }
}

public static class DialogueAudio
{
    private const string ResourceRoot = "Audio/SFX/Dialogue/";
    private const string DefaultNpcClipName = "npc_talk_blip_01";

    private static readonly Dictionary<string, AudioClip> Clips = new Dictionary<string, AudioClip>();

    public static void PlaySpeaker(ActorSo speaker)
    {
        if (speaker == null || speaker.isPlayerActor)
        {
            return;
        }

        string clipName = string.IsNullOrEmpty(speaker.voiceClipName) ? DefaultNpcClipName : speaker.voiceClipName;
        Play(clipName, speaker.voiceVolume);
    }

    private static void Play(string clipName, float volume)
    {
        if (string.IsNullOrEmpty(clipName) || volume <= 0f)
        {
            return;
        }

        AudioClip clip = LoadClip(clipName);
        if (clip == null)
        {
            return;
        }

        GameObject audioObject = new GameObject("OneShot Dialogue - " + clipName);
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = Mathf.Clamp01(volume);
        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.Play();

        Object.Destroy(audioObject, clip.length + 0.1f);
    }

    private static AudioClip LoadClip(string clipName)
    {
        if (Clips.TryGetValue(clipName, out AudioClip cachedClip))
        {
            return cachedClip;
        }

        AudioClip clip = Resources.Load<AudioClip>(ResourceRoot + clipName);
        Clips[clipName] = clip;

        if (clip == null)
        {
            Debug.LogWarning("Missing dialogue audio clip: " + ResourceRoot + clipName);
        }

        return clip;
    }
}
