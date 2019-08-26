
namespace ConversationLogger.Viewer.Behaviors
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// A behavior to scroll the current message into view
    /// </summary>
    public class MessageScrollIingBehavior : Behavior<ListView>
    {
        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (this.AssociatedObject is ListView view && view.Items is ICollectionView collection)
            {
                collection.CurrentChanged += this.OnCollectionCurrentChanged;
            }
            base.OnAttached();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            if (this.AssociatedObject is ListView view && view.Items is ICollectionView collection)
            {
                collection.CurrentChanged -= this.OnCollectionCurrentChanged;
            }
            base.OnAttached();
        }

        private void OnCollectionCurrentChanged(object sender, EventArgs e)
        {
            if (this.AssociatedObject is ListView view && sender is ICollectionView collection)
            {
                view.ScrollIntoView(collection.CurrentItem);
            }
        }
    }
}
