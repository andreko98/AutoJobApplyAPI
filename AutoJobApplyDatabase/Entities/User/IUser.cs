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
