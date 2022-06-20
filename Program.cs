using System.Security.Cryptography;
using System.Text;

namespace HashCompare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string originalHash = String.Empty;
            if (!CheckArgs(args))
            {
                PrintHelp();
                return;
            }

            string hashAlgorithmString = args[0];
            HashAlgorithm hashAlgorithm = GetHashAlgorithm(hashAlgorithmString);
            if (hashAlgorithm == null)
            {
                return;
            }
            string hash = ComputeHash(hashAlgorithm, $"{Environment.CurrentDirectory}\\{args[1]}").ToLower();
            Console.WriteLine(hash);
            if (args.Length == 2)
            {
                return;
            }
            if (args[2].Equals("-file"))
            {
                string filePath = $"{Environment.CurrentDirectory}\\{args[3]}";
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Die Datei {Environment.CurrentDirectory}\\{args[3]} existiert nicht!");
                    return;
                }
                originalHash = ReadHashFromFile(filePath).TrimEnd('\r', '\n').Trim().ToLower();
            }
            else
            {
                originalHash = args[3];
            }
            Console.WriteLine(originalHash);
            Console.WriteLine();
            if (hash.Equals(originalHash))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Hashes sind identisch!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hashes sind NICHT identisch!");
                Console.ResetColor();
            }

        }

        private static void PrintHelp()
        {
            Console.WriteLine("HashCompare berechnet Hashwerte und vergleicht diese mit einem " +
                "gegebenen Referenzwert");
            Console.WriteLine();
            Console.WriteLine("Verwendung: hashcompare <hashalgorithm> <fileToHash>");
            Console.WriteLine("   oder   : hashcompare <hashalgorithm> <fileToHash> -hash <hashValue>");
            Console.WriteLine("   oder   : hashcompare <hashalgorithm> <fileToHash> -file <fileThatContainsOnlyTheHashValue.txt>");
            Console.WriteLine();
            Console.WriteLine("Beispiel 1: hashcompare sha256 test.iso -file test.txt");
            Console.WriteLine("Beispiel 2: hashcompare sha256 test.iso -hash e739317677c2261ae746eee5f1f0662aa319ad0eff260d4eb7055d7c79d10952");
        }

        private static bool CheckArgs(string[] args)
        {
            if (args.Length != 4 && args.Length != 2)
            {
                Console.WriteLine("Angaben fehlen");
                return false;
            }
            if (args.Length == 4 && !(args[2].Equals("-file") || args[2].Equals("-hash")))
            {
                Console.WriteLine("Ungültiger Parameter: " + args[2]);
                return false;
            }
            if (args.Length == 4 && args[2].Equals("-file") && !(args[3].EndsWith(".txt")))
            {
                Console.WriteLine("Zum Auswerten einer Datei muss diese eine .txt-Datei sein!");
                return false;
            }
            return true;
        }

        private static string ReadHashFromFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        private static HashAlgorithm GetHashAlgorithm(string hashAlgorithmString)
        {
            HashAlgorithm hashAlgorithm = null;
            switch (hashAlgorithmString.ToLower())
            {
                case "sha256":
                    hashAlgorithm = SHA256.Create();
                    break;
                case "sha384":
                    hashAlgorithm = SHA384.Create();
                    break;
                case "sha512":
                    hashAlgorithm = SHA512.Create();
                    break;
                case "md5":
                    Console.WriteLine("Achtung: Verfahren ist unsicher!");
                    hashAlgorithm = MD5.Create();
                    break;
                case "sha1":
                    Console.WriteLine("Achtung: Verfahren ist unsicher!");
                    hashAlgorithm = SHA1.Create();
                    break;
                default:
                    Console.WriteLine("Hashverfahren nicht unterstützt...");
                    break;
            }
            return hashAlgorithm;
        }

        private static string ComputeHash(HashAlgorithm hashAlgorithm, string filePath)
        {
            byte[] hash = null;
            if (File.Exists(filePath))
            {
                FileInfo fi = new(filePath);
                using (FileStream fs = fi.Open(FileMode.Open))
                {
                    try
                    {
                        fs.Position = 0;
                        hash = hashAlgorithm.ComputeHash(fs);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"I/O Exception: {e.Message}");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Access Exception: {e.Message}");
                    }
                }
            }
            return hash == null ? String.Empty : ConvertByteArrayToString(hash);
        }

        private static string ConvertByteArrayToString(byte[] array)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append($"{array[i]:X2}");
            }
            return sb.ToString();
        }
    }
}