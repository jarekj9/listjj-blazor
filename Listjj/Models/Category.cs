using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace Listjj.Models
{
    public class Category : BaseEntity
    {
        public List<ListItem> ListItems { get; set; }

        public Category() 
        {
            this.Name = "";
            this.Description = "";
        }
    }


}