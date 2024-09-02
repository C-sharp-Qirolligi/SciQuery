using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Domain.UserModels.AppRoles;
using SciQuery.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.Services;

public class AccountService(UserManager<User> userManager,IConfiguration configuration) : IAccountService
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    public async Task<string> RegisterAsync(AddOrUpdateUserModel model)
    {
        var name = await _userManager.FindByNameAsync(model.UserName);

        if (name is not null)
        {
            throw new AuthenticationException($"Already exist user name");
        }

        var user = new User()
        {
            UserName = model.UserName,
            Email = model.Email,
            CreatedDate = DateTime.Now,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);

        if (!roleResult.Succeeded || !result.Succeeded)
        {
            throw new AuthenticationException($"Failed to register {model.UserName}");
        }

        var token = await GenerateToken(user, model.UserName);

        return token ?? "";
    }
    public async Task<string> LoginAsync(LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null)
        {
            throw new AuthenticationException($"{model.UserName} is not found!");
        }

        var result = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!result)
        {
            throw new AuthenticationException($"Incorrect Password!");
        }   
        
        user.LastLogindate = DateTime.Now;
        await _userManager.UpdateAsync(user);
        
        var token = await GenerateToken(user, model.UserName);
        
        return token ?? "";

    }

    private async Task<string?> GenerateToken(User user, string userName)
    {
        var secret = configuration["JwtConfig:SecretKey"];
        var audience = configuration["JwtConfig:ValidAudiences"];
        var issuer = configuration["JwtConfig:ValidIssuer"];

        if (secret is null ||  audience is null || issuer is null)
        {
            throw new ApplicationException("Jwt is not set in the configuration");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var tokenHandler = new JwtSecurityTokenHandler();
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,userName),
            new(ClaimTypes.NameIdentifier,user.Id),
        };
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMonths(10).ToUniversalTime(),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return token;
    }
}
