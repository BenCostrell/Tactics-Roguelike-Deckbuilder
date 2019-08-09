using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectRenderer : MonoBehaviour
{
    private SpriteRenderer sr;
    public int id { get; private set; }
    private Coroutine movementCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine deathCoroutine;
    private const float moveTimePerTile = 0.2f;
    private const float preAttackWait = 0.3f;
    private const float attackAnimationExtendDuration = 0.3f;
    private const float attackAnimationRetractDuration = 0.075f;
    private const float attackDistance = 0.3f;
    private const float deathAnimWhiteoutDuration = 0.2f;
    private const float deathAnimFadeoutDuration = 0.4f;
    public bool animating { get; private set; }
    public HealthBar healthBar;
    public SpriteMask whiteoutMask;
    public SpriteRenderer whiteoutSr;

    public void Initialize(GridObject gridObject, MapTile mapTile)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = gridObject.data.sprite;
        id = gridObject.id;
        gameObject.name = gridObject.data.gridObjectName;
        transform.localPosition = new Vector2(mapTile.coord.x, mapTile.coord.y);
        healthBar.Initialize(gridObject);
        whiteoutMask.sprite = sr.sprite;
        whiteoutSr.enabled = false;
        whiteoutMask.enabled = false;
        Services.EventManager.Register<GridObjectDeath>(OnDeath);
    }

    public void MoveToTile(GridObjectMoved e)
    {
        movementCoroutine = StartCoroutine(MovementCoroutine(e));
    }

    public void Attack(ObjectAttacked e)
    {
        attackCoroutine = StartCoroutine(AttackCoroutine(e));
    }

    public void OnDeath(GridObjectDeath e)
    {
        if (e.id != id) return;
        deathCoroutine = StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        while (!healthBar.doneAnimating)
        {
            yield return null;
        }
        animating = true;
        float timeElapsed = 0;
        whiteoutMask.enabled = true;
        whiteoutSr.enabled = true;
        whiteoutSr.color = new Color(1, 1, 1, 0);
        while(timeElapsed <= deathAnimFadeoutDuration + deathAnimWhiteoutDuration)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed <= deathAnimWhiteoutDuration)
            {
                whiteoutSr.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white,
                    EasingEquations.Easing.QuadEaseOut(timeElapsed / deathAnimWhiteoutDuration));
            }
            else
            {
                float alpha = Mathf.Lerp(1,0,EasingEquations.Easing.QuadEaseOut(
                    (timeElapsed - deathAnimWhiteoutDuration)/deathAnimFadeoutDuration));
                whiteoutSr.color = new Color(1, 1, 1, alpha);
                sr.color = new Color(1, 1, 1, alpha);
                healthBar.FadeColor(alpha);
            }
            yield return null;
        }
        animating = false;
        Destroy(gameObject);
    }

    IEnumerator AttackCoroutine(ObjectAttacked e)
    {
        animating = true;
        float timeElapsed = 0;
        Coord direction = Coord.Subtract(e.target.currentTile.coord, e.attacker.currentTile.coord);
        direction = new Coord(System.Math.Sign(direction.x), System.Math.Sign(direction.y));
        Vector3 attackDiff = new Vector3(direction.x, direction.y, 0).normalized * attackDistance;
        Vector3 basePos = transform.localPosition;
        Vector3 target = basePos + attackDiff;
        while (timeElapsed < preAttackWait + attackAnimationExtendDuration + attackAnimationRetractDuration)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed <= attackAnimationExtendDuration && timeElapsed > preAttackWait)
            {
                transform.localPosition = Vector3.Lerp(basePos, target,
                    EasingEquations.Easing.QuadEaseOut((timeElapsed - preAttackWait)
                    / attackAnimationExtendDuration));
            }
            else
            {
                transform.localPosition = Vector3.Lerp(target, basePos,
                    EasingEquations.Easing.QuadEaseIn(
                    (timeElapsed - attackAnimationExtendDuration - preAttackWait) /
                    attackAnimationRetractDuration));
            }
            yield return null;
        }
        Services.EventManager.Fire(new AttackAnimationComplete(e.target.id));
        animating = false;
    }

    IEnumerator MovementCoroutine(GridObjectMoved e)
    {
        float timeElapsed = 0;
        Coord prevCoord = e.originalTile.coord;
        Coord nextCoord = e.path[0].coord;
        int pathIndex = 0;
        bool done = false;
        animating = true;
        while(!done)
        {
            timeElapsed += Time.deltaTime;
            transform.localPosition = Vector2.Lerp(
                new Vector2(prevCoord.x, prevCoord.y),
                new Vector2(nextCoord.x, nextCoord.y),
                timeElapsed / moveTimePerTile);
            if(timeElapsed >= moveTimePerTile)
            {
                pathIndex += 1;
                if(pathIndex >= e.path.Count)
                {
                    done = true;
                }
                else
                {
                    prevCoord = nextCoord;
                    nextCoord = e.path[pathIndex].coord;
                    timeElapsed = 0;
                }
            }
            yield return null;
        }
        animating = false;
        Services.EventManager.Fire(new GridObjectMovementComplete(id));
    }
}

public class AttackAnimationComplete : GameEvent
{
    public readonly int id;

    public AttackAnimationComplete(int id_)
    {
        id = id_;
    }
}

public class GridObjectMovementComplete : GameEvent
{
    public readonly int id;
    public GridObjectMovementComplete(int id_)
    {
        id = id_;
    }
}
