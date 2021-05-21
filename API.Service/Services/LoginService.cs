using API.Domain.Dtos;
using API.Domain.Entities;
using API.Domain.Interfaces.Services.User;
using API.Domain.Repository;
using API.Domain.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace API.Service.Services
{
    public class LoginService : ILoginService
    {
        private IUserRepository _repository;
        private SigningConfigurations _signingConfigurations;
        private TokenConfigurations _tokenConfigurations;
        public IConfiguration _configuration { get; }

        public LoginService(
            IUserRepository repository,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IConfiguration configuration
        )
        {
            this._repository = repository;
            this._signingConfigurations = signingConfigurations;
            this._tokenConfigurations = tokenConfigurations;
            this._configuration = configuration;
        }


        public async Task<object> FindByLogin(LoginDto user)
        {
            var baseUser = new UserEntity();

            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
            {
                baseUser = await this._repository.FindByLogin(user.Email);
                if (baseUser == null)
                {
                    return new
                    {
                        authenticated = false,
                        message = "Falha ao autenticar"
                    };
                }
                else
                {
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(baseUser.Email),
                        new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email)
                        }
                    );

                    DateTime createDate = DateTime.Now;
                    DateTime expirationDate = createDate + TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

                    var handler = new JwtSecurityTokenHandler();
                    string token = CreateToken(identity, createDate, expirationDate, handler);

                    return SuccessObject(createDate, expirationDate, token, baseUser);
                }
            }

            return new
            {
                authenticated = false,
                message = "Falha ao autenticar"
            };
        }

        private string CreateToken(ClaimsIdentity identity, DateTime createDate, DateTime expirationDate, JwtSecurityTokenHandler handler)
        {
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = this._tokenConfigurations.Issuer,
                Audience = this._tokenConfigurations.Audience,
                SigningCredentials = this._signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = createDate,
                Expires = expirationDate,
            });

            var token = handler.WriteToken(securityToken);

            return token;
        }

        private object SuccessObject(DateTime createDate, DateTime expirationDate, string token, UserEntity user)
        {
            return new
            {
                authenticated = true,
                created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token,
                userName = user.Email,
                name = user.Name,
                message = "Usuário logado com sucesso"
            };
        }
    }

    
}
