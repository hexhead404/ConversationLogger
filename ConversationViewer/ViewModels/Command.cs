// <copyright file="Command.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Viewer.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using ConversationLogger.Common;

    /// <summary>
    /// A simple implementation of <see cref="ICommand"/> for use with view models.
    /// </summary>
    public class Command : ICommand, IDisposable
    {
        private INotifyPropertyChanged owner;
        private Func<object, bool> canExecute;
        private Action<object> execute;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="execute">The execute action.</param>
        public Command(INotifyPropertyChanged owner, Action<object> execute)
        : this(owner, null, execute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="canExecute">The function to execute to determine whether the command can execute.</param>
        /// <param name="execute">The execute action.</param>
        /// <exception cref="ArgumentException">When <paramref name="owner"/> is null.</exception>
        public Command(INotifyPropertyChanged owner, Func<object, bool> canExecute, Action<object> execute)
        {
            this.owner = owner.AssertParamterNotNull(nameof(owner));

            owner.PropertyChanged += this.OwnerOnPropertyChanged;
            this.canExecute = canExecute;
            this.execute = execute;
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke(parameter) ?? true;
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            this.execute?.Invoke(parameter);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

                /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposing">Whether this method was called from the <see cref="Dispose"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                this.owner.PropertyChanged -= this.OwnerOnPropertyChanged;
                this.owner = null;
                this.canExecute = null;
                this.execute = null;
            }
        }

        private void OwnerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
