using System;
using System.Numerics; //BigInteger
using System.Diagnostics;  //TimeSet
using System.Text;
using String = System.String; //TextWork


class ConvertArr
{
    public static string EncodeBigIntegerArray(BigInteger[] numbers)
    {
        // Преобразуем числа в строки и объединяем через разделитель
        string joinedNumbers = string.Join("|", numbers.Select(n => n.ToString()));

        // Кодируем объединенную строку в base64
        byte[] bytes = Encoding.UTF8.GetBytes(joinedNumbers);
        return Convert.ToBase64String(bytes);
    }

    public static BigInteger[] DecodeBigIntegerArray(string base64Encoded)
    {
        // Декодируем строку base64 в байты
        byte[] bytes = Convert.FromBase64String(base64Encoded);

        // Преобразуем байты в строку и разделяем через разделитель
        string joinedNumbers = Encoding.UTF8.GetString(bytes);
        string[] numberStrings = joinedNumbers.Split('|');

        // Преобразуем строки обратно в BigInteger
        BigInteger[] numbers = new BigInteger[numberStrings.Length];
        for (int i = 0; i < numberStrings.Length; i++)
        {
            numbers[i] = BigInteger.Parse(numberStrings[i]);
        }

        return numbers;
    }
    
    public static string Utf32ToString(BigInteger[] utf32CodePoints)
    {
        // Преобразование BigInteger в массив байтов
        byte[] utf32Bytes = new byte[utf32CodePoints.Length * sizeof(int)];
        for (int i = 0; i < utf32CodePoints.Length; i++)
        {
            byte[] codePointBytes = utf32CodePoints[i].ToByteArray();
            
            // Убираем лишние байты, если они есть
            int startIndex = Math.Max(0, codePointBytes.Length - sizeof(int));
            int length = Math.Min(sizeof(int), codePointBytes.Length);

            Buffer.BlockCopy(codePointBytes, startIndex, utf32Bytes, i * sizeof(int), length);
        }

        // Декодирование байтов в строку
        string resultString = Encoding.UTF32.GetString(utf32Bytes);

        return resultString;
    }
}

class PrimeGenerator
{
    private static Random random = new Random();

    public static BigInteger GeneratePrime(int bitSize)
    {
        while (true)
        {
            BigInteger candidate = GenerateRandomOddNumber(bitSize);

            if (IsProbablyPrime(candidate, 5)) // Второй параметр - количество итераций теста Миллера-Рабина
                return candidate;
        }
    }

    private static BigInteger GenerateRandomOddNumber(int bitSize)
    {
        byte[] data = new byte[bitSize / 8];
        random.NextBytes(data);

        // Устанавливаем старший и младший бит, чтобы число было нечетным
        data[0] |= 0x01;
        data[data.Length - 1] |= 0x01;

        return new BigInteger(data);
    }

    private static bool IsProbablyPrime(BigInteger n, int k)
    {
        if (n <= 1)
            return false;
        if (n == 2 || n == 3)
            return true;
        if (n % 2 == 0)
            return false;

        BigInteger d = n - 1;
        int s = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            s++;
        }

        for (int i = 0; i < k; i++)
        {
            BigInteger a = GenerateRandomBigInteger(2, n - 2);
            BigInteger x = BigInteger.ModPow(a, d, n);
            if (x == 1 || x == n - 1)
                continue;

            for (int j = 1; j < s; j++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                    break;
            }

            if (x != n - 1)
                return false;
        }

        return true;
    }

    private static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max)
    {
        byte[] data = new byte[max.ToByteArray().Length];
        random.NextBytes(data);

        BigInteger value = new BigInteger(data);
        value = BigInteger.Abs(value % (max - min)) + min;

        return value;
    }
}

class RSA
{
    private BigInteger p = 0;
    private BigInteger q = 0;
    private BigInteger n = 0;
    private BigInteger fn = 0;
    private BigInteger e = 0;
    private BigInteger d = 0;
    private BigInteger m = 0;
    private BigInteger c = 0;
    private BigInteger k = 2;
    private int lengthKey = 2048;

    //Конструктор по умолч.

    public RSA() {}
    
    public RSA(int numBit)
    {
        lengthKey = numBit;
        GenerateKeys();
    }

    //Генерация n-битного простого числа(долго)
    public BigInteger GeneratePrimeKey(int numBit)
    {
        bool test = true;
        
        int[] randomBits = GenerateRandomBits(numBit);
        BigInteger bigIntegerValue;
        do
        {
            randomBits = GenerateRandomBits(numBit);
            bigIntegerValue = ConvertToBigInteger(randomBits);
            test = IsPrimeBig(bigIntegerValue, 5);
        }
        while (!test);
        
        
        bigIntegerValue = ConvertToBigInteger(randomBits);
        // Console.Write($"bigIntegerValue");
        
        return bigIntegerValue;

    } 
    
