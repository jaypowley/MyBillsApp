using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;

namespace MyBills.Core
{
    /// <summary>
    /// Handles password hashing and validation
    /// </summary>
    public class Authentication
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 50000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Compute(string password)
        {
            return Compute(password, Iterations);
        }

        /// <summary>
        /// Hashes the provided string password value over the predefined iterations.
        /// </summary>
        /// <param name="password">The password string</param>
        /// <param name="iterations">The number of iterations</param>
        /// <returns></returns>
        private static string Compute(string password, int iterations)
        {
            using var bytes = new Rfc2898DeriveBytes(password, SaltSize, iterations);
            var hash = bytes.GetBytes(HashSize);

            return CreateHashString(hash, bytes.Salt, iterations);
        }

        /// <summary>
        /// Computes the password hash
        /// </summary>
        /// <param name="password">The password string</param>
        /// <param name="salt">The salt string</param>
        /// <param name="iterations">The number of iterations</param>
        /// <param name="hashSize">The hash size</param>
        /// <returns></returns>
        private static string ComputeHash(string password, string salt, int iterations, int hashSize)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using var bytes = new Rfc2898DeriveBytes(password, saltBytes, iterations);
            var hash = bytes.GetBytes(hashSize);

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Validates the password passed in.
        /// </summary>
        /// <param name="password">The password string</param>
        /// <param name="passwordHashString">The hash value</param>
        /// <returns></returns>
        public static bool Verify(string password, string passwordHashString)
        {
            var parameters = new HashParameters(passwordHashString);
            var hashSize = Convert.FromBase64String(parameters.PasswordHash).Length;
            var newPasswordHash = ComputeHash(password, parameters.Salt, parameters.Iterations, hashSize);

            return parameters.PasswordHash == newPasswordHash;
        }

        /// <summary>
        /// Creates the password hash.
        /// </summary>
        /// <param name="hash">The hash bytes</param>
        /// <param name="salt">The salt string</param>
        /// <param name="iterations">The number of iterations</param>
        /// <returns></returns>
        private static string CreateHashString(byte[] hash, byte[] salt, int iterations)
        {
            var saltString = Convert.ToBase64String(salt);
            var hashStringPart = Convert.ToBase64String(hash);

            return string.Join
                (
                    Constants.Splitter.ToString(),
                    Constants.Algorithm,
                    iterations.ToString(CultureInfo.InvariantCulture),
                    saltString,
                    hashStringPart
                );
        }
    }

    public class HashParameters
    {
        private string Algorithm { get; set; }
        
        public void SetAlgorithm(string value)
        {
            Algorithm = value;
        }

        public string GetAlgorithm()
        {
            return Algorithm;
        }

        public int Iterations { get; private set; }

        public string Salt { get; private set; }

        public string PasswordHash { get; private set; }

        public HashParameters(string passwordHashString)
        {
            var parameters = ParseParameters(passwordHashString);

            ProcessParameters(parameters);
        }

        private static string[] ParseParameters(string passwordHashString)
        {
            var parameters = passwordHashString.Split(Constants.Splitter);

            if (parameters.Length != 4)
            {
                throw new ArgumentException("Invalid password hash string format", nameof(passwordHashString));
            }
            return parameters;
        }

        private void ProcessParameters(IReadOnlyList<string> parameters)
        {
            Algorithm = parameters[0];
            Iterations = int.Parse(parameters[1]);
            Salt = parameters[2];
            PasswordHash = parameters[3];
        }
    }
}
