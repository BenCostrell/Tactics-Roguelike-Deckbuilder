using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;

public class HealthBar : MonoBehaviour
{
    private int id;
    public SpriteRenderer frontBar;
    public SpriteRenderer backBar;
    public SpriteRenderer animatedBar;
    //private readonly Vector2 offset = new Vector2(-0.25f, 0.5f);
    private float animTimeRemaining;
    private const float damageAnimDuration = 0.25f;
    private float damageDistance;
    private float healthProportion;
    private Queue<HealthChangeAnimation> animQueue;
    private List<SpriteRenderer> srs;
    public bool doneAnimating { get { return animQueue.Count == 0 && animTimeRemaining <= 0; } }
    private TileRenderer lastTileHovered;

    public void Initialize(GridObject gridObject)
    {
        //gameObject.name = "Health Bar";
        id = gridObject.id;
        srs = new List<SpriteRenderer>() { frontBar, backBar, animatedBar };
        animQueue = new Queue<HealthChangeAnimation>();

        Services.EventManager.Register<DamageTaken>(OnDamageTaken);
        Services.EventManager.Register<AttackAnimationComplete>(OnAttackAnimationComplete);
        Services.EventManager.Register<InputHover>(OnInputHover);
        gameObject.SetActive(false);
        healthProportion = (float)gridObject.currentHealth/gridObject.maxHealth;
        frontBar.transform.localScale = new Vector3(healthProportion, 1, 1);
    }

    private void OnInputHover(InputHover e)
    {
        if (animTimeRemaining > 0) return;
        if (lastTileHovered == e.hoveredTile) return;
        lastTileHovered = e.hoveredTile;
        if (e.hoveredTile == null)
        {
            gameObject.SetActive(false);
            return;
        }
        List<GridObject> tileObjects = e.hoveredTile.tile.containedObjects;
        if (tileObjects.Count == 0 || tileObjects[0].id != id)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        AnimateDamage();
    }

    private void AnimateDamage()
    {
        if (animTimeRemaining > 0)
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
        gameObject.SetActive(true);
        Services.EventManager.Unregister<InputHover>(OnInputHover);
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

    public void FadeColor(float alpha)
    {
        foreach(SpriteRenderer sr in srs)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        }
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<DamageTaken>(OnDamageTaken);
        Services.EventManager.Unregister<AttackAnimationComplete>(OnAttackAnimationComplete);
        Services.EventManager.Unregister<InputHover>(OnInputHover);
    }
}
