namespace Utility.Instancing
{
    using System;
    using UnityEngine;

    public abstract class MatPropBlockConsumer : MonoBehaviour
    {
        [NonSerialized] private bool _Inited = false;
        [SerializeField] private MatPropBlockHolder _MatPropBlockHolder;

        protected bool Init()
        {
            if (_Inited)
                return true;

            if (_MatPropBlockHolder == null || !_MatPropBlockHolder.Init())
            {
                return false;
            }

            _MatPropBlockHolder.RefreshEvent += OnRefresh;

            _Inited = true;
            OnInit();
            return true;
        }

        protected void RequestRefresh()
        {
            _MatPropBlockHolder.RequestRefresh();
        }

        private void OnDestroy()
        {
            if (_Inited && _MatPropBlockHolder != null)
                _MatPropBlockHolder.RefreshEvent -= OnRefresh;
        }

        protected virtual void OnInit()
        {

        }

        protected abstract void OnRefresh(MaterialPropertyBlock mpb);
    }
}