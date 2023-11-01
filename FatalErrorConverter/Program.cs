using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class AnsiCodeGenerator
{
    private static readonly Random _rand = new Random();

    private static int LastColour = -1, LastBg = -1;

    private static readonly string[] TextFormats = {
        "1", "4"
    };

    private static readonly string[] TextColors = {
        "30", "31", "32", "33", "34", "35", "36", "37", "38"
    };

    private static readonly string[] BackgroundColors = {
        "40", "41", "42", "43", "44", "45", "46", "47", "48"
    };

    public static void Main()
    {
        Console.WriteLine("Enter a phrase:");
        string input = Console.ReadLine();
        while (input.Contains("  "))
            input = input.Replace("  ", " ");

        string result = GenerateAnsiBlock(input);
        File.WriteAllText("out.txt", result);

        Console.WriteLine(result);
        Console.ReadKey();
    }

    public static string GenerateAnsiBlock(string input)
    {
        string[] words = input.Split(' ');
        string output = "";
        StringBuilder sb = new StringBuilder();
        sb.Append("```ansi\n");

        // spazi per tutte le righe
        for (int i = 0; i < words.Length; i++)
        {
            output += new string(' ', _rand.Next(10, 16)) + GetRandomAnsiCode() + new string(' ', _rand.Next(2, 6)) + words[i] + new string(' ', _rand.Next(2, 6)) + "\u001b[0;48m";
        }

        bool inCode = false;
        int counter = 0, nextInsertPosition = _rand.Next(44, 55);
        foreach (char c in output)
        {
            if (c == '\u001b')
            {
                inCode = true;
                nextInsertPosition++;
            }

            if (inCode) // evita di spezzare codici ansi con degli a capo riga in mezzo
            {
                sb.Append(c);
                counter++;
                if (c == 'm' && counter >= nextInsertPosition)
                {
                    sb.Insert(sb.ToString().LastIndexOf("\u001b"), "\n", 1);
                    nextInsertPosition += _rand.Next(44, 55);  // Randomly choose the next insert position
                    inCode = false;
                }
            }
            else
            {
                if (counter == nextInsertPosition)
                {
                    sb.Append("\n");
                    nextInsertPosition += _rand.Next(44, 55);  // Randomly choose the next insert position
                }
                sb.Append(c);
                counter++;
            }
        }
        sb.Append("```");

        // aggiunge spazi nelle righe che iniziano con pochi spazi
        string[] result = sb.ToString().Split('\n');
        for (int i = 1; i < result.Length - 2; i++)
        {
            if (result[i].TrimEnd().Length < 50)
            {
                result[i] = new string(' ', _rand.Next(1, 16)) + result[i];
            }
        }

        return String.Join("\r\n", result);
    }

    public static string GetRandomAnsiCode()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("\u001b[0;");

        if (_rand.Next(0, 2) == 0)
        {
            sb.Append(TextFormats[_rand.Next(TextFormats.Length)]);
            sb.Append(";");
        }
        int colour;
        do
        {
            colour = _rand.Next(TextColors.Length);
        } while (colour == LastColour);
        sb.Append(TextColors[colour] + ";");
        LastColour = colour;
        do
        {
            colour = _rand.Next(BackgroundColors.Length);
        } while (colour == LastBg);
        sb.Append(BackgroundColors[colour]);
        LastBg = colour;

        sb.Append("m");
        return sb.ToString();
    }
}
