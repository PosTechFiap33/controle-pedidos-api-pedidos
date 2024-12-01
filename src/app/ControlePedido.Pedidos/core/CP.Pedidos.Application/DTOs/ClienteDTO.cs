using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CP.Pedidos.Application.DTOs
{
    [DisplayName("Cliente")]
    public class ClienteDTO
    {
        [JsonPropertyName("nome")]
        public string Nome { get; private set; }
       
        [JsonPropertyName("cpf")]
        public string Cpf { get; private set; }
       
        [JsonPropertyName("email")]
        public string Email { get; private set; }

        public ClienteDTO(string nome, string cpf, string email)
        {
            Nome = nome;
            Cpf = cpf;
            Email = email;
        }
    }
}

