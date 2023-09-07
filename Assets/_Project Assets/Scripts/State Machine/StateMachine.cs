using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private bool debugState;
    [SerializeField] string currentStateLog;

    IState state;
    IState nextState;
    Dictionary<IState, List<Transition>> transitionMap;
    List<Transition> globalTransitions;
    
    private void Awake()
    {
        transitionMap = new Dictionary<IState, List<Transition>>();
        globalTransitions = new List<Transition>();
    }

    private void Update()
    {
        if (state != null)
        {
            nextState = CheckTransition();
        }
        if (nextState != null)
        {
            TransitionTo(nextState);
        }

        state?.Tick();

        if (debugState)
        {
            currentStateLog = state.ToString();
        }
    }

    private void FixedUpdate()
    {
        state?.FixedTick();
    }

    private void LateUpdate()
    {
        state?.LateTick();
    }

    public void SetInitialState(IState state)
    {
        this.state = state;
    }

    #region Transition Logic

    private IState CheckTransition()
    {
        if (transitionMap.ContainsKey(state))
        {
            foreach (Transition transition in transitionMap[state])
            {
                if (transition.cond() == true)
                {
                    return transition.toState;
                }
            }
        }
        foreach (Transition transition in globalTransitions)
        {
            if (transition.cond() == true)
            {
                return transition.toState;
            }
        }
        return null;
    }

    private void TransitionTo(IState next)
    {
        state.OnExit();
        state = next;
        state.OnEnter();
        nextState = null;
    }

    public void AddTransition(IState fromState, Func<bool> cond, IState toState)
    {
        if (!transitionMap.ContainsKey(fromState))
        {
            transitionMap.Add(fromState, new List<Transition>());
        }

        transitionMap[fromState].Add(new Transition(cond, toState));
    }

    public void AddGlobalTransition(Func<bool> cond, IState toState)
    {
        globalTransitions.Add(new Transition(cond, toState));
    } 
    #endregion
}

public struct Transition
{
    public Func<bool> cond;
    public IState toState;

    public Transition(Func<bool> cond, IState toState)
    {
        this.cond = cond;
        this.toState = toState;
    }
}
