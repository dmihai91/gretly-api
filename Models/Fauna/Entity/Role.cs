using System;
using FaunaDB.Types;
using Newtonsoft.Json;

namespace Gretly.Models
{
    public class Role : FaunaEntity
    {
        [FaunaField("name")]
        public string Name { get; set; }

        [FaunaField("permissions")]
        public Permission[] Permissions { get; set; }

        [FaunaConstructor]
        [JsonConstructor]
        public Role(string id, string name, Permission[] permissions)
        {
            this.Id = id;
            this.Name = name;
            this.Permissions = permissions;
        }

        public Role(Role r)
        {
            this.Id = r.Id;
            this.Name = r.Name;
            this.Permissions = r.Permissions;
        }
    }
}