using System;
using UnityEngine;

using Utility.Attributes;

namespace Utility
{
    [Serializable]
    public struct CollisionState
    {
        public bool Enabled;

        public CollisionsStateType CurrentState => _currentState;
        [SerializeField, ShowOnly] private CollisionsStateType _currentState;

        [SerializeField, ShowOnly] private int _enabledLayer;
        [SerializeField, ShowOnly] private int _disabledLayer;

        public void Init(GameObject gameObject, bool enabled, int enabledLayer, int disabledLayer)
        {
            _enabledLayer = enabledLayer;
            _disabledLayer = disabledLayer;
            Enabled = enabled;
            SetState(gameObject, Enabled ? CollisionsStateType.Enabled : CollisionsStateType.Disabled);
        }

        public void Update(GameObject gameObject, Collider2D collider, LayerMask layerMask)
        {
            switch (_currentState)
            {
                case CollisionsStateType.Enabled:
                    if (!Enabled)
                    {
                        SetState(gameObject, CollisionsStateType.Disabled);
                    }
                    break;
                case CollisionsStateType.Disabled:
                    if (Enabled)
                    {
                        SetState(gameObject, CollisionsStateType.Enabling);
                        goto case CollisionsStateType.Enabling;
                    }
                    break;
                case CollisionsStateType.Enabling:
                    if (Enabled)
                    {
                        var results = collider.Cast(new ContactFilter2D().WithLayerMask(layerMask));
                        if (results == 0)
                            SetState(gameObject, CollisionsStateType.Enabled);
                    }
                    else
                        SetState(gameObject, CollisionsStateType.Disabled);
                    break;

            }
        }

        private void SetState(GameObject gameObject, CollisionsStateType state)
        {
            if (state == _currentState)
                return;
            _currentState = state;
            switch (_currentState)
            {
                case CollisionsStateType.Enabled:
                    gameObject.layer = _enabledLayer;
                    break;
                case CollisionsStateType.Enabling:
                case CollisionsStateType.Disabled:
                    gameObject.layer = _disabledLayer;
                    break;
            }
        }
    }

    public enum CollisionsStateType
    {
        Enabled,
        Enabling,
        Disabled,
    }
}
