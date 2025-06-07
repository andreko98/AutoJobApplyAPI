using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobApplyDatabase.Entities
{
    public class Application : IApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int JobId { get; set; }
        public DateTime AppliedAt { get; set; }
        public string MessageSent { get; set; }
        public string Status { get; set; } // e.g. Enviado, Erro, Pendente

        //public User User { get; set; }
        public virtual Job Job { get; set; }
    }
}
