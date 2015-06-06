/*
 *  Copyright © 2015, Russell Libby
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.AspNet.SignalR.Client;
using SageKPI.ViewModel;

namespace SageKPI
{
    /// <summary>
    /// The main page for the Windows Phone 8.1 application.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Private fields

        private readonly AlertWrapper _alerts;
        private ListBox _activeControl;
        private HubConnection _connection;
        private IHubProxy _hub;
        private bool _connected;

        #endregion

        #region Private methods

        /// <summary>
        /// Show or hide progress meter.
        /// </summary>
        /// <param name="show"></param>
        private void ShowProgress(bool show)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Progress.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        /// <summary>
        /// Starts an animation on a listbox item element.
        /// </summary>
        /// <param name="element">The element to animate.</param>
        private void StartAnimation(ListBoxItem element)
        {
            var sb = new Storyboard();
            var animation = new DoubleAnimation()
            {
                Duration = new TimeSpan(0, 0, 1)
            };

            sb.Children.Add(animation);

            if (element.Projection == null)
            {
                element.Projection = new PlaneProjection()
                {
                    CenterOfRotationX = 0.5
                };
            }

            var projection = element.Projection as PlaneProjection;

            animation.From = 0;
            animation.To = 360;

            Storyboard.SetTarget(animation, projection);
            Storyboard.SetTargetProperty(animation, "RotationXProperty");

            sb.Begin();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        private void OnAlertUpdate(string channel, string data)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _alerts.AddAlert(data));
        }

        private void OnUpdateKpi(Kpi kpi)
        {
            if (kpi != null)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var viewKpi = App.ViewModel.UpdateKpi(kpi);

                    if ((viewKpi != null) && (_activeControl != null))
                    {
                        for (int i = 0; i < _activeControl.Items.Count; i++)
                        {
                            var item = (ListBoxItem)(_activeControl.ItemContainerGenerator.ContainerFromIndex(i));

                            if (item == null) continue;

                            var model = item.DataContext as KpiViewModel;

                            if (model == null) continue;

                            if (viewKpi.Type.Equals(model.Type))  
                            {
                                StartAnimation(item);
                            }
                        }
                    }
                });
            }
        }

        private void OnReset()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                App.ViewModel.SalesItems.Clear();
                App.ViewModel.CashFlowItems.Clear();
                App.ViewModel.ExpenseItems.Clear();
            });

            ShowProgress(true);

            try
            {
                GetKpiList("Sales");
                GetKpiList("CashFlow");
                GetKpiList("Expense");
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private void GetKpiList(string channel)
        {
            if (_connected)
            {
                Task.Run(async () =>
                {
                    var list = await _hub.Invoke<IEnumerable<Kpi>>("GetAllKPIs", new object[] {channel});

                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        foreach (Kpi kpi in list)
                        {
                            App.ViewModel.UpdateKpi(kpi);
                        }
                    });

                });
            }
        }

        private void ReportChange(StateChange change)
        {
            if (change.NewState == ConnectionState.Connected)
            {
                _connected = true;

                if (change.OldState != ConnectionState.Reconnecting)
                {
                    GetKpiList("Sales");
                    GetKpiList("CashFlow");
                    GetKpiList("Expense");
                }

                ShowProgress(false);

            }
            else if (change.OldState == ConnectionState.Connected)
            {
                _connected = false;

                ShowProgress(true);
            }
        }

        private void ReportError(Exception error)
        {
            ShowProgress(true);
        }

        private void _Connection_Closed()
        {
            _connection.Start();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = App.ViewModel;

            _connection = new HubConnection("http://sageaviato.azurewebsites.net/");

            _connection.StateChanged += change => ReportChange(change);
            _connection.Closed += _Connection_Closed;
            _connection.Error += ex => { ReportError(ex); };

            _hub = _connection.CreateHubProxy("erpTicker");

            _hub.On<string, string>("addTickerItem", (channel, data) => OnAlertUpdate(channel, data));
            _hub.On<Kpi>("updateSalesKPI", data => OnUpdateKpi(data));
            _hub.On<Kpi>("updateCashFlowKPI", data => OnUpdateKpi(data));
            _hub.On<Kpi>("updateExpenseKPI", data => OnUpdateKpi(data));
            _hub.On<Kpi>("addSalesKPI", data => OnUpdateKpi(data));
            _hub.On<Kpi>("addCashFlowKPI", data => OnUpdateKpi(data));
            _hub.On<Kpi>("addExpenseKPI", data => OnUpdateKpi(data));
            _hub.On("reset", () => OnReset());

            _connection.Start();
        }
    }
}
