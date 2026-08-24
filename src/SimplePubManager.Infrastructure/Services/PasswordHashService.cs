namespace SimplePubManager.Infrastructure.Services
{
    /// <summary>
    /// Service for hashing and verifying passwords using BCrypt.
    /// </summary>
    public class PasswordHashService
    {
        private const int BcryptWorkFactor = 10;

        /// <summary>
        /// Hashes a password using BCrypt with a work factor of 10.
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>The bcrypt hashed password</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password, BcryptWorkFactor);
        }

        /// <summary>
        /// Verifies a plain text password against a bcrypt hash.
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hash">The bcrypt hash to verify against</param>
        /// <returns>True if the password matches the hash; otherwise false</returns>
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (InvalidOperationException)
            {
                // Invalid hash format
                return false;
            }
        }
    }
}
