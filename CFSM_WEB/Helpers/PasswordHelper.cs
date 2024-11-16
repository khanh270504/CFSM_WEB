using System.Security.Cryptography;

public static class PasswordHelper
{
	public static string HashPassword(string password, out byte[] salt, int iterations = 10000)
	{
		salt = new byte[16];
		using (var rng = new RNGCryptoServiceProvider())
		{
			rng.GetBytes(salt);
		}

		using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
		{
			byte[] hash = pbkdf2.GetBytes(32);
			return Convert.ToBase64String(hash);
		}
	}

	public static bool VerifyPassword(string password, string hashedPassword, byte[] salt, int iterations = 10000)
	{
		using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
		{
			byte[] hash = pbkdf2.GetBytes(32);
			return Convert.ToBase64String(hash) == hashedPassword;
		}
	}
}
