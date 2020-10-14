using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;


namespace JKW.Tasks.User.GetRoles
{
    class UserMembership
    {
        public string UserName { get; set; }
        public List<GroupPrincipal> MemberOf { get; set; }
    }
}
