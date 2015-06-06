/*
 *  Copyright © 2015, Russell Libby
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SageKPI.ViewModel
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private bool _loaded;

        #endregion

        #region Private methods

        /// <summary>
        /// Called when a property value has changed so that bindings can be updated.
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isDataLoaded">True if data is loaded.</param>
        public MainViewModel(bool isDataLoaded)
        {
            _loaded = isDataLoaded;
            
            SalesItems = new ObservableCollection<KpiViewModel>();
            CashFlowItems = new ObservableCollection<KpiViewModel>();
            ExpenseItems = new ObservableCollection<KpiViewModel>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates a channel list with a new kpi.
        /// </summary>
        /// <param name="kpi">The new kpi to add.</param>
        /// <returns>The kpi that was added.</returns>
        public KpiViewModel UpdateKpi(Kpi kpi)
        {
            ObservableCollection<KpiViewModel> list = null;

            if (kpi == null) return null;

            if (kpi.Channel.Equals("Sales"))
            {
                list = SalesItems;
            }
            else if (kpi.Channel.Equals("CashFlow"))
            {
                list = CashFlowItems;
            }
            else if (kpi.Channel.Equals("Expense"))
            {
                list = ExpenseItems;
            }

            if (list == null) return null;

            foreach (var item in list)
            {
                if (item.Type.Equals(kpi.Type))
                {
                    item.Total = kpi.Total.ToString("C");
                    item.NumberOf = kpi.NumberOf.ToString(CultureInfo.InvariantCulture);
                    item.Last = kpi.Last.ToString("C");
                    item.Largest = kpi.Largest.ToString("C");
                    item.Smallest = kpi.Smallest.ToString("C");
                    item.Average = kpi.Average.ToString("C");

                    return item;
                }
            }

            var viewKpi = new KpiViewModel(kpi);

            list.Add(viewKpi);

            return viewKpi;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Collection of Sales kpis
        /// </summary>
        public ObservableCollection<KpiViewModel> SalesItems { get; private set; }

        /// <summary>
        /// Collection of cash flow kpis.
        /// </summary>
        public ObservableCollection<KpiViewModel> CashFlowItems { get; private set; }

        /// <summary>
        /// Collection of expense kpis.
        /// </summary>
        public ObservableCollection<KpiViewModel> ExpenseItems { get; private set; }

        /// <summary>
        /// The sales channel color.
        /// </summary>
        public Color SalesColor
        {
            get { return ChannelColors.GetChannelColor("Sales"); }
        }

        /// <summary>
        /// The cash flow channel color.
        /// </summary>
        public Color CashFlowColor
        {
            get { return ChannelColors.GetChannelColor("CashFlow"); }
        }

        /// <summary>
        /// The expense channel color.
        /// </summary>
        public Color ExpenseColor
        {
            get { return ChannelColors.GetChannelColor("Expense"); }
        }

        /// <summary>
        /// The sales channel brush.
        /// </summary>
        public Brush SalesBrush
        {
            get { return ChannelColors.GetChannelBrush("Sales"); }
        }

        /// <summary>
        /// The cash flow channel brush.
        /// </summary>
        public Brush CashFlowBrush
        {
            get { return ChannelColors.GetChannelBrush("CashFlow"); }
        }

        /// <summary>
        /// The expense channel brush.
        /// </summary>
        public Brush ExpenseBrush
        {
            get { return ChannelColors.GetChannelBrush("Expense"); }
        }

        /// <summary>
        /// True if data is loaded, otherwise false.
        /// </summary>
        public bool IsDataLoaded
        {
            get { return _loaded; }
            set
            {
                _loaded = value;

                NotifyPropertyChanged("IsDataLoaded");
            }
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}