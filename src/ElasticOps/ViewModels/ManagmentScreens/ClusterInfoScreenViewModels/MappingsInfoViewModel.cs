﻿using System.Collections.Generic;
using System.Windows;
using ElasticOps.Com;


namespace ElasticOps.ViewModels.ManagmentScreens
{
    public class MappingsInfoViewModel : ClusterConnectedAutorefreashScreen
    {
        private readonly Infrastructure _infrastructure;

        private IEnumerable<IndexInfo> indices;
        private string _mapping;

        public MappingsInfoViewModel(TypesListViewModel typesListViewModel, Infrastructure infrastructure) : base(infrastructure)
        {
            _infrastructure = infrastructure;
            this.TypesList = typesListViewModel;
            TypesList.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != "SelectedType") return;

                LoadMapping();
            };
        }

        public override void RefreshData()
        {
            TypesList.RefreashData();


        }

        private void LoadMapping()
        {
            if (string.IsNullOrEmpty(TypesList.SelectedIndex) || string.IsNullOrEmpty(TypesList.SelectedType)) return;

            var res =_infrastructure.CommandBus.Execute(new ClusterInfo.GetMappingCommand(_infrastructure.Settings.Connection,
                TypesList.SelectedIndex, TypesList.SelectedType));

            if (res.Success) Mapping = res.Result;
        }

        public TypesListViewModel TypesList { get; set; }

        public string Mapping
        {
            get { return _mapping; }
            set
            {
                _mapping = value;
                NotifyOfPropertyChange(() => Mapping);
            }
        }

        public void CopyToCliboard()
        {
            Clipboard.SetText(Mapping);
        }
    }
}
