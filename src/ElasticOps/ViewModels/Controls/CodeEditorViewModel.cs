﻿using System;
using System.Globalization;
using System.Windows.Media;
using System.Xml;
using Caliburn.Micro;
using ElasticOps.Commands;
using ElasticOps.Events;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace ElasticOps.ViewModels.Controls
{
    public class CodeEditorViewModel : PropertyChangedBase, IHandle<ThemeChangedEvent>
    {
        private string _code;
        private Brush _foreground;
        private IHighlightingDefinition _highlightingDefinition;
        private bool _isReadOnly;

        public CodeEditorViewModel(Infrastructure infrastructure)
        {
            Ensure.ArgumentNotNull(infrastructure, "infrastructure");

            infrastructure.EventAggregator.Subscribe(this);
            LoadHighlightRules(infrastructure.Config.Appearance.Theme == "BaseDark" ? Theme.Dark : Theme.Light);
        }

        public string Code
        {
            get { return _code; }
            set
            {
                if (value == _code) return;
                _code = value;
                NotifyOfPropertyChange(() => Code);
            }
        }

        public Brush Foreground
        {
            get { return _foreground; }
            set
            {
                if (Equals(value, _foreground)) return;
                _foreground = value;
                NotifyOfPropertyChange(() => Foreground);
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                if (value.Equals(_isReadOnly)) return;
                _isReadOnly = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }

        public IHighlightingDefinition HighlightingDefinition
        {
            get { return _highlightingDefinition; }
            set
            {
                if (Equals(value, _highlightingDefinition)) return;
                _highlightingDefinition = value;
                NotifyOfPropertyChange(() => HighlightingDefinition);
            }
        }

        public void Handle(ThemeChangedEvent message)
        {
            Ensure.ArgumentNotNull(message, "message");

            LoadHighlightRules(message.IsDark ? Theme.Dark : Theme.Light);
        }

        private void LoadHighlightRules(Theme theme)
        {
            Foreground = theme == Theme.Dark ? Brushes.AntiqueWhite : Brushes.Navy;

            var schemaStream =
                (GetType()).Assembly.GetManifestResourceStream(String.Format(CultureInfo.InvariantCulture,
                    "ElasticOps.Resources.Query{0}HighlightingRules.xshd", theme));

            using (var reader = new XmlTextReader(schemaStream))
            {
                HighlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        private enum Theme
        {
            Dark,
            Light
        }
    }
}