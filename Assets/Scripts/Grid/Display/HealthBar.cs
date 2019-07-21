using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;

public class HealthBar : MonoBehaviour
{
    private int id;
    private SpriteRenderer frontBar;
    private SpriteRenderer backBar;
    private SpriteRenderer animatedBar;
    private readonly Vector2 offset = new Vector2(-0.25f, 0.5f);
    private float animTimeRemaining;
    private const float damageAnimDuration = 0.25f;
    private float damageDistance;
    private float healthProportion;
    private Queue<HealthChangeAnimation> animQueue;

    public void Initialize(GridObjectRenderer gridObjectRenderer, GridObject gridObject)
    {
        gameObject.name = "Health Bar";
        id = gridObject.id;
        transform.parent = gridObjectRenderer.transform;
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        transform.localPosition = offset;

        frontBar = new GameObject().AddComponent<SpriteRenderer>();
        frontBar.gameObject.name = "Front Bar";
        spriteRenderers.Add(frontBar);
        frontBar.sortingOrder = 2;
        frontBar.color = Color.green;

        backBar = new GameObject().AddComponent<SpriteRenderer>();
        backBar.gameObject.name = "Back Bar";
        spriteRenderers.Add(backBar);
        backBar.sortingOrder = 0;
        backBar.color = Color.red;

        animatedBar = new GameObject().AddComponent<SpriteRenderer>();
        animatedBar.gameObject.name = "Animated Bar";
        spriteRenderers.Add(animatedBar);
        animatedBar.sortingOrder = 1;
        animatedBar.color = Color.yellow;
        animatedBar.transform.localScale = new Vector3(0, 1, 1);

        foreach(SpriteRenderer sr in spriteRenderers)
        {
            sr.sortingLayerName = "UI";
            sr.sprite = Resources.Load<SpriteAtlas>("SpriteData/UiAtlas").GetSprite("bar");
            sr.transform.parent = transform;
            sr.transform.localPosition = Vector3.zero;
        }
        animQueue = new Queue<HealthChangeAnimation>();

        Services.EventManager.Register<DamageTaken>(OnDamageTaken);
        Services.EventManager.Register<AttackAnimationComplete>(OnAttackAnimationComplete);
    }

    // Update is called once per frame
    void Update()
    {
        AnimateDamage();
    }

    private void AnimateDamage()
    {
        if(animTimeRemaining > 0)
        {
            animTimeRemaining -= Time.deltaTime;
            float width = Mathf.Lerp(damageDistance, 0,
                EasingEquations.Easing.QuadEaseOut((damageAnimDuration - animTimeRemaining) / damageAnimDuration));
            animatedBar.transform.localScale = new Vector3(width, 1, 1);
        }
    }

    public void OnAttackAnimationComplete(AttackAnimationComplete e)
    {
        if (e.id != id) return;
        if (animQueue.Count == 0) return;
        HealthChangeAnimation healthChangeAnimation = animQueue.Dequeue();
        healthProportion = (float)healthChangeAnimation.health / healthChangeAnimation.maxHealth;
        damageDistance = frontBar.transform.localScale.x - healthProportion;
        frontBar.transform.localScale = new Vector3(healthProportion, 1, 1);
        animTimeRemaining = damageAnimDuration;
        animatedBar.transform.localScale = new Vector3(damageDistance, 1, 1);
        animatedBar.transform.localPosition = new Vector3(healthProportion / 2, 0, 0);
    }

    public void OnDamageTaken(DamageTaken e)
    {
        if (e.gridObject.id != id) return;
        animQueue.Enqueue(new HealthChangeAnimation(e.damage, e.gridObject.currentHealth,
            e.gridObject.maxHealth));
    }

    public class HealthChangeAnimation
    {
        public readonly int damage;
        public readonly int health;
        public readonly int maxHealth;

        public HealthChangeAnimation(int damage_, int health_, int maxHealth_)
        {
            damage = damage_;
            health = health_;
            maxHealth = maxHealth_;
        }
    }
}
