using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobApplyDatabase.Entities
{
    public interface IApplication
    {
        int Id { get; set; }
        int UserId { get; set; }
        int JobId { get; set; }
        DateTime AppliedAt { get; set; }
        string MessageSent { get; set; }
        ApplicationStatus Status { get; set; }

        User User { get; set; }
        Job Job { get; set; }
    }
}
