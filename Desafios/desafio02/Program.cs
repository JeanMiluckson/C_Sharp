/*
    1 - Crie um pequeno script em C# que pergunte ao usuario o nome de um item que ele encontrou no jogo
    2 - Pergunte a quantidade desse item que ele possui
    3 - Converta essa quantidade para int
    4 - Exiba uma mensagem final: "Voce adicionou[quantidade] x [item] ao seu inventario!"
*/

using System;

class Program
{
    static void Main()
    {
        Console.Write("Nome do item: ");
        string item = Console.ReadLine();
        Console.Write("quantidade: ");
        string quantidade = Console.ReadLine();
        int n_quantidade = Convert.ToInt32(quantidade);

        Console.WriteLine($"Voce adicionou {quantidade}x {item} ao seu iventario!");
    }
}