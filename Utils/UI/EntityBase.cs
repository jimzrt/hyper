using System;
using System.ComponentModel;

namespace Utils.UI
{
    [Serializable]
    public class EntityBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Same as Notify
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            Notify(propertyName);
        }

        /// <summary>
        /// Notify Property Changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void Notify(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
