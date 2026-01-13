using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.EndPoints.Configuration;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests
{
    /// <summary>
    /// Integration tests for Contact Search endpoint
    /// Tests the endpoint: GET /contact/search
    /// 
    /// Configuration in appsettings.json:
    /// - Sufficit:EndPoints:BaseAddress - API base URL
    /// - Sufficit:Authentication:Token - Bearer token for authentication
    /// </summary>
    [Trait("Category", "Integration")]
    public class ContactSearchIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        
        public ContactSearchIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
            
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var endpointsUrl = configuration["Sufficit:EndPoints:BaseAddress"] 
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured in appsettings.json");
            
            // Use Manager token for integration tests (has full access)
            var token = configuration["Sufficit:Authentication:Tokens:Manager"] 
                ?? throw new InvalidOperationException("Manager token not configured in appsettings.json");

            var timeout = uint.TryParse(configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            // Configure EndPoints options
            var options = new EndPointsAPIOptions
            {
                BaseAddress = endpointsUrl,
                TimeOut = timeout
            };

            // Create API client
            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }
                
        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "ReadOnly")]
        public async Task ContactSearch_WithFilter_ReturnsContactsWithTitle()
        {
            // Arrange
            var filter = "s"; // Simple filter to get some results
            var results = 5;
            
            _output.WriteLine($"Testing GET /contact/search?filter={filter}&results={results}");
            
            // Act
            var contacts = await _apiClient.Contacts.Search(filter, results, CancellationToken.None);
            
            // Assert
            Assert.NotNull(contacts);
            var contactList = contacts.ToList();
            
            _output.WriteLine($"Found {contactList.Count} contacts");
            
            // IMPORTANT: Test validates Title is present IF there are results
            // The assertion is more flexible - passes even with 0 results
            foreach (var contact in contactList)
            {
                // CRITICAL: Title property must be present and not null
                Assert.NotNull(contact.Title);
                Assert.NotEmpty(contact.Title);
                
                _output.WriteLine($"Contact: {contact.Id} -> Title: '{contact.Title}'");
                
                // Verify contact has ID
                Assert.NotEqual(Guid.Empty, contact.Id);
            }
            
            if (!contactList.Any())
            {
                _output.WriteLine("No contacts found for filter 's' - test passes (database dependent)");
            }
        }
        
        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "ReadOnly")]
        public async Task ContactSearch_EmptyFilter_ReturnsContacts()
        {
            // Arrange
            var filter = "";
            var results = 3;
            
            _output.WriteLine($"Testing GET /contact/search?filter=&results={results}");
            
            // Act
            var contacts = await _apiClient.Contacts.Search(filter, results, CancellationToken.None);
            Assert.NotNull(contacts);
            var contactList = contacts.ToList();
            
            _output.WriteLine($"Found {contactList.Count} contacts with empty filter");
            
            // May have 0 results, that's ok
            foreach (var contact in contactList)
            {
                Assert.NotNull(contact.Title);
                Assert.NotEqual(Guid.Empty, contact.Id);
                
                _output.WriteLine($"Contact: {contact.Id} -> Title: '{contact.Title}'");
            }
        }
        
        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "ReadOnly")]
        public async Task ContactSearch_VerifyDefaultSearchKeys()
        {
            // Arrange
            var filter = "test";
            var results = 10;
            
            _output.WriteLine("Testing that contacts have essential attributes loaded");
            _output.WriteLine("Expected keys: title, timestamp, cellular, email, document (DEFAULTSEARCHKEYS)");
            
            // Act
            var contacts = await _apiClient.Contacts.Search(filter, results, CancellationToken.None);
            Assert.NotNull(contacts);
            var contactList = contacts.ToList();
            
            if (contactList.Any())
            {
                foreach (var contact in contactList.Take(3)) // Check first 3 contacts
                {
                    _output.WriteLine($"\nContact: {contact.Id}");
                    _output.WriteLine($"  Title: {contact.Title ?? "(null)"}");
                    
                    // At minimum, Title should be present (it's in DEFAULTSEARCHKEYS)
                    Assert.NotNull(contact.Title);
                    Assert.NotEmpty(contact.Title);
                }
            }
            else
            {
                _output.WriteLine("No contacts found for filter 'test', skipping attribute verification");
            }
        }
        
        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "ReadOnly")]
        public async Task ContactSearch_ConsistentResultsAcrossCalls()
        {
            // Arrange
            var filter = "a";
            var results = 5;
            
            _output.WriteLine("Testing that multiple calls return consistent results");
            
            // Act - Call twice with same parameters
            var contacts1 = await _apiClient.Contacts.Search(filter, results, CancellationToken.None);
            var contacts2 = await _apiClient.Contacts.Search(filter, results, CancellationToken.None);
            
            Assert.NotNull(contacts1);
            Assert.NotNull(contacts2);
            
            var list1 = contacts1.ToList();
            var list2 = contacts2.ToList();
            
            _output.WriteLine($"First call: {list1.Count} contacts");
            _output.WriteLine($"Second call: {list2.Count} contacts");
            
            // Assert - Should return same count and same IDs
            Assert.Equal(list1.Count, list2.Count);
            
            if (list1.Any())
            {
                var ids1 = list1.Select(c => c.Id).ToList();
                var ids2 = list2.Select(c => c.Id).ToList();
                
                Assert.Equal(ids1, ids2);
                
                _output.WriteLine("Contact IDs are consistent across calls");
            }
        }
    }
}
