/*
 *  Copyright © 2015, Russell Libby
 */
using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SageKPI.ViewModel
{
    /// <summary>
    /// Static class for channel colors and brushes.
    /// </summary>
    public static class ChannelColors
    {
        /// <summary>
        /// Returns the color for the spceified channel name.
        /// </summary>
        /// <param name="channel">The name of the channel.</param>
        /// <returns>The color for the channel.</returns>
        public static Color GetChannelColor(string channel)
        {
            if (String.Equals(channel, "Sales", StringComparison.OrdinalIgnoreCase)) return Color.FromArgb(0xff, 0x02, 0x47, 0x31);
            if (String.Equals(channel, "CashFlow", StringComparison.OrdinalIgnoreCase)) return Color.FromArgb(0xff, 0x69, 0x92, 0x3a);
            if (String.Equals(channel, "Expense", StringComparison.OrdinalIgnoreCase)) return Color.FromArgb(0xff, 0xa8, 0xb4, 0x00);

            return Color.FromArgb(0xff, 0x00, 0x00, 0x00);
        }

        /// <summary>
        /// Returns the solid color brush for the specifed channel.
        /// </summary>
        /// <param name="channel">The name of the channel.</param>
        /// <returns>The brush for the channel.</returns>
        public static Brush GetChannelBrush(string channel)
        {
            if (String.Equals(channel, "Sales", StringComparison.OrdinalIgnoreCase)) return new SolidColorBrush(Color.FromArgb(0xff, 0x02, 0x47, 0x31));
            if (String.Equals(channel, "CashFlow", StringComparison.OrdinalIgnoreCase)) return new SolidColorBrush(Color.FromArgb(0xff, 0x69, 0x92, 0x3a));
            if (String.Equals(channel, "Expense", StringComparison.OrdinalIgnoreCase)) return new SolidColorBrush(Color.FromArgb(0xff, 0xa8, 0xb4, 0x00));

            return new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0x00, 0x00));
        }
    }

    /// <summary>
    /// The KPI structure.
    /// </summary>
    public class Kpi
    {
        #region Public properties

        /// <summary>
        /// The type of the kpi.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The channel for the kpi.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// The total value for the kpi.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// The number of items represented by the kpi.
        /// </summary>
        public int NumberOf { get; set; }

        /// <summary>
        /// The value of the last item in the kpi.
        /// </summary>
        public decimal Last { get; set; }

        /// <summary>
        /// The value of the largest item in the kpi.
        /// </summary>
        public decimal Largest { get; set; }

        /// <summary>
        /// The value of the smallest item in the kpi.
        /// </summary>
        public decimal Smallest { get; set; }

        /// <summary>
        /// The average value of all items in the kpi.
        /// </summary>
        public decimal Average { get; set; }

        #endregion
    }

    /// <summary>
    /// The view model for a channel.
    /// </summary>
    public class ChannelViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private string _name;

        #endregion

        #region Private methods

        /// <summary>
        /// Called when a property has changed so that bindings can be notified.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
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
        /// <param name="name">The channel name.</param>
        public ChannelViewModel(string name)
        {
            _name = name;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The channel name.
        /// </summary>
        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? "" : _name; }
            set
            {
                if (value == _name) return;

                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The channel description.
        /// </summary>
        public string Description
        {
            get { return string.Format("{0} Channel", Name); }
        }

        /// <summary>
        /// The channel color.
        /// </summary>
        public Color ChannelColor
        {
            get { return ChannelColors.GetChannelColor(_name); }
        }

        /// <summary>
        /// The channel brush.
        /// </summary>
        public Brush ChannelBrush
        {
            get { return ChannelColors.GetChannelBrush(_name); }
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    /// <summary>
    /// The kpi view model.
    /// </summary>
    public class KpiViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private string _type;
        private string _channel;
        private string _total;
        private string _numberOf;
        private string _last;
        private string _largest;
        private string _smallest;
        private string _average;

        #endregion

        #region Private methods

        /// <summary>
        /// Called when a property value changes so bindings can be updated.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
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
        /// <param name="kpi">The kpi to seed values from.</param>
        public KpiViewModel(Kpi kpi)
        {
            if (kpi == null) return;

            _type = kpi.Type;
            _channel = kpi.Channel;
            _total = kpi.Total.ToString("C");
            _numberOf = kpi.NumberOf.ToString();
            _last = kpi.Last.ToString("C");
            _largest = kpi.Largest.ToString("C");
            _smallest = kpi.Smallest.ToString("C");
            _average = kpi.Average.ToString("C");
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the kpi.
        /// </summary>
        public string Name
        {
            get { return string.Format("{0} ({1})", _type, _numberOf); }
        }

        /// <summary>
        /// The type of kpi.
        /// </summary>
        public string Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;

                _type = value;
                
                NotifyPropertyChanged("Type");
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The channel color for the kpi.
        /// </summary>
        public Color ChannelColor
        {
            get { return ChannelColors.GetChannelColor(_channel); }
        }

        /// <summary>
        /// The channel brush for the kpi.
        /// </summary>
        public Brush ChannelBrush
        {
            get { return ChannelColors.GetChannelBrush(_channel); }
        }

        /// <summary>
        /// The channel for the kpi.
        /// </summary>
        public string Channel
        {
            get { return _channel; }
            set
            {
                if (value == _channel) return;
                    
                _channel = value;
                
                NotifyPropertyChanged("Channel");
                NotifyPropertyChanged("ChannelBrush");
            }
        }

        /// <summary>
        /// The kpi total.
        /// </summary>
        public string Total
        {
            get { return _total; }
            set
            {
                if (value == _total) return;

                _total = value;
                
                NotifyPropertyChanged("Total");
            }
        }

        /// <summary>
        /// Number of items represented by kpi.
        /// </summary>
        public string NumberOf
        {
            get { return _numberOf; }
            set
            {
                if (value == _numberOf) return;

                _numberOf = value;
                
                NotifyPropertyChanged("NumberOf");
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The largest value for kpi item.
        /// </summary>
        public string Largest
        {
            get { return _largest; }
            set
            {
                if (value == _largest) return;

                _largest = value;
                
                NotifyPropertyChanged("Highest");
            }
        }

        /// <summary>
        /// The smallest value for kpi item.
        /// </summary>
        public string Smallest
        {
            get { return _smallest; }
            set
            {
                if (value == _smallest) return;

                _smallest = value;
                
                NotifyPropertyChanged("Smallest");
            }
        }

        /// <summary>
        /// The last value added to the kpi.
        /// </summary>
        public string Last
        {
            get { return _last; }
            set
            {
                if (value == _last) return;

                _last = value;
                
                NotifyPropertyChanged("Last");
            }
        }

        /// <summary>
        /// The average value of the items in the kpi.
        /// </summary>
        public string Average
        {
            get { return _average; }
            set
            {
                if (value == _average) return;

                _average = value;
                   
                NotifyPropertyChanged("Average");
            }
        }

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}