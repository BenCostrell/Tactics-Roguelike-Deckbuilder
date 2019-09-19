using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayer
{
    public TileRenderer[,] tileRenderers { get; private set; }
    private Dictionary<int, GridObjectRenderer> gridObjectRenderers;
    public Transform mapHolder { get; private set; }
    private readonly Vector2 centerOffset = new Vector2(0.5f, 2f);
    private List<GridObject> currentlyMovingObjects = new List<GridObject>();
    private Reticle reticle;
    private Queue<GameEvent> animationQueue;
    private bool wasAnimating;
    private GridObjectRenderer gridObjectRendererPrefab;
    private StateMachine<MapDisplayer> stateMachine;
    private NavArrow navArrow;
    public Card hoveredCard;
    public GridObject hoveredEnemy;

    public void InitializeMapDisplay(MapTile[,] map)
    {
        gridObjectRendererPrefab = Resources.Load<GridObjectRenderer>("Prefabs/GridObjectRenderer");
        mapHolder = new GameObject("MapHolder").transform;
        navArrow = GameObject.Instantiate<NavArrow>(Resources.Load<NavArrow>("Prefabs/NavArrow"), mapHolder);
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        tileRenderers = new TileRenderer[width, height];
        gridObjectRenderers = new Dictionary<int, GridObjectRenderer>();
        foreach (MapTile tile in map)
        {
            GameObject gameObject = new GameObject();
            TileRenderer tileRenderer = gameObject.AddComponent<TileRenderer>();
            tileRenderer.Init(tile, mapHolder);
            tileRenderers[tile.coord.x, tile.coord.y] = tileRenderer;
        }
        mapHolder.transform.position = new Vector2(-width / 2, -height / 2) + centerOffset;
        Services.EventManager.Register<GridObjectSpawned>(OnGridObjectSpawned);
        Services.EventManager.Register<GridObjectMoved>(OnGridObjectMoved);
        Services.EventManager.Register<ObjectAttacked>(OnObjectAttacked);
        Services.EventManager.Register<LevelCompleted>(OnLevelCompleted);
        foreach (MapTile tile in map)
        {
            if (tile.containedObject != null)
            {
                Services.EventManager.Fire(new GridObjectSpawned(tile.containedObject, tile));
            }
        }
        GameObject reticleObj = new GameObject();
        reticle = reticleObj.AddComponent<Reticle>();
        reticle.Init(mapHolder);
        animationQueue = new Queue<GameEvent>();
        stateMachine = new StateMachine<MapDisplayer>(this);
        stateMachine.InitializeState<PlayerRange>();
    }

    public void OnObjectAttacked(ObjectAttacked e)
    {
        animationQueue.Enqueue(e);
    }

    public void OnGridObjectMoved(GridObjectMoved e)
    {
        animationQueue.Enqueue(e);
    }

    public void OnLevelCompleted(LevelCompleted e)
    {
        animationQueue.Enqueue(e);
    }

    public void OnGridObjectSpawned(GridObjectSpawned e)
    {
        GridObjectRenderer gridObjectRenderer = GameObject.Instantiate(gridObjectRendererPrefab, 
            mapHolder).GetComponent<GridObjectRenderer>();
        gridObjectRenderer.Initialize(e.gridObject, e.mapTile);
        gridObjectRenderers[e.gridObject.id] = gridObjectRenderer;
    }

    private void ProcessAnimationQueue()
    {
        foreach (GridObjectRenderer renderer in gridObjectRenderers.Values)
        {
            if (renderer.animating) return;
        }
        if (animationQueue.Count == 0)
        {
            if (wasAnimating)
            {
                wasAnimating = false;
                Services.EventManager.Fire(new AllQueuedAnimationsComplete());
            }
            return;
        }
        GameEvent animationEvent = animationQueue.Dequeue();
        Type eventType = animationEvent.GetType();
        if (eventType == typeof(GridObjectMoved))
        {
            GridObjectMoved movement = animationEvent as GridObjectMoved;
            gridObjectRenderers[movement.gridObject.id].MoveToTile(movement);
            //Debug.Log("starting movement");
        }
        else if (eventType == typeof(ObjectAttacked))
        {
            ObjectAttacked attack = animationEvent as ObjectAttacked;
            gridObjectRenderers[attack.attacker.id].Attack(attack);
        }
        else if (eventType == typeof(LevelCompleted))
        {
            Services.EventManager.Fire(new StartLevelTransitionAnimation());
        }
        
        wasAnimating = true;
    }

    public void Update()
    {
        ProcessAnimationQueue();
        stateMachine.Update();
    }
}

public class AllQueuedAnimationsComplete : GameEvent { }

public abstract class MapDisplayState : StateMachine<MapDisplayer>.State
{
    protected Player player { get { return Services.LevelManager.player; } }

    public override void OnEnter()
    {
        base.OnEnter();
        Services.EventManager.Register<CardRendererHoverStart>(OnCardHoverStart);
    }

    public void OnCardHoverStart(CardRendererHoverStart e)
    {
        Context.hoveredCard = e.cardRenderer.card;
        TransitionTo<CardRange>();
    }



    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<CardRendererHoverStart>(OnCardHoverStart);

        foreach (TileRenderer tileRenderer in Context.tileRenderers)
        {
            tileRenderer.SetRangeColor(TileRenderer.RangeLevel.NONE);
        }
    }
}

