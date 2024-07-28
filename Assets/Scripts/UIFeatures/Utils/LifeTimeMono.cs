namespace UIFeatures.Utils
{
    using System;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using UniRx;
    using UnityEngine;

    public class LifeTimeMono : MonoBehaviour
    {
        public  float       lifeTime;
        private IDisposable lifeTimeObservable;

        private void OnEnable() { this.lifeTimeObservable = Observable.Timer(TimeSpan.FromSeconds(this.lifeTime)).Subscribe(l => { this.gameObject.Recycle(); }); }

        private void OnDisable() { this.lifeTimeObservable.Dispose(); }
    }
}