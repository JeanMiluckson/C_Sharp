/*
    1 - Na sua classe Item, adicione um metodo chamado ExibirStatus
    2 - Dentro desse metodo, coloque o Console.WriteLine que mostra o nome e o dano
    3 - No seu Main, apos atribuir os valores ao objeto, chame o metodo assi: item.ExibirStatus();
    4 - Desafio Extra: Crie um segundo objeto com valores diferente e chame o ExibirStatus dele também
*/ 
using System;

class Program
{
    static void Main()
    {
        Item item = new Item();
        item.Nome = "faca";
        item.Dano = 30;

        Item item2 = new Item();
        item2.Nome = "Arco Longo";
        item2.Dano = 45;

        item.ExibirStatus();
        item2.ExibirStatus();
    }
    class Item
    {
        public string Nome;
        public int Dano;
        public void ExibirStatus()
        {
            Console.WriteLine($"Nome: {Nome} | Dano: {Dano}");
        }
    }
}