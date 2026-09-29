using System;
using System.Security.Cryptography;
// codigo para hash de senhas do InstrumentsStore.Api
namespace InstrumentsStore.Api.Security;

// funciona da seguinte maneira: gera um salt aleatório, aplica PBKDF2 com SHA256 e retorna uma string no formato "iterations.salt.key".
// Para verificar, divide a string, extrai o salt e aplica PBKDF2 novamente para comparar com a chave armazenada.
// O número de iterações é configurável, mas 100.000 é um bom valor para segurança e desempenho.
// explicação leiga: o salt é um valor aleatório que é adicionado à senha antes de gerar o hash, para evitar ataques de rainbow table.
// O PBKDF2 é um algoritmo de derivação de chave que aplica uma função hash várias vezes (iterações) para tornar o processo mais lento e seguro contra ataques de força bruta.
// A string final contém o número de iterações, o salt e a chave derivada, separados por pontos.

public static class PasswordHasher
{
    private const int SaltSize = 16;   // 128 bits
    private const int KeySize = 32;    // 256 bits
    private const int Iterations = 100_000;

    public static string Hash(string password) // Retorna uma string no formato "iterations.salt.key"
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public static bool Verify(string password, string storedHash) // Retorna true se a senha fornecida corresponder ao hash armazenado
    {
        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3) return false;

        if (!int.TryParse(parts[0], out var iterations)) return false;

        byte[] salt, expectedKey; // Variáveis para armazenar o salt e a chave esperada
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            expectedKey = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false; // Se a conversão falhar, retorna false
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedKey.Length); // Gera a chave a partir da senha fornecida e do salt armazenado

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey); // Compara as chaves em tempo constante para evitar ataques de timing
    }
}
