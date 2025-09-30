using BCrypt.Net;

namespace AuthService.Utilities
{
    /// <summary>
    /// Provides static methods for hashing and verifying passwords using BCrypt.
    /// </summary>
    public static class HashingPassword
    {
        /// <summary>
        /// Hashes a plain-text password using BCrypt.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>The hashed password string.</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies that a plain-text password matches a hashed password.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="hashedPassword">The hashed password from the database.</param>
        /// <returns>True if the password is valid, otherwise false.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}