using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

    // Don't bother changing this in the inspector. Like you have a choice
    public int Health = 10;
    public HealthDisc Disc;

    public float InvicibleTime = 2.0f;
    public float FlickersPerSecond = 45.0f;

    [HideInInspector]
    public bool PermanentInvincibility;

    public int CurrentHealth { get; private set; }

    private Component[] renderers;
    private bool invincible;
    private PlayerSound sound;
    private float lastHealTime;

    // Startup (health, invicible)
    void Start() {
        CurrentHealth = Health;
        invincible = false;
        renderers = GetComponent<PlayerMachine>().AnimatedMesh.GetComponentsInChildren(typeof(Renderer));
        sound = GetComponent<PlayerSound>();
    }

    /// <summary>
    /// Health functions (heal,damage)
    /// </summary>

    public void TakeDamage(int damage) {
        CurrentHealth = (int)Mathf.Clamp(CurrentHealth - damage, 0, Health);

        Disc.UpdateDisc(CurrentHealth);

        invincible = true;

        Disc.Maximize();
    }

    public void AddHealth(int health) {
        if (CurrentHealth != Health)
            sound.PlayGetLife();

        // Are we fully healing?
        if (CurrentHealth != Health && CurrentHealth + health >= Health)
            Disc.Minimize();

        CurrentHealth = (int)Mathf.Clamp(CurrentHealth + health, 0, Health);

        Disc.UpdateDisc(CurrentHealth);
    }

    /// <summary>
    /// ISpid Invincible routine
    /// used for recover
    /// </summary>

    public void StartInvincible() {
        if (invincible)
            StartCoroutine(Invicibility());
    }

    public bool Invincible() {
        if (PermanentInvincibility)
            return true;
        else
            return invincible;
    }

    public void EndInvincible() {
        StopAllCoroutines();
        invincible = false;
    }

    // Make ISpid flicker
    private IEnumerator Invicibility() {
        foreach (Renderer renderer in renderers) {
            renderer.enabled = false;
        }

        float i = 0;
        float lastFlicker = 0;
        float flickerFrequency = InvicibleTime / FlickersPerSecond;

        while (i < InvicibleTime) {
            if (i > lastFlicker + flickerFrequency) {
                foreach (Renderer renderer in renderers) {
                    renderer.enabled = !renderer.enabled;
                }

                lastFlicker = i;
            }

            i += Time.deltaTime;

            yield return 0;
        }

        foreach (Renderer renderer in renderers) {
            renderer.enabled = true;
        }

        invincible = false;

        yield return null;
    }
}
