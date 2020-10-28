using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.Extensions.Shared;
using Microsoft.EnterpriseManagement.GenericForm;
using Microsoft.EnterpriseManagement.UI.DataModel;

namespace JKW.Tasks.User.GetRoles
{
    public class GetRoles : ConsoleCommand
    {
        internal const string MP_PROPERTY_ID_DOLLAR = "$Id$";
        internal const string MP_PROJECTION_TYPE_USER = "490ab845-b14c-1d91-c39f-bb9e8a350933";

        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            foreach (NavigationModelNodeBase node in nodes)
            {
                IDataItem dataItemUser = null;

                //Check if task was started from form
                bool startedFromForm = FormUtilities.Instance.IsNodeWithinForm(nodes[0]);
                string userName = "";

                //If started from form
                if (startedFromForm)
                {
                    //Get User object instance
                    dataItemUser = FormUtilities.Instance.GetFormDataContext(node);

                    userName = dataItemUser["UserName"] as string;
                    try
                    {
                        List<RoleItem> roleItems = Util.GetMemberships(userName);
                        if (roleItems.Count > 0)
                        {
                            ShowRoleGrid form = new ShowRoleGrid(roleItems);
                            form.Title = string.Format("Roles for {0}", userName);
                            form.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Roleset was empty!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                //Else started from view
                else
                {
                    IDataItem dataItemNode = node;
                    if (dataItemNode != null)
                    {
                        //Get User object instance from selected navigationNode
                        dataItemUser = ConsoleContextHelper.Instance.GetProjectionInstance((Guid)dataItemNode[MP_PROPERTY_ID_DOLLAR], new Guid(MP_PROJECTION_TYPE_USER));
                        userName = dataItemUser["UserName"] as string;
                        List<RoleItem> viewItems = Util.GetMemberships(userName);
                        if (viewItems.Count > 0)
                        {
                            ShowRoleGrid form = new ShowRoleGrid(viewItems);
                            form.Title = string.Format("Roles for {0}", userName);
                            form.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Roleset was empty");
                        }
                    }
                }
            }
        }
    }

}
