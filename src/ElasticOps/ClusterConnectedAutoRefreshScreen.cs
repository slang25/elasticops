﻿using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using ElasticOps.Com;

namespace ElasticOps
{
    internal abstract class ClusterConnectedAutoRefreshScreen : Screen, IHandle<RefreshEvent>
    {
        protected ClusterConnectedAutoRefreshScreen(Infrastructure infrastructure)
        {
            if (infrastructure == null)
                throw new ArgumentNullException("infrastructure");

            EventAggregator = infrastructure.EventAggregator;
            CommandBus = infrastructure.CommandBus;
            Connection = infrastructure.Connection;
        }

        protected IEventAggregator EventAggregator { get; set; }
        protected CommandBus CommandBus { get; set; }
        protected Connection Connection { get; set; }
        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                NotifyOfPropertyChange(() => IsRefreshing);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            Deactivate(close);
        }

        public void Deactivate(bool close)
        {
            EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        protected override void OnActivate()
        {
            EventAggregator.Subscribe(this);
            StartRefreshingData();
            base.OnActivate();
        }

        public void Handle(RefreshEvent message)
        {
            if (!IsRefreshing)
                StartRefreshingData();
        }

        private void StartRefreshingData()
        {
            IsRefreshing = true;
            Task.Factory.StartNew(RefreshData)
                .ContinueWith(t => IsRefreshing = false);
        }

        public abstract void RefreshData();
    }
}