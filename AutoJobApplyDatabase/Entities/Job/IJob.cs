using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobApplyDatabase.Entities
{
    public interface IJob
    {
        int Id { get; set; }
        string Title { get; set; }
        string Company { get; set; }
        string Location { get; set; }
        string Description { get; set; }
        string Url { get; set; }
        DateTime DatePosted { get; set; }
    }
}
