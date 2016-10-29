using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;

namespace Msra.SA.ETWAnalysisStudio.Common
{
    public class WPFTreeListView : ListView
    {
        #region Properties
        /// <summary>
        /// Internal collection of rows representing visible nodes, actually displayed in the ListView
        /// </summary>
        public ObservableCollectionAdv<WPFTreeNode> Rows
        {
            get;
            private set;
        }

        private ITreeModel _model;
        public ITreeModel Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    _root.Children.Clear();
                    Rows.Clear();
                    CreateChildrenNodes(_root);
                }
            }
        }

        private WPFTreeNode _root;
        internal WPFTreeNode Root
        {
            get { return _root; }
        }

        public ReadOnlyCollection<WPFTreeNode> Nodes
        {
            get { return Root.Nodes; }
        }

        internal WPFTreeNode PendingFocusNode
        {
            get;
            set;
        }

        public ICollection<WPFTreeNode> SelectedNodes
        {
            get
            {
                return SelectedItems.Cast<WPFTreeNode>().ToArray();
            }
        }

        public WPFTreeNode SelectedNode
        {
            get
            {
                if (SelectedItems.Count > 0)
                    return SelectedItems[0] as WPFTreeNode;
                else
                    return null;
            }
        }
        #endregion

        public WPFTreeListView()
        {
            Rows = new ObservableCollectionAdv<WPFTreeNode>();
            _root = new WPFTreeNode(this, null);
            _root.IsExpanded = true;
            ItemsSource = Rows;
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;

            //Windows Forms font size = WPF font size * 72.0 / 96.0.
            this.FontSize = 9 * 96.0 / 72.0;
            this.FontFamily = new FontFamily("Microsoft Sans Serif");
            this.Foreground = new SolidColorBrush(Colors.Black);
        }

        void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && PendingFocusNode != null)
            {
                var item = ItemContainerGenerator.ContainerFromItem(PendingFocusNode) as WPFTreeListViewItem;
                if (item != null)
                    item.Focus();
                PendingFocusNode = null;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WPFTreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WPFTreeListViewItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var ti = element as WPFTreeListViewItem;
            var node = item as WPFTreeNode;
            if (ti != null && node != null)
            {
                ti.Node = item as WPFTreeNode;
                base.PrepareContainerForItemOverride(element, node.Tag);
            }
        }

        internal void SetIsExpanded(WPFTreeNode node, bool value)
        {
            if (value)
            {
                if (!node.IsExpandedOnce)
                {
                    node.IsExpandedOnce = true;
                    node.AssignIsExpanded(value);
                    CreateChildrenNodes(node);
                }
                else
                {
                    node.AssignIsExpanded(value);
                    CreateChildrenRows(node);
                }
            }
            else
            {
                DropChildrenRows(node, false);
                node.AssignIsExpanded(value);
            }
        }

        internal void CreateChildrenNodes(WPFTreeNode node)
        {
            var children = GetChildren(node);
            if (children != null)
            {
                int rowIndex = Rows.IndexOf(node);
                node.ChildrenSource = children as INotifyCollectionChanged;
                foreach (object obj in children)
                {
                    WPFTreeNode child = new WPFTreeNode(this, obj);
                    child.HasChildren = HasChildren(child);
                    node.Children.Add(child);
                }

                Rows.InsertRange(rowIndex + 1, node.Children.ToArray());
            }
        }

        private void CreateChildrenRows(WPFTreeNode node)
        {
            int index = Rows.IndexOf(node);
            if (index >= 0 || node == _root) // ignore invisible nodes
            {
                var nodes = node.AllVisibleChildren.ToArray();
                Rows.InsertRange(index + 1, nodes);
            }
        }

        internal void DropChildrenRows(WPFTreeNode node, bool removeParent)
        {
            int start = Rows.IndexOf(node);
            if (start >= 0 || node == _root) // ignore invisible nodes
            {
                int count = node.VisibleChildrenCount;
                if (removeParent)
                    count++;
                else
                    start++;
                Rows.RemoveRange(start, count);
            }
        }

        private IEnumerable GetChildren(WPFTreeNode parent)
        {
            if (Model != null)
                return Model.GetChildren(parent.Tag);
            else
                return null;
        }

        private bool HasChildren(WPFTreeNode parent)
        {
            if (parent == Root)
                return true;
            else if (Model != null)
                return Model.HasChildren(parent.Tag);
            else
                return false;
        }

        internal void InsertNewNode(WPFTreeNode parent, object tag, int rowIndex, int index)
        {
            WPFTreeNode node = new WPFTreeNode(this, tag);
            if (index >= 0 && index < parent.Children.Count)
                parent.Children.Insert(index, node);
            else
            {
                index = parent.Children.Count;
                parent.Children.Add(node);
            }
            Rows.Insert(rowIndex + index + 1, node);
        }
    }
}