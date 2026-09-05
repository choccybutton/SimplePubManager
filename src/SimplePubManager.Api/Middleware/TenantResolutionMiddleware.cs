using Microsoft.Extensions.Configuration;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data.Repositories;

namespace SimplePubManager.Api.Middleware
{
    /// <summary>
    /// Middleware that resolves the tenant (organization) from the request's subdomain
    /// and injects it into the HttpContext for use throughout the request pipeline.
    /// </summary>
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantResolutionMiddleware> _logger;

        public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, OrganizationRepository organizationRepository, IConfiguration configuration)
        {
            try
            {
                var subdomain = ExtractSubdomain(context.Request.Host.Host);
                Organization? organization = null;

                if (!string.IsNullOrWhiteSpace(subdomain))
                {
                    // Production: resolve by subdomain
                    organization = await organizationRepository.FindBySubdomainAsync(subdomain);

                    if (organization != null)
                    {
                        context.Items["OrganizationId"] = organization.Id;
                        context.Items["Organization"] = organization;
                        _logger.LogInformation($"Resolved tenant: {subdomain} -> Organization {organization.Id}");
                    }
                    else
                    {
                        _logger.LogWarning($"Tenant not found for subdomain: {subdomain}");
                    }
                }
                else if (IsLocalhost(context.Request.Host.Host))
                {
                    // Development: use default organization for localhost
                    var defaultOrgId = configuration.GetValue<Guid?>("Tenancy:DefaultOrganizationId");

                    if (defaultOrgId.HasValue && defaultOrgId != Guid.Empty)
                    {
                        organization = await organizationRepository.GetByIdAsync(defaultOrgId.Value);

                        if (organization != null)
                        {
                            context.Items["OrganizationId"] = organization.Id;
                            context.Items["Organization"] = organization;
                            _logger.LogInformation($"Using default organization for localhost: {organization.Id}");
                        }
                        else
                        {
                            _logger.LogWarning($"Default organization not found: {defaultOrgId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant from subdomain");
            }

            await _next(context);
        }

        /// <summary>
        /// Checks if the host is localhost or a local IP address.
        /// </summary>
        private bool IsLocalhost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return false;
            }

            var hostWithoutPort = host.Split(':')[0];
            return hostWithoutPort == "localhost" || hostWithoutPort.StartsWith("127.") || hostWithoutPort == "::1";
        }

        /// <summary>
        /// Extracts the subdomain from a hostname.
        /// Examples:
        /// - "tenant1.myapp.com" -> "tenant1"
        /// - "tenant1.localhost:5000" -> "tenant1"
        /// - "localhost:5000" -> null (local development)
        /// - "192.168.1.1:5000" -> null (IP address)
        /// </summary>
        private string? ExtractSubdomain(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return null;
            }

            // Remove port if present
            var hostWithoutPort = host.Split(':')[0];

            // Split by dots
            var parts = hostWithoutPort.Split('.');

            // If it's localhost, 127.0.0.1, or IP address, return null
            if (hostWithoutPort == "localhost" ||
                hostWithoutPort.StartsWith("127.") ||
                parts.Length < 2)
            {
                return null;
            }

            // Return the first part (subdomain)
            return parts[0];
        }
    }
}
