using System;
using System.Collections;
using System.Security.Cryptography;


namespace Game_Rick_and_Morty
{

    public class Sha3BasedRandomGenerator
    {
        public Sha3BasedRandomGenerator() { }
        public byte[] GenerateSecretKey()
        {
            byte[] key = new byte[256 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return key;
        }
        public string ComputeHmac(byte[] key, int valueToHash)
        {
            byte[] valueBytes = BitConverter.GetBytes(valueToHash);
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(valueBytes);
            return Convert.ToHexString(hash).ToLower();
        }

        public int FinalValue { get; } = 0;
        public byte[] MortySecretValueHmac { get; } = Array.Empty<byte>();
        public byte[] SecretKeyUsed { get; } = Array.Empty<byte>();
        public int MortySecretValue { get; } = 0;
        public int RickInputValue { get; } = 0;


        internal Sha3BasedRandomGenerator(int finalValue, byte[] mortySecretValueHmac, byte[] secretKeyUsed, int mortySecretValue, int rickInputValue)
        {
            FinalValue = finalValue;
            MortySecretValueHmac = mortySecretValueHmac;
            SecretKeyUsed = secretKeyUsed;
            MortySecretValue = mortySecretValue;
            RickInputValue = rickInputValue;
        }

        public Sha3BasedRandomGenerator GenerateFairNumber(int N, int rickInputValue)
        {
            byte[] secretKey = GenerateSecretKey();

            int mortySecretValue = GenerateUniformRandomInRange(N);

            byte[] mortySecretValueBytes = BitConverter.GetBytes(mortySecretValue);
            byte[] hashOfMortySecretValue;
            using (var hmac = new HMACSHA256(secretKey))
            {
                hashOfMortySecretValue = hmac.ComputeHash(mortySecretValueBytes);
            }

            Console.WriteLine($"\nMorty has generated a secret value and its HMAC.");
            Console.WriteLine($"HMAC (SHA-256) based on Morty's secret value: {BitConverter.ToString(hashOfMortySecretValue).Replace("-", "")}");
            Console.WriteLine($"This HMAC guarantees Morty won't change his mind later.");

            int finalValue = (mortySecretValue + rickInputValue) % N;

            Console.WriteLine($"Protocol finished. Final value combined by Morty and Rick: {finalValue}");

            return new Sha3BasedRandomGenerator(finalValue, hashOfMortySecretValue, secretKey, mortySecretValue, rickInputValue);
        }

        public static void RevealProtocolDetails(Sha3BasedRandomGenerator result, int N)
        {
            Console.WriteLine("\n--- PROTOCOL REVEALING ---");
            Console.WriteLine($"Morty's secret value: {result.MortySecretValue}");
            Console.WriteLine($"Rick's input value: {result.RickInputValue}");
            Console.WriteLine($"Secret Key used: {BitConverter.ToString(result.SecretKeyUsed).Replace("-", "")}");
            Console.WriteLine($"Final calculated value: ({result.MortySecretValue} + {result.RickInputValue}) % {N} = {result.FinalValue}");

            byte[] mortySecretValueBytes = BitConverter.GetBytes(result.MortySecretValue);
            byte[] recomputedHmac;
            using (var hmac = new HMACSHA256(result.SecretKeyUsed))
            {
                recomputedHmac = hmac.ComputeHash(mortySecretValueBytes);
            }

            Console.WriteLine($"Recomputed HMAC for Morty's secret value: {BitConverter.ToString(recomputedHmac).Replace("-", "")}");
            Console.WriteLine($"Original HMAC provided by Morty:       {BitConverter.ToString(result.MortySecretValueHmac).Replace("-", "")}");

            if (StructuralComparisons.StructuralEqualityComparer.Equals(recomputedHmac, result.MortySecretValueHmac))
            {
                Console.WriteLine("HMAC verification successful! Morty's secret value was consistent with the promised HMAC.");
            }
            else
            {
                Console.WriteLine("HMAC verification FAILED! This indicates a potential attempt by Morty to cheat!");
            }
            Console.WriteLine("--- END PROTOCOL REVEALING ---\n");
        }

        public Sha3BasedRandomGenerator GenerateFairNumberInteractive(int N, Func<int> getRickInput)
        {
            byte[] secretKey = GenerateSecretKey();
            int mortySecretValue = GenerateUniformRandomInRange(N);

            byte[] mortySecretValueBytes = BitConverter.GetBytes(mortySecretValue);
            byte[] hashOfMortySecretValue;
            using (var hmac = new HMACSHA256(secretKey))
            {
                hashOfMortySecretValue = hmac.ComputeHash(mortySecretValueBytes);
            }

            Console.WriteLine($"\nMorty has generated a secret value and its HMAC.");
            Console.WriteLine($"HMAC (SHA-256) based on Morty's secret value: {BitConverter.ToString(hashOfMortySecretValue).Replace("-", "")}");
            Console.WriteLine($"This HMAC guarantees Morty won't change his mind later.");

            int rickInputValue = getRickInput();
            int finalValue = (mortySecretValue + rickInputValue) % N;

            return new Sha3BasedRandomGenerator(finalValue, hashOfMortySecretValue, secretKey, mortySecretValue, rickInputValue);
        }

        private static int GenerateUniformRandomInRange(int upperExclusive)
        {
            if (upperExclusive <= 0) throw new ArgumentOutOfRangeException(nameof(upperExclusive));

            var buffer = new byte[4];
            uint bound = (uint)upperExclusive;
            uint max = uint.MaxValue - (uint.MaxValue % bound);
            using var rng = RandomNumberGenerator.Create();
            while (true)
            {
                rng.GetBytes(buffer);
                uint value = BitConverter.ToUInt32(buffer, 0);
                if (value < max)
                {
                    return (int)(value % bound);
                }
            }
        }
    }
}





