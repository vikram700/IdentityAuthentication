using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth.Models
{
    public class EditRoleModel
    {
        public EditRoleModel()
        {
            this.Users = new List<string>();
        }

        public string Id { get; set; }
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