    public bool IsPrimeBig(BigInteger num, int key)
    {
        if (num <= 1)
            return false;
        if (num <= 3)
            return true;
        if (num % 2 == 0)
            return false;

        BigInteger d = num - 1;
        int s = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            s++;
        }

        Random rand = new Random();

        for (int i = 0; i < key; i++)
        {
            BigInteger a = RandomBigInteger(2, num - 2, rand);
            BigInteger x = BigInteger.ModPow(a, d, num);
            if (x == 1 || x == num - 1)
                continue;

            for (int j = 1; j < s; j++)
            {
                x = BigInteger.ModPow(x, 2, num);
                if (x == num - 1)
                    break;
            }

            if (x != num - 1)
                return false;
        }

        return true;
    }
    
    private BigInteger RandomBigInteger(BigInteger min, BigInteger max, Random rand)
    {
        byte[] data = new byte[max.ToByteArray().Length];
        rand.NextBytes(data);

        BigInteger value = new BigInteger(data);
        value = BigInteger.Abs(value % (max - min)) + min;

        return value;
    }
    
    int[] GenerateRandomBits(int n)
    {
        Random random = new Random();
        int[] bits = new int[n];

        for (int i = 0; i < n; i++)
        {
            bits[i] = random.Next(2); // генерация случайных 0 или 1
        }

        return bits;
    }
    BigInteger ConvertToBigInteger(int[] bits)
    {
        BigInteger result = 0;

        for (int i = 0; i < bits.Length; i++)
        {
            result = result << 1; // сдвигаем влево на 1 бит
            result = result | bits[i]; // устанавливаем последний бит
        }

        return result;
    }
    
    bool IsPrime(BigInteger number)
    {
        if (number <= 1)
            return false;

        if (number == 2 || number == 3)
            return true;

        if (number % 2 == 0 || number % 3 == 0)
            return false;

        for (BigInteger i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }

        return true;
    }
    
    BigInteger Euler()
    {
        return (p - 1) * (q - 1);
    }
    
    BigInteger GeneratePrimeNumber()
    {
        Random rand = new Random();
        BigInteger number;

        do
        {
            Console.Write("No! ");
            number = rand.Next(1000000000, 2000000000); // Вы можете изменить диапазон по вашему усмотрению
        }
        while (!IsPrime(number));

        return number;
    }
    
    BigInteger Exponent()
    {
        e = 0;
        for (ulong i = 2; i < fn; i++)
        {
            
            if (IsPrime(i) && BigInteger.GreatestCommonDivisor(i, fn) == 1)
            {
                e = i;
                break;
            }
        }

        return e;
    }
    
    void GeneratePrivateKey()
    {
        do
        {
            GeneratePublicKey();
            d = ((k * fn) + 1) / e;
        } 
        while ((d * e) % fn != 1);
    }
    
    void GeneratePublicKey()
    {
        // p = GeneratePrimeNumber();
        // q = GeneratePrimeNumber();
        
        p = GeneratePrimeKey(lengthKey/2);
        q = GeneratePrimeKey(lengthKey/2);
        
        // p = PrimeGenerator.GeneratePrime(lengthKey/2);
        // q = PrimeGenerator.GeneratePrime(lengthKey/2);
        
        n = p * q;
        fn = Euler();
        e = Exponent();
    }

    public void GenerateKeys()
    {
        GeneratePrivateKey();
    }

    public void PrintKeys()
    {
        // GenerateKeys();
        bool condition = (d * e) % fn == 1;
        Console.WriteLine($"\np = {p}");
        Console.WriteLine($"\nq = {q}");
        Console.WriteLine($"\nn = {n}");
        Console.WriteLine($"\nf(n) = {fn}");
        Console.WriteLine($"\ne = {e}");
        Console.WriteLine($"\nd = {d}");
        Console.WriteLine($"\nde ≡ 1 (mod f(n)): {condition}");
    }
    
    BigInteger StringToBigInteger(string? text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        
        BigInteger bigInteger = new BigInteger(bytes);
        return bigInteger;
    }
    
    string BigIntegerToString(BigInteger num)
    {
        byte[] resultBytes = num.ToByteArray();
        string resultText = Encoding.UTF8.GetString(resultBytes);
        return resultText;
    }

    public void Encoder()
    {
        Console.WriteLine($"\nm на входе = {m}");
        c = BigInteger.ModPow(m, e, n);
        Console.WriteLine($"c = {c}\n");
    }
    
    public BigInteger Encoder(string? input)
    {
        c = BigInteger.ModPow(StringToBigInteger(input), e, n);
        return c;
    }

    public string Decoder()
    {
        m = BigInteger.ModPow(c, d, n);
        Console.WriteLine($"m на выходе = {m}");
        return BigIntegerToString(m);
    }
    
    public string Decoder(BigInteger inputC)
    {
        string outp = BigIntegerToString(BigInteger.ModPow(inputC, d, n));
        return outp;
    }

    public string[] ExportPrivateKey()
    {
        string nInBase64 = Convert.ToBase64String(n.ToByteArray());
        string dKeyInBase64 = Convert.ToBase64String(d.ToByteArray());
        string[] lines = new string[] { "-----BEGIN RSA PRIVATE KEY-----", nInBase64, dKeyInBase64, "-----END RSA PRIVATE KEY-----" };
        return lines;
    }
    
    public string[] ExportPublicKey()
    {
        
        string exponentInBase64 = Convert.ToBase64String(e.ToByteArray());
        string nKeyInBase64 = Convert.ToBase64String(n.ToByteArray());
        string[] lines = new string[] { "-----BEGIN RSA PUBLIC KEY-----", exponentInBase64, nKeyInBase64, "-----END RSA PUBLIC KEY-----" };
        return lines;
    }

    public void ImportPrivate(string path)
    {
        string[] line = File.ReadAllLines(path);
        
        // Декодирование base64-строки в массив байт
        byte[] byteArrayN = Convert.FromBase64String(line[1]);
        // Преобразование массива байт в BigInteger
        BigInteger bigIntegerValueN = new BigInteger(byteArrayN);
        
        // Декодирование base64-строки в массив байт
        byte[] byteArrayD = Convert.FromBase64String(line[2]);
        // Преобразование массива байт в BigInteger
        BigInteger bigIntegerValueD = new BigInteger(byteArrayD);

        n = bigIntegerValueN;
        d = bigIntegerValueD;

    }
    
    public void ImportPublic(string path)
    {
        string[] line = File.ReadAllLines(path);
        
        // Декодирование base64-строки в массив байт
        byte[] byteArrayExp = Convert.FromBase64String(line[1]);
        // Преобразование массива байт в BigInteger
        BigInteger bigIntegerValueExp = new BigInteger(byteArrayExp);
        
        // Декодирование base64-строки в массив байт
        byte[] byteArrayN = Convert.FromBase64String(line[2]);
        // Преобразование массива байт в BigInteger
        BigInteger bigIntegerValueN = new BigInteger(byteArrayN);

        e = bigIntegerValueExp;
        n = bigIntegerValueN;

    }


    public BigInteger[] EncryptArr(string input)
    {
        BigInteger[] encryptedArray = new BigInteger[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            // Console.Write($"{Char.ConvertToUtf32(input, i)} = ");
            encryptedArray[i] = BigInteger.ModPow(Char.ConvertToUtf32(input, i), e, n);
            // Console.WriteLine($"{encryptedArray[i]} ");
        }

        return encryptedArray;
    }
    
    
    public string DecryptArr(BigInteger[] array)
    {
        BigInteger[] decryptedArray = new BigInteger[array.Length];
        
        for (int i = 0; i < array.Length; i++)
        {
            // Console.Write($"{array[i]} = ");
            
            decryptedArray[i] = BigInteger.ModPow(array[i], d, n);
            
            // Console.WriteLine($"{decryptedArray[i]}");
        }

        
        return ConvertArr.Utf32ToString(decryptedArray);
    }
    
}

