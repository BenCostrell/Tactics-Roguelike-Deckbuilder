using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StateMachine<TContext>
{
    private readonly TContext _context;
    public State currentState { get; private set; }
    private State _pendingState;
    private readonly Dictionary<Type, State> _stateCache = new Dictionary<Type, State>();

    public StateMachine(TContext context)
    {
        _context = context;
    }

    public void InitializeState<TState>() where TState : State
    {
        currentState = GetOrCreateState<TState>();
        currentState.OnEnter();
    }

    public void Update()
    {
        // Handle any pending transition if someone called TransitionTo externally (although they probably shouldn't)
        PerformPendingTransition();
        // Make sure there's always a current state to update...
        Debug.Assert(currentState != null, "Updating FSM with null current state. Did you forget to transition to a starting state?");
        currentState.Update();
        // Handle any pending transition that might have happened during the update
        PerformPendingTransition();
    }

    private void PerformPendingTransition()
    {
        if (_pendingState != null)
        {
            if (currentState != null) currentState.OnExit();
            currentState = _pendingState;
            _pendingState = null;
            currentState.OnEnter();
        }
    }

    // Queues transition to a new state
    public void TransitionTo<TState>() where TState : State
    {
        // We do the actual transtion
        _pendingState = GetOrCreateState<TState>();
    }

    private TState GetOrCreateState<TState>() where TState : State
    {
        State state;
        if (_stateCache.TryGetValue(typeof(TState), out state))
        {
            return (TState)state;
        }
        else
        {
            // This activator business is required to create instances of states
            // using only the type
            var newState = Activator.CreateInstance<TState>();
            newState.Parent = this;
            newState.Init();
            _stateCache[typeof(TState)] = newState;
            return newState;
        }
    }

    public abstract class State
    {
        internal StateMachine<TContext> Parent { get; set; }

        protected TContext Context { get { return Parent._context; } }

        protected void TransitionTo<TState>() where TState : State
        {
            Parent.TransitionTo<TState>();
        }

        public virtual void Update() { }

        public virtual void Init() { }

        public virtual void OnEnter() { }

        public virtual void OnExit() { }
    }
}


