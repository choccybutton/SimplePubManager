using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data.Repositories;

namespace SimplePubManager.Infrastructure.Services
{
    /// <summary>
    /// Service for user authentication and password management.
    /// </summary>
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordHashService _passwordHashService;
        private readonly JwtTokenService _jwtTokenService;

        /// <summary>
        /// Initializes a new instance of the AuthService class.
        /// </summary>
        /// <param name="userRepository">The user repository</param>
        /// <param name="passwordHashService">The password hash service</param>
        /// <param name="jwtTokenService">The JWT token service</param>
        public AuthService(
            UserRepository userRepository,
            PasswordHashService passwordHashService,
            JwtTokenService jwtTokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }

        /// <summary>
        /// Authenticates a user with email and password.
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <param name="password">The user's password</param>
        /// <param name="organizationId">The organization ID</param>
        /// <returns>A tuple containing the JWT token and user object if successful; otherwise (null, null)</returns>
        public async Task<(string? token, User? user)> LoginAsync(string email, string password, Guid organizationId)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (null, null);
            }

            var user = await _userRepository.GetUserByEmailAsync(organizationId, email);

            if (user == null)
            {
                return (null, null);
            }

            // Verify password
            if (!_passwordHashService.VerifyPassword(password, user.PasswordHash))
            {
                return (null, null);
            }

            // Check if user is active
            if (user.Status != UserStatus.Active)
            {
                return (null, null);
            }

            // Generate token
            var token = _jwtTokenService.GenerateToken(user, organizationId);

            return (token, user);
        }

        /// <summary>
        /// Registers a new user with the provided information.
        /// </summary>
        /// <param name="name">The user's full name</param>
        /// <param name="email">The user's email address</param>
        /// <param name="password">The user's password</param>
        /// <param name="organizationId">The organization ID</param>
        /// <param name="role">The user's role in the organization</param>
        /// <returns>The created user if successful; otherwise null</returns>
        public async Task<User?> RegisterAsync(string name, string email, string password, Guid organizationId, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // Check if email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(organizationId, email);
            if (existingUser != null)
            {
                return null;
            }

            // Hash password
            var passwordHash = _passwordHashService.HashPassword(password);

            // Create user
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(newUser);
            return createdUser;
        }

        /// <summary>
        /// Hashes a password using BCrypt.
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>The bcrypt hashed password</returns>
        public string HashPassword(string password)
        {
            return _passwordHashService.HashPassword(password);
        }

        /// <summary>
        /// Changes a user's password after verifying the old password.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="oldPassword">The user's current password</param>
        /// <param name="newPassword">The new password to set</param>
        /// <returns>True if the password was changed successfully; otherwise false</returns>
        public async Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                return false;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Verify old password
            if (!_passwordHashService.VerifyPassword(oldPassword, user.PasswordHash))
            {
                return false;
            }

            // Hash new password
            var newPasswordHash = _passwordHashService.HashPassword(newPassword);

            // Update user
            user.PasswordHash = newPasswordHash;
            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}