class Interface
{ 
    public static void MainMenu()
    {
        int selectedItemIndex = 0;
        string[] menuItems = { "Показать ВСЁ", "Encrypt создавая пару ключей", "Encrypt импортируя PUBLIC-ключ", "Decrypt импортируя PRIVATE-ключ", "ПШ(Посимвольное шифрование)", "ПШ Encrypt", "ПШ Decrypt"};
        while (true)
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("\nВыберите(стрелками)\n");
            
            for (int i = 0; i < menuItems.Length; i++)
            {
                Console.Write("\t");
                if (i == selectedItemIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(menuItems[i]);

                Console.ResetColor();
            }
            
            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = (selectedItemIndex == 0) ? menuItems.Length - 1 : selectedItemIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex = (selectedItemIndex == menuItems.Length - 1) ? 0 : selectedItemIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    if (selectedItemIndex == 0)
                    {
                        FullDemonstrate();
                    }
                    else if (selectedItemIndex == 1)
                    {
                        CreateKeysEncode();
                    }
                    else if (selectedItemIndex == 2)
                    {
                        ImpPublicKey();
                    }
                    else if (selectedItemIndex == 3)
                    {
                        ImpPrivateKey();
                    }
                    else if (selectedItemIndex == 4)
                    {
                        Utf8EncodingArray();
                    }
                    break;
            }
        }
    }

    public static void FullDemonstrate()
    {
        Console.CursorVisible = false;
        bool Menu = true;
        int input = 2048;
        int selectedItemIndex = 0;
        string[] menuItems = { "32 битa","64 бит","128 бит","256 бит", "512 бит", "1024 бит", "2048 бит", "4096 бит" };

        while (Menu)
        {
            Console.Clear();
            Console.WriteLine("\nРазмер ключа: ");
            
            for (int i = 0; i < menuItems.Length; i++)
            {
                Console.Write("\t");
                if (i == selectedItemIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(menuItems[i]);

                Console.ResetColor();
            }
            
            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = (selectedItemIndex == 0) ? menuItems.Length - 1 : selectedItemIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex = (selectedItemIndex == menuItems.Length - 1) ? 0 : selectedItemIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    if (selectedItemIndex == 0)
                    {
                        input = 32;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 1)
                    {
                        input = 64;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 2)
                    {
                        input = 128;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 3)
                    {
                        input = 256;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 4)
                    {
                        input = 512;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 5)
                    {
                        input = 1024;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 6)
                    {
                        input = 2048;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 7)
                    {
                        input = 4096;
                        Menu = false;
                    }
                    else
                    {
                        input = 2048;
                        Menu = false;
                    }
                    break;
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Подождите, ключ генерируется!");
        Console.ResetColor();
        
        Console.CursorVisible = true;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        RSA rsaK = new RSA(input);
        stopwatch.Stop();
        Console.WriteLine($"\n\nЗатраченное время: {stopwatch.ElapsedMilliseconds/1000} с");
        
        
        rsaK.PrintKeys();
        File.WriteAllLines("private.txt", rsaK.ExportPrivateKey());
        File.WriteAllLines("public.txt", rsaK.ExportPublicKey());
        
        
        Console.Write("\n\n=========================\n\nВведите текст для шифрования: ");
        string? text = Console.ReadLine();
        Console.WriteLine($"c = {rsaK.Encoder(text)}");
        Console.WriteLine($"\nРасшифрованный текст: {rsaK.Decoder()}");

        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("\n\nНажмите любую клавишу, чтобы вернуться в главное меню");
        Console.ResetColor();
        Console.ReadKey();
    }

    public static void CreateKeysEncode()
    {
        Console.CursorVisible = false;
        bool Menu = true;
        int input = 2048;
        int selectedItemIndex = 0;
        string[] menuItems = { "512 бит", "1024 бит", "2048 бит", "4096 бит" };

        while (Menu)
        {
            Console.Clear();
            Console.WriteLine("\nРазмер ключа: ");
            
            for (int i = 0; i < menuItems.Length; i++)
            {
                Console.Write("\t");
                if (i == selectedItemIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(menuItems[i]);

                Console.ResetColor();
            }
            
            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = (selectedItemIndex == 0) ? menuItems.Length - 1 : selectedItemIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex = (selectedItemIndex == menuItems.Length - 1) ? 0 : selectedItemIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    if (selectedItemIndex == 0)
                    {
                        input = 512;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 1)
                    {
                        input = 1024;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 2)
                    {
                        input = 2048;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 3)
                    {
                        input = 4096;
                        Menu = false;
                    }
                    else
                    {
                        input = 2048;
                        Menu = false;
                    }
                    break;
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Подождите, ключ генерируется!");
        Console.ResetColor();
        Console.CursorVisible = true;
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        RSA rsaKe = new RSA();
        // rsaKe.GenerateKeys();
        stopwatch.Stop();
        Console.Clear();
        Console.WriteLine($"\n\nЗатрачено время для генерации: {stopwatch.ElapsedMilliseconds/1000} с");
        
        File.WriteAllLines("private.txt", rsaKe.ExportPrivateKey());
        File.WriteAllLines("public.txt", rsaKe.ExportPublicKey());
        
        Console.WriteLine("\n\nКлючи сохранены в корневую папку с программой");
        
        Console.Write("\n\n=========================\n\nВведите текст для шифрования: ");
        string? text = Console.ReadLine();
        rsaKe.Encoder(text);

        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("\n\nНажмите любую клавишу, чтобы вернуться в главное меню");
        Console.ResetColor();
        Console.ReadKey();
    }

    public static void ImpPublicKey()
    {
        Console.Clear();
        Console.CursorVisible = true;
        RSA rsaKeys = new RSA();
        Console.WriteLine("Введите путь до файла где содержится PUBLIC-ключ(без кавычек)\n");
        string path = "public.txt";
        path = Console.ReadLine();
        rsaKeys.ImportPublic(path);
        Console.Clear();
        
        Console.WriteLine("\nВведите текст, который надо зашифровать(m):");
        string text;
        text = Console.ReadLine();

        rsaKeys.Encoder(text);
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("\n\nНажмите любую клавишу, чтобы вернуться в главное меню");
        Console.ResetColor();
        Console.ReadKey();
    }

    public static void ImpPrivateKey()
    {
        Console.Clear();
        Console.CursorVisible = true;
        RSA rsaKeyss = new RSA();
        Console.WriteLine("Введите путь до файла где содержится PRIVATE-ключ(без кавычек)\n");
        string path = "private.txt";
        path = Console.ReadLine();
        rsaKeyss.ImportPrivate(path);
        Console.Clear();
        
        Console.WriteLine("\nВведите зашифрованный текст (с):");
        string text = "0";
        text = Console.ReadLine();
        BigInteger.TryParse(text, out BigInteger result);

        Console.WriteLine(rsaKeyss.Decoder(result));
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("\n\nНажмите любую клавишу, чтобы вернуться в главное меню");
        Console.ResetColor();
        Console.ReadKey();
    }

    public static void Utf8EncodingArray()
    {
        Console.CursorVisible = false;
        bool Menu = true;
        int input = 2048;
        int selectedItemIndex = 0;
        string[] menuItems = { "32 битa","64 бит","128 бит","256 бит", "512 бит", "1024 бит", "2048 бит", "4096 бит" };

        while (Menu)
        {
            Console.Clear();
            Console.WriteLine("\nРазмер ключа: ");
            
            for (int i = 0; i < menuItems.Length; i++)
            {
                Console.Write("\t");
                if (i == selectedItemIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(menuItems[i]);

                Console.ResetColor();
            }
            
            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = (selectedItemIndex == 0) ? menuItems.Length - 1 : selectedItemIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex = (selectedItemIndex == menuItems.Length - 1) ? 0 : selectedItemIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    if (selectedItemIndex == 0)
                    {
                        input = 32;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 1)
                    {
                        input = 64;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 2)
                    {
                        input = 128;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 3)
                    {
                        input = 256;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 4)
                    {
                        input = 512;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 5)
                    {
                        input = 1024;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 6)
                    {
                        input = 2048;
                        Menu = false;
                    }
                    else if (selectedItemIndex == 7)
                    {
                        input = 4096;
                        Menu = false;
                    }
                    else
                    {
                        input = 2048;
                        Menu = false;
                    }
                    break;
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Подождите, ключ генерируется!");
        Console.ResetColor();
        
        Console.CursorVisible = true;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        RSA testRSA = new RSA(input);
        stopwatch.Stop();
        Console.WriteLine($"\nЗатрачено времени: {stopwatch.ElapsedMilliseconds/1000} с");
        
        Console.Write("Введите название файлов-ключей: ");
        string nameF = Console.ReadLine();
        File.WriteAllLines($"{nameF}-private.pem", testRSA.ExportPrivateKey());
        File.WriteAllLines($"{nameF}-public.pem", testRSA.ExportPublicKey());
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nКлючи сохранены в корневой директории!");
        Console.ResetColor();

        Console.Write("\n=========================\n\nВведите текст, который надо зашифровать: ");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        string? text = Console.ReadLine();
        Console.ResetColor();
        
        string mBase64 = ConvertArr.EncodeBigIntegerArray(testRSA.EncryptArr(text));
        File.WriteAllText("output.txt", mBase64);
        Console.WriteLine("\nШифр текст сохранен в файл 'output.txt'!");
        
        Console.WriteLine($"\nШифр текст:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{mBase64}");
        Console.ResetColor();

        string encryptedText = testRSA.DecryptArr(ConvertArr.DecodeBigIntegerArray(mBase64));

        Console.Write($"\nРасшифрованный текст(для проерки): ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{encryptedText}");
        Console.ResetColor();

        Console.Write("\n\n");
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню");
        Console.ResetColor();
        Console.ReadKey();
    }
}

class Program
{
    static void Main()
    {
        Interface.MainMenu();
    }
}