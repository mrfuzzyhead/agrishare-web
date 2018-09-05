using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
        string Title { get; }
        DateTime DateCreated { get; set; }
        DateTime LastModified { get; set; }
        bool Deleted { get; set; }
    }
}
