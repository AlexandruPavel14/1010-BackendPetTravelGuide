using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace PetTravelGuide.Backend.Utils;

/// <summary>
/// JWT Token Builder class. Use this to generate JWT tokens with a fluent API.
/// </summary>
public sealed class JwtTokenBuilder
{
    /// <summary>
    /// The security key for the token
    /// </summary>
    private SecurityKey? _securityKey;

    /// <summary>
    /// The subject of the token
    /// </summary>
    private string _subject = string.Empty;

    /// <summary>
    /// The issuer of the token
    /// </summary>
    private string _issuer = string.Empty;

    /// <summary>
    /// The audience of the token
    /// </summary>
    private string _audience = string.Empty;

    /// <summary>
    /// The claims of the token
    /// </summary>
    private readonly Dictionary<string, string> _claims = new();

    /// <summary>
    /// The token payloads
    /// </summary>
    private readonly Dictionary<string, object> _payloads = new();

    /// <summary>
    /// The token roles
    /// </summary>
    private readonly List<string> _roles = new();

    /// <summary>
    /// Default token expiry in minutes
    /// </summary>
    private int _expiryInMinutes = 30;

    /// <summary>
    /// The role the user has
    /// </summary>
    public static string Role => "role";

    /// <summary>
    /// Add the security key to the JWT token
    /// </summary>
    /// <param name="securityKey">The <see cref="SecurityKey">SecurityKey</see></param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddSecurityKey(SecurityKey securityKey)
    {
        _securityKey = securityKey;
        
        return this;
    }

    /// <summary>
    /// Add the JWT token subject
    /// </summary>
    /// <param name="subject">The subject to be added</param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddSubject(string subject)
    {
        _subject = subject;
        
        return this;
    }

    /// <summary>
    /// Add the JWT token issuer
    /// </summary>
    /// <param name="issuer">The issuer to be added</param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddIssuer(string issuer)
    {
        _issuer = issuer;
        
        return this;
    }

    /// <summary>
    /// Add the JWT token audience
    /// </summary>
    /// <param name="audience">The audience to be added</param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddAudience(string audience)
    {
        _audience = audience;
        
        return this;
    }

    /// <summary>
    /// Add the JWT token claim and value for that claim
    /// </summary>
    /// <param name="type">The claim type</param>
    /// <param name="value">The claim value</param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddClaim(string type, string value)
    {
        _claims.Add(type, value);
        
        return this;
    }

    /// <summary>
    /// Add JWT token role
    /// </summary>
    /// <param name="value">The role to be added</param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddRole(string value)
    {
        _roles.Add(value);
        
        return this;
    }

    /// <summary>
    /// Add JWT token roles
    /// </summary>
    /// <param name="roles">The roles to be added</param>
    /// <returns></returns>
    public JwtTokenBuilder AddRoles(ICollection<string> roles)
    {
        foreach (string role in roles)
        {
            _roles.Add(role);
        }

        return this;
    }

    /// <summary>
    /// Add several claims to the JWT token
    /// </summary>
    /// <param name="claims">The claims to be added as a <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see></param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddClaims(Dictionary<string, string> claims)
    {
        foreach ((string key, string value) in claims)
        {
            _claims[key] = value;
        }

        return this;
    }

    /// <summary>
    /// Set the JWT token expiry value in minutes
    /// </summary>
    /// <param name="expiryInMinutes">The expiry value in minutes</param>
    /// <returns>The JWT token instance</returns>
    public JwtTokenBuilder AddExpiryInMinutes(int expiryInMinutes)
    {
        _expiryInMinutes = expiryInMinutes;
        
        return this;
    }

    /// <summary>
    /// Add a payload to the JWT token
    /// </summary>
    /// <param name="identifier">The unique identifier of the payload</param>
    /// <param name="data">The object value representing the identifier</param>
    public JwtTokenBuilder AddPayload(string identifier, object data)
    {
        _payloads[identifier] = data;
        
        return this;
    }

    /// <summary>
    /// Build the JWT token
    /// </summary>
    /// <returns>The generated token instance</returns>
    public JwtSecurityToken Build()
    {
        EnsureArguments(_securityKey, _issuer, _audience, _roles);
        DateTime date = DateTime.UtcNow;
        List<Claim> claims = new();
        if (!string.IsNullOrWhiteSpace(_subject))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, _subject));
        }
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        foreach ((string key, string value) in _claims)
        {
            claims.Add(new Claim(key, value));
        }

        // Add roles
        claims.AddRange(_roles.Select(role => new Claim(Role, role)));
        JwtSecurityToken token = new(
            _issuer,
            _audience,
            claims,
            notBefore: DateTime.UtcNow,
            expires: date.AddMinutes(_expiryInMinutes),
            signingCredentials: new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
        );
        foreach ((string key, object value) in _payloads)
        {
            token.Payload[key] = value;
        }

        return token;
    }

    /// <summary>
    /// Ensure that all mandatory arguments are set in the JWT token
    /// </summary>
    private static void EnsureArguments(SecurityKey? securityKey, string? issuer, string? audience, List<string>? roles)
    {
        if (securityKey is null)
        {
            throw new ArgumentNullException(nameof(securityKey));
        }

        if (string.IsNullOrEmpty(issuer))
        {
            throw new ArgumentNullException(nameof(issuer));
        }

        if (string.IsNullOrEmpty(audience))
        {
            throw new ArgumentNullException(nameof(audience));
        }

        if (roles?.Count == 0)
        {
            throw new ArgumentNullException(nameof(roles));
        }
    }
}
