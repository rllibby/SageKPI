/*
 *  Copyright © 2015, Russell Libby
 */
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace SageKPI
{
    /// <summary>
    /// Class for the alert wrapper.
    /// </summary>
    public class AlertWrapper
    {
        #region Private fields

        private readonly List<string> _list;
        private readonly TextBlock _control;
        private int _newAlert = (-1);
        private int _index;
        private bool _running;

        #endregion

        #region Private methods

        /// <summary>
        /// Fades a UI element in.
        /// </summary>
        private void FadeIn()
        {
            if (!_running) return;

            var fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                BeginTime = TimeSpan.FromSeconds(2)
            };

            var sb = new Storyboard();

            Storyboard.SetTarget(fadeIn, _control);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");

            sb.Children.Add(fadeIn);

            _control.Resources.Clear();
            _control.Resources.Add("FaderEffect", sb);

            sb.Completed += OnFadeInCompleted;
            sb.Begin();

        }

        /// <summary>
        /// Handler that is called when the fade in has completed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event argument.</param>
        private void OnFadeInCompleted(object sender, object e)
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                BeginTime = TimeSpan.FromSeconds(2)
            };

            var sb = new Storyboard();

            Storyboard.SetTarget(fadeOut, _control);
            Storyboard.SetTargetProperty(fadeOut, "Opacity");

            sb.Children.Add(fadeOut);

            _control.Resources.Clear();
            _control.Resources.Add("FaderEffect", sb);

            sb.Completed += OnFadeOutCompleted;
            sb.Begin();
        }

        /// <summary>
        /// Handler that is called when the fade out has completed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event argument.</param>
        private void OnFadeOutCompleted(object sender, object e)
        {
            if (_newAlert >= 0)
            {
                _index = _newAlert;
                _newAlert = (-1);
            }
            else
            {
                _index++;
            }

            if (_index >= _list.Count)
            {
                _index = 0;

                if (_list.Count == 0)
                {
                    _index = (-1);
                }
            }

            _control.Text = (_index >= 0) ? _list[_index] : string.Empty;

            if (_running) FadeIn();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constrcutor.
        /// </summary>
        /// <param name="control">The text block control to wrap.</param>
        public AlertWrapper(TextBlock control)
        {
            _list = new List<string>();
            _control = control;
            _running = false;
            _newAlert = (-1);
            _index = 0;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public void Start()
        {
            if (_running) return;

            _running = true;
            FadeIn();
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// Adds a new alert to be processed.
        /// </summary>
        /// <param name="textAlert">The text body for the alert.</param>
        public void AddAlert(string textAlert)
        {
            if (_list.Count >= 10) _list.RemoveAt(0);

            _list.Add(textAlert);

            if (_newAlert == (-1)) _newAlert = _list.Count - 1;
        }

        #endregion
    }
}
