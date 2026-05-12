using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class StateMachine<T> where T : class
    {
        private T owner;
        private Dictionary<System.Type, IState> states = new Dictionary<System.Type, IState>();
        private IState currentState;

        public StateMachine(T owner)
        {
            this.owner = owner;
        }

        public void AddState(IState state)
        {
            states[state.GetType()] = state;
        }

        public void ChangeState<TState>() where TState : IState
        {
            var newState = states[typeof(TState)];
            if (currentState == newState) return;

            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public TState GetState<TState>() where TState : IState
        {
            return (TState)states[typeof(TState)];
        }

        public bool IsInState<TState>() where TState : IState
        {
            return currentState is TState;
        }
    }
}