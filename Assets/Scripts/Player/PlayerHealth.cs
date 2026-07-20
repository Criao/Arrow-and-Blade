using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth ActiveInstance { get; private set; }

    [SerializeField] private Animator healthTextAnimator;

    private bool deathHandled;

    private void Awake()
    {
        if (ActiveInstance != null && ActiveInstance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        ActiveInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (ActiveInstance == this)
        {
            ActiveInstance = null;
        }
    }

    private void Start()
    {
        UpdateHealthUI();
        GameOverManager.EnsureInstance().RegisterPlayer(gameObject);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 && GameManager.IsSceneTransitionBlocked)
        {
            return;
        }

        if (StatsManager.Instance == null)
        {
            Debug.LogError("StatsManager.Instance is missing. Cannot change player health.");
            return;
        }

        StatsManager.Instance.UpdateHealth(amount);
    }

    public void HandleHealthChanged(int amount)
    {
        if (amount < 0)
        {
            HitFeedbackAudio.PlayPlayerHurt(transform.position);
        }

        if (healthTextAnimator != null)
        {
            healthTextAnimator.Play("TextUpdate");
        }

        UpdateHealthUI();

        Died();
    }

    public void ResetDeathState()
    {
        deathHandled = false;
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.RefreshHealthUI();
        }
    }

    private void Died()
    {
        if (deathHandled || StatsManager.Instance == null || StatsManager.Instance.CurrentHealth > 0)
        {
            return;
        }

        deathHandled = true;
        StatsManager.Instance.CurrentHealth = 0;

        PlayerMoveMent movement = GetComponent<PlayerMoveMent>();
        if (movement != null)
        {
            movement.SetDead();
        }

        GameOverManager gameOverManager = GameOverManager.EnsureInstance();
        gameOverManager.ShowGameOver();

        gameObject.SetActive(false);
    }
}

public static class HitFeedbackAudio
{
    private const string ResourceRoot = "Audio/SFX/HitFeedback/";
    private static readonly Dictionary<string, AudioClip> Clips = new Dictionary<string, AudioClip>();

    public static void PlayEnemyHit(Vector3 position)
    {
        Play("enemy_hit_01", position, 0.75f);
    }

    public static void PlayPlayerHurt(Vector3 position)
    {
        Play("player_hurt_01", position, 0.8f);
    }

    public static void PlayKnockbackImpact(Vector3 position)
    {
        Play("knockback_impact_01", position, 0.75f);
    }

    public static void PlayLightHitConfirm(Vector3 position)
    {
        Play("hit_confirm_light_01", position, 0.6f);
    }

    private static void Play(string clipName, Vector3 position, float volume)
    {
        AudioClip clip = LoadClip(clipName);
        if (clip == null)
        {
            return;
        }

        GameObject audioObject = new GameObject("OneShot SFX - " + clipName);
        audioObject.transform.position = position;

        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
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
            Debug.LogWarning("Missing hit feedback audio clip: " + ResourceRoot + clipName);
        }

        return clip;
    }
}
