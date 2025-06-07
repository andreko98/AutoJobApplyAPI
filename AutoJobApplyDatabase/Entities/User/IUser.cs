using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobApplyDatabase.Entities
{
    public interface IUser
    {
        int Id { get; set; }
        string Nome { get; set; }
        string Sobrenome { get; set; }
        string Email { get; set; }
        DateTime DataNascimento { get; set; }
        string Endereco { get; set; }
        string Sobre { get; set; }
        string CurriculoPath { get; set; }
    }
}
