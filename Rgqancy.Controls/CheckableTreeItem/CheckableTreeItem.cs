using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System;
using System.Xml;
using System.IO;

namespace Msra.SA.ETWAnalysisStudio.Common
{
    public class CheckableTreeItem : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        CheckableTreeItem _parent;

        #endregion // Data

        public CheckableTreeItem(string name)
        {
            this.Name = name;
            this.Children = new List<CheckableTreeItem>();
        }

        public void Initialize()
        {
            foreach (CheckableTreeItem child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        #region Properties

        public List<CheckableTreeItem> Children { get; private set; }

        public bool IsInitiallySelected { get; set; }

        public string Name { get; private set; }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child CheckableTreeItems.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
