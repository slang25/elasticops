﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ElasticOps.Behaviors.Suggesters;

namespace ElasticOps.Behaviors
{
    public class UrlAutoCompleteBoxBehavior : Behavior<AutoCompleteBox>
    {
        private UrlSuggest _urlSuggest;

        protected override void OnAttached()
        {
            AssociatedObject.ItemTemplate = Application.Current.FindResource("UrlAutoCompleteBoxItemTemplate") as DataTemplate;
            _urlSuggest = AppBootstrapper.GetInstance<UrlSuggest>();
            AssociatedObject.IsTextCompletionEnabled = true;
            AssociatedObject.MinimumPrefixLength = 0;
            AssociatedObject.ItemsSource = _urlSuggest;

            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            base.OnDetaching();
        }

        void AssociatedObject_TextChanged(object sender, RoutedEventArgs e)
        {
            _urlSuggest.UpdateSuggestions(AssociatedObject.Text);
        }

    }
}