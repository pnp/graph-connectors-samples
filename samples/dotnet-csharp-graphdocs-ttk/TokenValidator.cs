using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;

namespace GraphDocsConnector
{
    internal class TokenValidator
    {
        private string _audience;
        private string _tenant;

        public TokenValidator(string tenant, string audience)
        {
            _audience = audience;
            _tenant = tenant;
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerValidator = (issuer, token, parameters) =>
                    {
                        var validIssuer = $"https://sts.windows.net/{_tenant}/";

                        if (token is JwtSecurityToken jwt)
                        {
                            var version = jwt.Claims.FirstOrDefault(c => c.Type == "ver")?.Value;
                            switch (version)
                            {
                                case "1.0":
                                    validIssuer = $"https://sts.windows.net/{_tenant}/";
                                    break;
                                case "2.0":
                                    validIssuer = $"https://login.microsoftonline.com/{_tenant}/v2.0";
                                    break;
                                default:
                                    throw new SecurityTokenInvalidIssuerException($"Invalid token version: {version}");
                            }

                            var documentRetriever = new HttpDocumentRetriever(Utils.GetHttpClient())
                            {
                                RequireHttps = true
                            };

                            var _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                                $"{validIssuer}{(validIssuer.EndsWith('/') ? "" : "/")}.well-known/openid-configuration",
                                new OpenIdConnectConfigurationRetriever(),
                                documentRetriever
                            );
                            var discoveryDocument = _configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
                            parameters.IssuerSigningKeys = discoveryDocument.SigningKeys;
                        }

                        if (issuer != validIssuer)
                        {
                            // from https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/d353b5a67200933361941550e99d4a15a11de04e/src/Microsoft.IdentityModel.Tokens/Validators.cs#L316
                            throw new SecurityTokenInvalidIssuerException(string.Format(
                                // from https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/d353b5a67200933361941550e99d4a15a11de04e/src/Microsoft.IdentityModel.Tokens/LogMessages.cs#L33
                                "IDX10205: Issuer validation failed. Issuer: '{0}'. Did not match: validIssuer: '{1}'. For more details, see https://aka.ms/IdentityModel/issuer-validation.",
                                LogHelper.MarkAsNonPII(issuer),
                                LogHelper.MarkAsNonPII(validIssuer)))
                            { InvalidIssuer = issuer };
                        }

                        return issuer;
                    },
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ValidateSignatureLast = true,
                    ClockSkew = TimeSpan.FromSeconds(30) // Allow a brief skew in time
                };

                try
                {
                    var principal = new JwtSecurityTokenHandler()
                        .ValidateToken(token, validationParameters, out var validatedToken);

                    return validatedToken != null;
                }
                catch (SecurityTokenValidationException)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}