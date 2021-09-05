using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Listjj.Models
{
    public class Category : BaseEntity
    {
        [JsonIgnore]
        public List<ListItem> ListItems { get; set; }

        public Category() 
        {
            this.Name = "";
            this.Description = "";
        }
    }


}