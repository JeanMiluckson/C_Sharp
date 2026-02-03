/*
    1 - Crie uma classe chamada Item
    2 - Dentro dela, coloque dois campos publicos: string Nome e int Dano
    3 - No seu Main, instancie(crie) um novo objeto dessa classe
    4 - Atribua um nome e um valor de dano a esse objeto
    5 - Exiba o nome do item acessando a propriedade do objeto
*/

using System;

class Program
{
    static void Main()
    {
        
       Item item = new Item();
        item.Nome = "Espada";
        item.Dano = 60;
        Console.WriteLine($"Item: {item.Nome} | Dano: {item.Dano}");
    }
    class Item
    {
        public string Nome;
        public int Dano;
    }
}