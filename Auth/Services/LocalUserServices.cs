using Auth.Data;
using Auth.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Services
{
  public class LocalUserServices : ILocalUserService
  {
    private readonly IdentityContext _context;

    public LocalUserServices(IdentityContext context)
    {
      _context = context;
    }
    public void AddUser(User userToAdd)
    {
      if (userToAdd is null)
        throw new ArgumentNullException(nameof(userToAdd));

      if (_context.Users.Any(u => u.UserName.Equals(userToAdd.UserName)))
        throw new Exception("Username must be unique");

      _context.Users.Add(userToAdd);
    }

    public async Task<User> GetUserBySubjectAsync(string subject)
    {
      if (string.IsNullOrWhiteSpace(subject))
        throw new ArgumentNullException(nameof(subject));

      return await _context.Users.FirstOrDefaultAsync(u => u.Subject.Equals(subject));
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
      if (string.IsNullOrWhiteSpace(userName))
        throw new ArgumentNullException(nameof(userName));

      return await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(userName));
    }

    public async Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(string subject)
    {
      if (string.IsNullOrWhiteSpace(subject))
        throw new ArgumentNullException(nameof(subject));

      return await _context.UserClaims.Where(u => u.User.Subject.Equals(subject)).ToListAsync();
    }

    public async Task<bool> IsUserActive(string subject)
    {
      if (string.IsNullOrWhiteSpace(subject))
        return false;

      var user = await GetUserBySubjectAsync(subject);

      return !(user is null) && user.IsActive;
    }

    public async Task<bool> SaveChangesAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ValidateClearTextCredentialsAsync(string userName, string password)
    {
      if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        return false;

      var user = await GetUserByUserNameAsync(userName);

      if (user is null) return false;

      if (!user.IsActive)
        return false;

      return user.Password.Equals(password);
    }

    //public void AddUser(User userToAdd, string password)
    //{
    //  if (userToAdd is null)
    //    throw new ArgumentNullException(nameof(userToAdd));

    //  if (string.IsNullOrWhiteSpace(password))
    //    throw new ArgumentNullException(nameof(password));

    //  if (_context.Users.Any(u => u.UserName.Equals(userToAdd.UserName)))
    //    throw new Exception("Username must be unique");

    //  if (_context.Users.Any(u => u.Email.Equals(userToAdd.Email)))
    //    throw new Exception("Email must be unique");

    //  //userToAdd.Password = 
    //}

    //public void AddUser(User userToAdd, string password)
    //{
    //    if (userToAdd == null)
    //    {
    //        throw new ArgumentNullException(nameof(userToAdd));
    //    }

    //    if (string.IsNullOrWhiteSpace(password))
    //    {
    //        throw new ArgumentNullException(nameof(password));
    //    }

    //    if (_context.Users.Any(u => u.UserName == userToAdd.UserName))
    //    {
    //        // in a real-life scenario you'll probably want to 
    //        // return this a a validation issue
    //        throw new Exception("Username must be unique");
    //    }

    //    if (_context.Users.Any(u => u.Email == userToAdd.Email))
    //    {
    //        // in a real-life scenario you'll probably want to 
    //        // return this a a validation issue
    //        throw new Exception("Email must be unique");
    //    }

    //    // hash & salt the password
    //    userToAdd.Password = _passwordHasher.HashPassword(userToAdd, password);

    //    using (var randomNumberGenerator = new RNGCryptoServiceProvider())
    //    {
    //        var securityCodeData = new byte[128];
    //        randomNumberGenerator.GetBytes(securityCodeData);
    //        userToAdd.SecurityCode = Convert.ToBase64String(securityCodeData);
    //    }
    //    userToAdd.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);
    //    _context.Users.Add(userToAdd);
    //}

    //public async Task<bool> ActivateUser(string securityCode)
    //{
    //    if (string.IsNullOrWhiteSpace(securityCode))
    //    {
    //        throw new ArgumentNullException(nameof(securityCode));
    //    }

    //    // find an user with this security code as an active security code.  
    //    var user = await _context.Users.FirstOrDefaultAsync(u => 
    //        u.SecurityCode == securityCode && 
    //        u.SecurityCodeExpirationDate >= DateTime.UtcNow);

    //    if (user == null)
    //    {
    //        return false;
    //    }

    //    user.Active = true;
    //    user.SecurityCode = null;
    //    return true;
    //}

    //public async Task<bool> AddUserSecret(string subject, string name, string secret)
    //{
    //    if (string.IsNullOrWhiteSpace(subject))
    //    {
    //        throw new ArgumentNullException(nameof(subject));
    //    }

    //    if (string.IsNullOrWhiteSpace(name))
    //    {
    //        throw new ArgumentNullException(nameof(name));
    //    }

    //    if (string.IsNullOrWhiteSpace(secret))
    //    {
    //        throw new ArgumentNullException(nameof(secret));
    //    }

    //    var user = await GetUserBySubjectAsync(subject); 

    //    if (user == null)
    //    {
    //        return false;
    //    }

    //    user.Secrets.Add(new UserSecret() { Name = name, Secret = secret });             
    //    return true;
    //}

    //public async Task<bool> UserHasRegisteredTotpSecret(string subject)
    //{
    //    if (string.IsNullOrWhiteSpace(subject))
    //    {
    //        throw new ArgumentNullException(nameof(subject));
    //    }

    //    return await _context.UserSecrets.AnyAsync(u => u.User.Subject == subject && u.Name == "TOTP");
    //}

    //public async Task<UserSecret> GetUserSecret(string subject, string name)
    //{
    //    if (string.IsNullOrWhiteSpace(subject))
    //    {
    //        throw new ArgumentNullException(nameof(subject));
    //    }

    //    if (string.IsNullOrWhiteSpace(name))
    //    {
    //        throw new ArgumentNullException(nameof(name));
    //    }

    //    return await _context.UserSecrets
    //        .FirstOrDefaultAsync(u => u.User.Subject == subject && u.Name == name);
    //}

    //public async Task<string> InitiatePasswordResetRequest(string email)
    //{
    //    if (string.IsNullOrWhiteSpace(email))
    //    {
    //        throw new ArgumentNullException(nameof(email));
    //    }

    //    var user = await _context.Users.FirstOrDefaultAsync(u =>
    //      u.Email == email);

    //    if (user == null)
    //    {
    //        throw new Exception($"User with email address {email} can't be found.");
    //    }

    //    using (var randomNumberGenerator = new RNGCryptoServiceProvider())
    //    {
    //        var securityCodeData = new byte[128];
    //        randomNumberGenerator.GetBytes(securityCodeData);
    //        user.SecurityCode = Convert.ToBase64String(securityCodeData);
    //    }

    //    user.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);
    //    return user.SecurityCode;
    //}

    //public async Task<bool> SetPassword(string securityCode, string password)
    //{
    //    if (string.IsNullOrWhiteSpace(securityCode))
    //    {
    //        throw new ArgumentNullException(nameof(securityCode));
    //    }

    //    if (string.IsNullOrWhiteSpace(password))
    //    {
    //        throw new ArgumentNullException(nameof(password));
    //    }

    //    var user = await _context.Users.FirstOrDefaultAsync(u =>
    //    u.SecurityCode == securityCode &&
    //    u.SecurityCodeExpirationDate >= DateTime.UtcNow);

    //    if (user == null)
    //    {
    //        return false;
    //    }

    //    user.SecurityCode = null;
    //    // hash & salt the password
    //    user.Password = _passwordHasher.HashPassword(user, password);
    //    return true;        
    //}

    //public async Task<User> GetUserByExternalProvider(
    //    string provider, 
    //    string providerIdentityKey)
    //{
    //    if (string.IsNullOrWhiteSpace(provider))
    //    {
    //        throw new ArgumentNullException(nameof(provider));
    //    }

    //    if (string.IsNullOrWhiteSpace(providerIdentityKey))
    //    {
    //        throw new ArgumentNullException(nameof(providerIdentityKey));
    //    }

    //    var userLogin = await _context.UserLogins.Include(ul => ul.User)
    //        .FirstOrDefaultAsync(ul => ul.Provider == provider && ul.ProviderIdentityKey == providerIdentityKey);

    //    return userLogin?.User;
    //}

    //public async Task AddExternalProviderToUser(
    //    string subject,
    //    string provider,
    //    string providerIdentityKey)
    //{
    //    if (string.IsNullOrWhiteSpace(subject))
    //    {
    //        throw new ArgumentNullException(nameof(subject));
    //    }

    //    if (string.IsNullOrWhiteSpace(provider))
    //    {
    //        throw new ArgumentNullException(nameof(provider));
    //    }

    //    if (string.IsNullOrWhiteSpace(providerIdentityKey))
    //    {
    //        throw new ArgumentNullException(nameof(providerIdentityKey));
    //    }

    //    var user = await GetUserBySubjectAsync(subject);
    //    user.Logins.Add(new UserLogin()
    //    {
    //        Provider = provider,
    //        ProviderIdentityKey = providerIdentityKey
    //    });            
    //}

    //public User ProvisionUserFromExternalIdentity(
    //    string provider, 
    //    string providerIdentityKey,
    //    IEnumerable<Claim> claims)
    //{
    //    if (string.IsNullOrWhiteSpace(provider))
    //    {
    //        throw new ArgumentNullException(nameof(provider));
    //    }

    //    if (string.IsNullOrWhiteSpace(providerIdentityKey))
    //    {
    //        throw new ArgumentNullException(nameof(providerIdentityKey));
    //    }

    //    var user = new User()
    //    {
    //        Active = true,
    //        Subject = Guid.NewGuid().ToString()
    //    };
    //    foreach (var claim in claims)
    //    {
    //        user.Claims.Add(new UserClaim()
    //        {
    //            Type = claim.Type,
    //            Value = claim.Value
    //        });
    //    }
    //    user.Logins.Add(new UserLogin()
    //    {
    //         Provider = provider,
    //         ProviderIdentityKey = providerIdentityKey
    //    });

    //    _context.Users.Add(user);

    //    return user;
    //}

  }
}