public class EnemyRange : MapDisplayState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Services.EventManager.Register<InputHover>(OnInputHover);
        int moveRange = 0;
        int attackRange = 0;
        foreach(EnemyTurnBehavior enemyTurnBehavior in Context.hoveredEnemy.data.enemyTurnBehaviors)
        {
            if(enemyTurnBehavior is Attack)
            {
                Attack attack = enemyTurnBehavior as Attack;
                moveRange = attack.moveSpeed;
                attackRange = attack.range;
            }
        }
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(Context.hoveredEnemy.currentTile,
            moveRange, Context.hoveredEnemy, false, 0, true);
        bool inAnyRange = false;
        foreach (TileRenderer tileRenderer in Context.tileRenderers)
        {
            if(tileRenderer.tile == Context.hoveredEnemy.currentTile)
            {
                tileRenderer.SetRangeColor(TileRenderer.RangeLevel.NONE);
                continue;
            }
            if (tilesInRange.Contains(tileRenderer.tile))
            {
                tileRenderer.SetRangeColor(TileRenderer.RangeLevel.MOVE);
                inAnyRange = true;
            }
            else 
            {
                foreach(MapTile tileInRange in tilesInRange)
                {
                    if(Coord.Distance(tileInRange.coord, tileRenderer.tile.coord) <= attackRange)
                    {
                        tileRenderer.SetRangeColor(TileRenderer.RangeLevel.ATTACK);
                        inAnyRange = true;
                        break;
                    }
                }
            }
            if(!inAnyRange)
            {
                tileRenderer.SetRangeColor(TileRenderer.RangeLevel.NONE);
            }
        }
    }

    private void OnInputHover(InputHover e)
    {
        if(e.hoveredTile == null ||
            e.hoveredTile.tile.containedObject == null ||
            e.hoveredTile.tile.containedObject != Context.hoveredEnemy)
        {
            TransitionTo<PlayerRange>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<InputHover>(OnInputHover);
    }
}

public class PlayerRange : MapDisplayState
{
    public override void OnEnter()
    {
        base.OnEnter();
        SetPlayerRangeDisplay();
        Services.EventManager.Register<EnergyChanged>(OnEnergyChanged);
        Services.EventManager.Register<GridObjectMoved>(OnGridObjectMoved);
        Services.EventManager.Register<GridObjectDeath>(OnGridObjectDeath);
        Services.EventManager.Register<InputHover>(OnInputHover);
        //Debug.Log("entering player range at time " + Time.time);
    }

    public void OnEnergyChanged(EnergyChanged e)
    {
        SetPlayerRangeDisplay();
    }

    public void OnGridObjectDeath(GridObjectDeath e)
    {
        SetPlayerRangeDisplay();
    }

    public void OnGridObjectMoved(GridObjectMoved e)
    {
        if (e.gridObject != player) return;
        TransitionTo<NoRangeDisplay>();
    }

    private void SetPlayerRangeDisplay()
    {
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(player.currentTile,
            player.currentEnergy, player);
        foreach(TileRenderer tileRenderer in Context.tileRenderers)
        {
            if (tilesInRange.Contains(tileRenderer.tile))
            {
                tileRenderer.SetRangeColor(TileRenderer.RangeLevel.MOVE);
            }
            else
            {
                tileRenderer.SetRangeColor(TileRenderer.RangeLevel.NONE);
            }
        }
    }

    protected virtual void OnInputHover(InputHover e)
    {
        if (e.hoveredTile != null)
        {
            if (e.hoveredTile.tile.containedObject !=null)
            {
                GridObject gridObject = e.hoveredTile.tile.containedObject;
                if (gridObject.data.phylum == GridObjectData.Phylum.ENEMY)
                {
                    Context.hoveredEnemy = gridObject;
                    TransitionTo<EnemyRange>();
                }
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<EnergyChanged>(OnEnergyChanged);
        Services.EventManager.Unregister<GridObjectMoved>(OnGridObjectMoved);
        Services.EventManager.Unregister<GridObjectDeath>(OnGridObjectDeath);
        Services.EventManager.Unregister<InputHover>(OnInputHover);
        //Debug.Log("exiting player range at time " + Time.time);
    }
}

public class NoRangeDisplay : MapDisplayState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Services.EventManager.Register<GridObjectMovementComplete>(OnMovementComplete);
    }

    public void OnMovementComplete(GridObjectMovementComplete e)
    {
        if (e.id != player.id) return;
        TransitionTo<PlayerRange>();
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<GridObjectMovementComplete>(OnMovementComplete);
    }
}

public class CardRange : MapDisplayState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Services.EventManager.Register<CardRendererHoverEnd>(OnCardHoverEnd);
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(player.currentTile,
           Context.hoveredCard.maxRange, player, true, Context.hoveredCard.minRange);
        foreach (TileRenderer tileRenderer in Context.tileRenderers)
        {
            if (tilesInRange.Contains(tileRenderer.tile)) tileRenderer.SetRangeColor(TileRenderer.RangeLevel.ATTACK);
            else tileRenderer.SetRangeColor(TileRenderer.RangeLevel.NONE);
        }
        //Debug.Log("entering card range at time " + Time.time);
    }

    public void OnCardHoverEnd(CardRendererHoverEnd e)
    {
        if (!e.otherCardHovered)
        {
            TransitionTo<PlayerRange>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<CardRendererHoverEnd>(OnCardHoverEnd);
        //Debug.Log("exiting card range at time " + Time.time);
    }
}

public class StartLevelTransitionAnimation : GameEvent { }