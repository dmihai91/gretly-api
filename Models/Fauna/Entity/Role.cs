using System;
using FaunaDB.Types;
using Newtonsoft.Json;

namespace GretlyStudio.Models
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
            Id = id;
            Name = name;
            Permissions = permissions;
        }

        public Role(Role r)
        {
            Id = r.Id;
            Name = r.Name;
            Permissions = r.Permissions;
        }
    }
}