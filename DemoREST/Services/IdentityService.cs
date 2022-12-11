using DemoREST.Data;
using DemoREST.Domain;
using DemoREST.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DemoREST.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DataContext dataContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dataContext = dataContext;
            _roleManager = roleManager;
        }

        public async Task<AuthenticationResult> LoginAsync(string Email, string Password)
        {
            var user = await _userManager.FindByNameAsync(Email);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exist" }
                };
            }

            var userHasValidPwd = await _userManager.CheckPasswordAsync(user, Password);
            if (!userHasValidPwd)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Username/Password" }
                };
            }

            return await GenerateAuthResultForUser(user);
        }

        public async Task<AuthenticationResult> RegisterAsync(string Email, string Password)
        {
            var existingUser = await _userManager.FindByNameAsync(Email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with email address already exists" }
                };
            }

            var user = new IdentityUser
            {
                Email = Email,
                UserName = Email,
            };

            var createdUser = await _userManager.CreateAsync(user, Password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            var authResultForUser = await GenerateAuthResultForUser(user);

            if (!authResultForUser.Success)
            {
                await _userManager.DeleteAsync(user);
            }

            return authResultForUser;
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string Token, string RefreshToken)
        {
            var validatedToken = GetPrincipalFromExpiredToken(Token);

            if(validatedToken is null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if(expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This token hasn't expired yet" }
                };
            }

            var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == RefreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token does not exists" }
                };
            }

            if (storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has expired. Please login with credentials" }
                };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has been invalidated" }
                };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has been used" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token does not match with the JWT" }
                };
            }

            storedRefreshToken.Used = true;
            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x=>x.Type == "id").Value);
            return await GenerateAuthResultForUser(user);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParamsForExpiredToken = _tokenValidationParameters.Clone();
                validationParamsForExpiredToken.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, validationParamsForExpiredToken, out SecurityToken validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken)) return null;
                return principal;
            }
            catch (Exception)
            {

                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthResultForUser(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                claims.Add(new Claim("role", userRoles.First()));
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };
            
            try
            {
                //purge old refresh tokens, just keep the latest one
                var existingRefreshTokenCount = await _dataContext.RefreshTokens.CountAsync(x => x.UserId == user.Id);            
                if(existingRefreshTokenCount >= 3)
                {
                    var oldTokens = await _dataContext.RefreshTokens.Where(x => x.UserId == user.Id)
                                            .OrderBy(x => x.CreationDate)
                                            .Take(existingRefreshTokenCount - 1).ToListAsync();
                    _dataContext.RefreshTokens.RemoveRange(oldTokens);
                }
                _dataContext.RefreshTokens.Add(refreshToken);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new [] {"Could not create Refresh Token"},
                };
            }

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
            };
        }        
    }
}
