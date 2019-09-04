using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class EnemyTurnManager
{
    private StateMachine<EnemyTurnManager> _stateMachine;
    public List<GridObject> gridObjects { get; private set; }

    public EnemyTurnManager()
    {
        _stateMachine = new StateMachine<EnemyTurnManager>(this);
        _stateMachine.InitializeState<Waiting>();
    }

    public void ExecuteEnemyTurn(List<GridObject> gridObjects_)
    {
        gridObjects = gridObjects_;
        Services.EventManager.Fire(new EnemyTurnStarted());
    }

    public void EnactEnemyBehaviors()
    {
        int behaviorCount = 0;
        List<GridObject> sortedObjects = new List<GridObject>(gridObjects.OrderBy(go => go.id));
        foreach (GridObject gridObject in sortedObjects)
        {
            List<EnemyTurnBehavior> behaviors = new List<EnemyTurnBehavior>(
                gridObject.data.enemyTurnBehaviors.OrderBy(b => b.priority));
            foreach (EnemyTurnBehavior behavior in behaviors)
            {
                behavior.OnEnemyTurn(gridObject);
                behaviorCount += 1;
            }
        }
        if (behaviorCount == 0)
        {
            Services.EventManager.Fire(new AllQueuedAnimationsComplete());
        }
    }

    public void Update()
    {
        _stateMachine.Update();
    }

    private class Animating : StateMachine<EnemyTurnManager>.State
    {
        public override void OnEnter()
        {
            Services.EventManager.Register<AllQueuedAnimationsComplete>(OnAnimationsComplete);
            Context.EnactEnemyBehaviors();
        }

        public void OnAnimationsComplete(AllQueuedAnimationsComplete e)
        {
            TransitionTo<Waiting>();
        }

        public override void OnExit()
        {
            Services.EventManager.Unregister<AllQueuedAnimationsComplete>(OnAnimationsComplete);
        }
    }

    private class Waiting : StateMachine<EnemyTurnManager>.State
    {
        public override void OnEnter()
        {
            Services.EventManager.Register<EnemyTurnStarted>(OnEnemyTurnStarted);
            Services.EventManager.Fire(new PlayerTurnStarted());
        }

        public void OnEnemyTurnStarted(EnemyTurnStarted e)
        {
            TransitionTo<Animating>();
        }

        public override void OnExit()
        {
            Services.EventManager.Unregister<EnemyTurnStarted>(OnEnemyTurnStarted);
        }
    }
}

public class EnemyTurnStarted : GameEvent
{

}

