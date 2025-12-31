using Numerics.Data;
using Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NumericControls.Distributions.Multivariate
{
    public class BivariateEmpirical : INotifyPropertyChanged
    {

        private BivariateEmpirical _bivariateEmpiricalCDF;

        public BivariateEmpirical BivariateEmpiricalCDF
        {
            get { return _bivariateEmpiricalCDF; }
            set 
            {
                _bivariateEmpiricalCDF = value;
                RaisePropertyChange(nameof(BivariateEmpiricalCDF));
            }
        }

        /// <summary>
        /// Return the array of X1 values (distribution 1). Points On the cumulative curve are specified
        /// with increasing value and increasing probability.
        /// </summary>
        public ObservableCollection<double> X1Values { get; set; }

        /// <summary>
        /// Return the array of X2 values (distribution 2). Points on the cumulative curve are specified
        /// with increasing value and increasing probability.
        /// </summary>
        public ObservableCollection<double> X2Values { get; set; }


        /// <summary>
        /// Raise event when a property changes. 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise property changed event.
        /// </summary>
        /// <param name="propertyname">Name of property that changed.</param>
        public void RaisePropertyChange(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

    }
}
