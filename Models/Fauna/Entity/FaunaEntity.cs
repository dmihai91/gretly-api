using System;
using System.Reflection;
using System.Threading.Tasks;
using FaunaDB.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gretly.Models
{
    // abstract class for FaundDB models
    public class FaunaEntity
    {
        public string Id { get; set; }

        // model indexer property

        public T ConvertValueToType<T>(Value v)
        {
            return Decoder.Decode<T>(v.At("data"));
        }

        public T GetQueryResult<T>(Task<T> queryExec)
        {
            return queryExec.GetAwaiter().GetResult();
        }

        public object ToJson()
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
        }

        public void SetProperty<T>(string propName, T propValue)
        {
            Type myType = this.GetType();
            PropertyInfo myPropInfo = myType.GetProperty(propName);
            myPropInfo.SetValue(this, propValue, null);
        }

        protected void Init()
        {
            this.SetDefaults();
            this.LinkRefs();
        }

        // should be override by the derived class
        protected virtual void SetDefaults()
        {

        }

        protected virtual void LinkRefs()
        {

        }
    }
}