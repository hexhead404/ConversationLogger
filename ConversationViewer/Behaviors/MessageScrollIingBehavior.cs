// <copyright file="MessageScrollIingBehavior.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Viewer.Behaviors
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// A <see cref="ListBox"/> behavior to scroll the current message into view.
    /// </summary>
    public class MessageScrollIingBehavior : Behavior<ListBox>
    {
        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (this.AssociatedObject is ItemsControl control)
            {
                if (control.Items is ICollectionView view)
                {
                    view.CurrentChanged += this.OnCollectionCurrentChanged;
                }

                if (control.Items is INotifyCollectionChanged notifier)
                {
                    notifier.CollectionChanged += this.OnCollectionChanged;
                }
            }

            base.OnAttached();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            if (this.AssociatedObject is ItemsControl control)
            {
                if (control.Items is ICollectionView view)
                {
                    view.CurrentChanged -= this.OnCollectionCurrentChanged;
                }

                if (control.Items is INotifyCollectionChanged notifier)
                {
                    notifier.CollectionChanged -= this.OnCollectionChanged;
                }
            }

            base.OnAttached();
        }

        private void OnCollectionCurrentChanged(object sender, EventArgs e)
        {
            if (this.AssociatedObject is ListBox control && sender is ICollectionView view)
            {
                control.ScrollIntoView(view.CurrentItem);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.AssociatedObject is ListBox control && sender is ICollectionView view)
            {
                if (e.Action != NotifyCollectionChangedAction.Reset)
                {
                    view.MoveCurrentToLast();
                }

                control.ScrollIntoView(view.CurrentItem);
            }
        }
    }
}
