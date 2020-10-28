using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows.Forms;
using System.DirectoryServices.AccountManagement;
using Microsoft.EnterpriseManagement.Security;
using Microsoft.EnterpriseManagement;


namespace JKW.Tasks.User.GetRoles
{
    class Util
    {
        public static UserMembership GetUserGroupMembershipRecursive(string username)
        {

            Dictionary<string, GroupPrincipal> ADGroupCache = new Dictionary<string, GroupPrincipal>();
            Dictionary<string, GroupPrincipal> UserGroups = new Dictionary<string, GroupPrincipal>();
            PrincipalContext _pc = new PrincipalContext(ContextType.Domain);

            void findPath(string currentGroup)
            {
                if (currentGroup != null)
                {
                    try
                    {
                        GroupPrincipal group;
                        if (!UserGroups.ContainsKey(currentGroup))
                        {
                            if (ADGroupCache.ContainsKey(currentGroup))
                            {
                                group = ADGroupCache[currentGroup];
                            }
                            else
                            {
                                group = GroupPrincipal.FindByIdentity(_pc, currentGroup);
                                if (group != null)
                                {
                                    ADGroupCache.Add(group.DistinguishedName, group);
                                }
                            }
                            if (group != null)
                            {
                                UserGroups.Add(currentGroup, group);
                                foreach (Principal p in group.GetGroups())
                                {
                                    if (p != null)
                                    {
                                        findPath(p.DistinguishedName);
                                    }

                                }
                            }

                        }
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            try
            {
                UserPrincipal userObject = UserPrincipal.FindByIdentity(_pc, username);
                if (userObject != null)
                {
                    PrincipalSearchResult<Principal> groups = userObject.GetGroups();
                    if (groups != null)
                    {
                        foreach (Principal p in groups)
                        {
                            if (p is GroupPrincipal && p != null)
                            {

                                findPath(p.DistinguishedName);
                            }
                        }
                    }

                    List<GroupPrincipal> userMemberOf = new List<GroupPrincipal>();

                    foreach (GroupPrincipal g in UserGroups.Values)
                    {
                        userMemberOf.Add(g);
                    }

                    UserMembership user = new UserMembership()
                    {
                        UserName = userObject.SamAccountName,
                        MemberOf = userMemberOf
                    };

                    return user;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return new UserMembership();
        }
        public static List<RoleItem> GetMemberships(string username)
        {
            //Get the server name to connect to and connect to the server 
            string strServerName = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\System Center\\2010\\Service Manager\\Console\\User Settings", "SDKServiceMachine", "localhost").ToString();
            EnterpriseManagementGroup _emg = EnterpriseManagementGroup.Connect(strServerName);

            UserMembership userGroups = GetUserGroupMembershipRecursive(username);
            if (userGroups != null)
            {

                List<RoleItem> items = new List<RoleItem>();
                if (userGroups != null)
                {

                    IList<UserRole> roles = _emg.Security.GetUserRoles();
                    if (roles != null)
                    {

                        foreach (UserRole role in roles)
                        {
                            if (role.Users != null)
                            {
                                foreach (string user in role.Users)
                                {
                                    if (userGroups.MemberOf != null)
                                    {
                                        foreach (GroupPrincipal group in userGroups.MemberOf)
                                        {
                                            if (group != null)
                                            {
                                                if (user != null)
                                                {
                                                    if (user.EndsWith(group.Name))
                                                    {
                                                        RoleItem item = new RoleItem()
                                                        {
                                                            Group = group.Name,
                                                            UserRole = role.DisplayName
                                                        };
                                                        items.Add(item);
                                                    }
                                                }
                                                else
                                                {
                                                    return items;
                                                }
                                            }
                                            else
                                            {
                                                return items;
                                            }
                                        }
                                    }
                                    if(user.EndsWith(username))
                                    {
                                        RoleItem item = new RoleItem()
                                        {
                                            Group = "**Explicitly Added**",
                                            UserRole = role.DisplayName
                                        };
                                        items.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                return items;
                            }
                        }
                        return items;
                    }
                    else { return items; }

                }
                else
                {
                    return items;
                }
            }
            else
            {
                return new List<RoleItem>();
            }

        }
    }
}

