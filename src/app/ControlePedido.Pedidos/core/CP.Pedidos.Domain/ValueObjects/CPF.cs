using System.Collections.Generic;
using CP.Pedidos.Domain.Base;

namespace CP.Pedidos.Domain.ValueObjects
{
    public class CPF : ValueObject
    {
        public string Numero { get; private set; }

        public CPF(string numero)
        {
            Numero = numero;

            ValidateValueObject();
        }

        protected CPF() { }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Numero;
        }

        private void ValidateValueObject()
        {
            AssertionConcern.AssertArgumentExactlyLength(Numero, 11, "Cpf deve conter 11 caracters!");

            var cpfValido = ValidarCpf(Numero);
            AssertionConcern.AssertArgumentTrue(cpfValido, "Cpf inválido!");
        }

        public static bool ValidarCpf(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            bool todosDigitosIguais = true;
            for (int i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != cpf[0])
                {
                    todosDigitosIguais = false;
                    break;
                }
            }
            if (todosDigitosIguais)
                return false;

            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += (10 - i) * int.Parse(cpf[i].ToString());
            }
            int primeiroDigito = 11 - (soma % 11);
            if (primeiroDigito >= 10)
                primeiroDigito = 0;

            if (int.Parse(cpf[9].ToString()) != primeiroDigito)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += (11 - i) * int.Parse(cpf[i].ToString());
            }
            int segundodigito = 11 - (soma % 11);
            if (segundodigito >= 10)
                segundodigito = 0;

            if (int.Parse(cpf[10].ToString()) != segundodigito)
                return false;

            return true;
        }
    }
}

