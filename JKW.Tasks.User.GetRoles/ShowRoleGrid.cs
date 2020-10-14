using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Collections.Generic;

namespace JKW.Tasks.User.GetRoles
{
    class ShowRoleGrid : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal DataGrid grdRoles;
        public List<RoleItem> roleItems
        {
            get {
                List<RoleItem> ls = new List<RoleItem>();
                foreach (RoleItem item in grdRoles.Items)
                {
                    ls.Add(item);
                }
                return ls;
            }
            set {
                grdRoles.ItemsSource = value;
            }
        }
        
        public ShowRoleGrid(List<RoleItem> roleItems)
        {
            InitializeComponent();
            grdRoles.ItemsSource = roleItems;
        }

        public void Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    grdRoles = (DataGrid)target;
                    break;
                default:
                    _contentLoaded = true;
                    break;
            }
        }

        public void InitializeComponent()
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                Uri formUri = new Uri("/JKW.Tasks.User.GetRoles;component/RolesUI.xaml", UriKind.Relative);
                Application.LoadComponent(this, formUri);
            }
        }
    }
}
