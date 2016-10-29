using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace Msra.SA.ETWAnalysisStudio.Common
{
    public class WPFTreeListViewItem : ListViewItem, INotifyPropertyChanged
    {
        #region Properties

        private WPFTreeNode _node;
        public WPFTreeNode Node
        {
            get { return _node; }
            internal set
            {
                _node = value;
                OnPropertyChanged("Node");
            }
        }

        #endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Node != null)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        e.Handled = true;
                        if (!Node.IsExpanded)
                        {
                            Node.IsExpanded = true;
                            ChangeFocus(Node);
                        }
                        else if (Node.Children.Count > 0)
                            ChangeFocus(Node.Children[0]);
                        break;

                    case Key.Left:

                        e.Handled = true;
                        if (Node.IsExpanded && Node.IsExpandable)
                        {
                            Node.IsExpanded = false;
                            ChangeFocus(Node);
                        }
                        else
                            ChangeFocus(Node.Parent);
                        break;

                    case Key.Subtract:
                        e.Handled = true;
                        Node.IsExpanded = false;
                        ChangeFocus(Node);
                        break;

                    case Key.Add:
                        e.Handled = true;
                        Node.IsExpanded = true;
                        ChangeFocus(Node);
                        break;
                }
            }

            if (!e.Handled)
                base.OnKeyDown(e);
        }

        private void ChangeFocus(WPFTreeNode node)
        {
            var tree = node.Tree;
            if (tree != null)
            {
                var item = tree.ItemContainerGenerator.ContainerFromItem(node) as WPFTreeListViewItem;
                if (item != null)
                    item.Focus();
                else
                    tree.PendingFocusNode = node;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}