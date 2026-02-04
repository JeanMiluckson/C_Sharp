/*
    1 - Altere a classe Item para incluir um Construtor que receba dois parametros: string nome e int dano
    2 - Dentro do construtor, atribua esses parametros aos campos da classe(this.Nome = nome;)
    3 - No seu Main, apague as linhas onde voce definia o nome e o dano manualmente
    4 - Agora, crie os itens passando os valores direto no paranteses new Item("faca", 60);
    5 - Mantenha a lista e o foreach para verificar se tudo continua funcionando
*/

using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        List<Item> inventario = new List<Item>();
        Item item1 = new Item("faca", 60);
        Item item2 = new Item("arco", 50);
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
        public Item(string nome, int dano)
        {
            this.Nome = nome;
            this.Dano = dano;
        }
        public void ExibirStatus()
        {
            Console.WriteLine($"Item: {Nome} | Dano: {Dano}");
        }
        
    }
}