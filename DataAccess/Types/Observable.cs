using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace DataAccess.Types
{
    public abstract class Observable : INotifyPropertyChanged
    {
        /// <summary>
        /// The listeners
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool VerifyPropertyChange<T>(string propertyName, ref T oldValue, ref T newValue)
        {
            //No change occured if values is and was null
            if (oldValue == null && newValue == null) { return false; }
            //Change occured; set value and fire events
            if ((oldValue == null && newValue != null) || !oldValue.Equals((T)newValue))
            {
                oldValue = newValue;
                FirePropertyChanged(propertyName);
                return true;
            }
            //No change occured
            return false;
        }
        protected void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}