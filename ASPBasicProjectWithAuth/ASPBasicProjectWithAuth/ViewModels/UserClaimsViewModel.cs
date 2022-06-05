using System.Collections.Generic;

namespace ASPBasicProjectWithAuth.ViewModels
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Claims = new List<UserClaim>();
        }
        public string UserID { get; set; }
        public List<UserClaim>  Claims { get; set; }
    }
}
