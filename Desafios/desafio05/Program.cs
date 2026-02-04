/*
    1 - No topo do seu codigo, adicione: using System.Collections.Generic;
    2 - No seu Main, crie uma lista chamada inventario: List<Item> inventario = new List<Item>();
    3 - Adicione o seu item e o seu item2 a essa lista usando o metodo .Add()
    4 - Use um laço foreach para percorrer a lista e chamar o metodo .ExibirStatus() de cada item dentro dela
*/

using System;
using System.Collections.Generic;
using System.IO.Compression;

class Program
{
    static void Main()
    {
        List<Item> inventario = new List<Item>();
        Item item1 = new Item();
        Item item2 = new Item();
        item1.Nome = "faca";
        item1.Dano = 60;
        item2.Nome = "arco";
        item2.Dano = 50;
        inventario.Add(item1);
        inventario.Add(item2);

        foreach (Item i in inventario)
        {
            i.ExibirStatus();
        }
    }
    class Item
    {
       public string Nome;
       public int Dano;
       public void ExibirStatus()
        {
            Console.WriteLine($"Item: {Nome} | Dano: {Dano}");
        }
    }
}