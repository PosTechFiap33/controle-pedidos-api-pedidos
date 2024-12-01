using System.Collections.Generic;
using CP.Pedidos.Domain.Base;

namespace CP.Pedidos.Domain.ValueObjects
{
    public class Imagem : ValueObject
    {
        public string Url { get; private set; }
        public string Extensao { get; private set; }
        public string Nome { get; private set; }
        public string UrlExibicao { get {return $"{Url}/{Nome}.{Extensao}";} }
        
        protected Imagem() {}

        public Imagem(string url, string extensao, string nome)
        {
            Url = url;
            Extensao = extensao;
            Nome = nome;
            ValidateValueObject();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
            yield return Extensao;
            yield return Nome;
        }

        private void ValidateValueObject(){
            AssertionConcern.AssertArgumentNotEmpty(Url, "Url da imagem não pode estar vazia!");
            AssertionConcern.AssertArgumentNotEmpty(Nome, "Nome da imagem não pode estar vazio!");
            AssertionConcern.AssertArgumentNotEmpty(Extensao, "Extensão da imagem não pode estar vazio!");
            AssertionConcern.AssertArgumentLength(Nome, 100, "O nome não pode ultrapassar 100 caracters!");
            AssertionConcern.AssertArgumentLength(Extensao, 10, "A extensão não pode ultrapassar 100 caracters!");

        }

    }
}
