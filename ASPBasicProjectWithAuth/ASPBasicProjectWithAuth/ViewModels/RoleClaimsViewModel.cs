using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPBasicProjectWithAuth.ViewModels
{
    public class RoleClaimsViewModel
    {
        public RoleClaimsViewModel()
        {
            RoleClaims = new List<UserClaim>();
        }
        public string RoleID { get; set; }

        public IList<UserClaim> RoleClaims { get; set; }
    }
}
