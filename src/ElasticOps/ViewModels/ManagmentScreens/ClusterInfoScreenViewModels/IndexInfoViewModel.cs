﻿using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ElasticOps.Com;
using Humanizer;
using Action = System.Action;

namespace ElasticOps.ViewModels.ManagmentScreens
{
    public class IndexInfoViewModel : PropertyChangedBase
    {
        private readonly Infrastructure _infrastructure;
        private readonly Action _refreashIndexList;
        private bool _isOpen;

        public IndexInfoViewModel(IndexInfo indexInfo, Infrastructure infrastructure, Action refreashIndexList)
        {
            _infrastructure = infrastructure;
            _refreashIndexList = refreashIndexList;
            Name = indexInfo.Name;
            IsOpen = indexInfo.IsOpen;
            Settings = indexInfo.Settings.Select(setting => new ElasticPropertyViewModel
            {
                Label = setting.Key.Humanize(LetterCasing.Sentence),
                Value = setting.Value
            });
            Types = indexInfo.Types.Select(type => new ElasticPropertyViewModel
            {
                Label = type.Key.Humanize(LetterCasing.Sentence),
                Value = type.Value
            });

        }


        public string Name { get; set; }

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (value.Equals(_isOpen)) return;
                _isOpen = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        public IEnumerable<ElasticPropertyViewModel> Settings { get; set; }

        public IEnumerable<ElasticPropertyViewModel> Types { get; set; }


        public void Flush()
        {
            _infrastructure.CommandBus.Execute(new Index.FlushCommand(_infrastructure.Connection, Name));
        }

        public void ClearCache()
        {
            _infrastructure.CommandBus.Execute(new Index.ClearCacheCommand(_infrastructure.Connection, Name));

        }

        public void Optimize()
        {
            _infrastructure.CommandBus.Execute(new Index.OptimizeCommand(_infrastructure.Connection, Name));
        }

        public void RefreshIndex()
        {
            _infrastructure.CommandBus.Execute(new Index.RefreshCommand(_infrastructure.Connection, Name));
        }

        public void Close()
        {
            var res = _infrastructure.CommandBus.Execute(new Index.CloseCommand(_infrastructure.Connection, Name));

            if (res.Success)
                IsOpen = false;

        }

        public void Open()
        {
            var res = _infrastructure.CommandBus.Execute(new Index.OpenCommand(_infrastructure.Connection, Name));

            if (res.Success)
                IsOpen = true;
        }

        public void Delete()
        {
            _infrastructure.CommandBus.Execute(new Index.DeleteCommand(_infrastructure.Connection, Name));
            _refreashIndexList();
        }

    }
}
