using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.EndPoints.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Contacts
{
    /// <summary>
    /// Integration tests for Contact Create/Update operations
    /// Tests the endpoint: POST /Contact
    /// 
    /// Configuration in appsettings.json:
    /// - Sufficit:EndPoints:BaseAddress - API base URL
    /// - Sufficit:Authentication:Tokens:Manager - Bearer token for authentication
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Database", "Transactional")]
    public class ContactUpdateIntegrationTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly Guid _testContactId = Guid.NewGuid();
        
        public ContactUpdateIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var endpointsUrl = configuration["Sufficit:EndPoints:BaseAddress"] 
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured");
            
            var token = configuration["Sufficit:Authentication:Tokens:Manager"] 
                ?? throw new InvalidOperationException("Manager token not configured");

            var timeout = uint.TryParse(configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            var options = new EndPointsAPIOptions
            {
                BaseAddress = endpointsUrl,
                TimeOut = timeout
            };

            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }
        
        [Fact]
        public async Task UpdateContact_CreateNew_ReturnsContactId()
        {
            // Arrange
            var newContact = new ContactWithAttributes
            {
                Id = Guid.Empty, // Empty Guid signals new contact creation
                Attributes = new System.Collections.Generic.HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.Title, Value = "Integration Test Contact" },
                    new() { Key = Attributes.Document, Value = "12345678900" },
                    new() { Key = Attributes.EMail, Value = "test@integration.local" }
                }
            };
            
            _output.WriteLine("Creating new contact via POST /Contact");
            
            // Act
            var result = await _apiClient.Contacts.Update(newContact, CancellationToken.None);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Value);
            
            _output.WriteLine($"Created contact with ID: {result.Value}");
            
            // Verify contact was created by searching for it
            var searchParams = new ContactSearchParameters
            {
                Value = new TextFilterWithKeys
                {
                    Text = "12345678900",
                    Keys = new System.Collections.Generic.HashSet<string> { Attributes.Document }
                }
            };
            
            var found = await _apiClient.Contacts.Search(searchParams, CancellationToken.None);
            var foundContact = found.FirstOrDefault();
            
            Assert.NotNull(foundContact);
            Assert.Equal("Integration Test Contact", foundContact.Title);
            
            _output.WriteLine($"Verified contact exists: {foundContact.Title}");
        }
        
        [Fact]
        public async Task UpdateContact_UpdateExisting_ModifiesAttributes()
        {
            // Arrange - Create contact first
            var newContact = new ContactWithAttributes
            {
                Id = Guid.Empty,
                Attributes = new System.Collections.Generic.HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.Title, Value = "Original Title" },
                    new() { Key = Attributes.Document, Value = "98765432100" }
                }
            };
            
            var contactId = await _apiClient.Contacts.Update(newContact, CancellationToken.None);
            Assert.NotNull(contactId);
            
            _output.WriteLine($"Created contact {contactId} for update test");
            
            // Act - Update with additional attributes
            var updateContact = new ContactWithAttributes
            {
                Id = contactId.Value,
                Attributes = new System.Collections.Generic.HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.EMail, Value = "updated@test.com", Description = "" },
                    new() { Key = Attributes.Phone, Value = "34999887766", Description = Attributes.Cellular }
                }
            };
            
            var updateResult = await _apiClient.Contacts.Update(updateContact, CancellationToken.None);
            
            _output.WriteLine($"Updated contact, result: {updateResult}");
            
            // Assert - Search and verify attributes were added
            var searchParams = new ContactSearchParameters
            {
                Value = new TextFilterWithKeys
                {
                    Text = "98765432100",
                    Keys = new System.Collections.Generic.HashSet<string> { Attributes.Document }
                }
            };
            
            var found = await _apiClient.Contacts.Search(searchParams, CancellationToken.None);
            var foundContact = found.FirstOrDefault();
            
            Assert.NotNull(foundContact);
            Assert.Contains(foundContact.Attributes, a => a.Key == Attributes.EMail && a.Value == "updated@test.com");
            Assert.Contains(foundContact.Attributes, a => a.Key == Attributes.Phone && a.Value == "34999887766");
            
            _output.WriteLine($"Verified contact has {foundContact.Attributes.Count} attributes after update");
        }
        
        [Fact]
        public async Task UpdateContact_WithOwnerIdAttribute_AutoAddsOwner()
        {
            // Arrange - New contact without explicit OwnerId
            var newContact = new ContactWithAttributes
            {
                Id = Guid.Empty,
                Attributes = new System.Collections.Generic.HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.Title, Value = "Auto Owner Test" },
                    new() { Key = Attributes.Document, Value = "11122233344" }
                }
            };
            
            _output.WriteLine("Creating contact - API should auto-add OwnerId");
            
            // Act
            var contactId = await _apiClient.Contacts.Update(newContact, CancellationToken.None);
            Assert.NotNull(contactId);
            
            // Assert - Verify contact has OwnerId attribute
            var searchParams = new ContactSearchParameters
            {
                Value = new TextFilterWithKeys
                {
                    Text = "11122233344",
                    Keys = new System.Collections.Generic.HashSet<string> { Attributes.Document }
                }
            };
            
            var found = await _apiClient.Contacts.Search(searchParams, CancellationToken.None);
            var foundContact = found.FirstOrDefault();
            
            Assert.NotNull(foundContact);
            
            // Check for OwnerId attribute (idproprietario)
            var ownerAttributes = foundContact.Attributes.Where(a => a.Key == Attributes.OwnerId).ToList();
            Assert.NotEmpty(ownerAttributes);
            
            _output.WriteLine($"Contact has {ownerAttributes.Count} owner attribute(s)");
            foreach (var owner in ownerAttributes)
            {
                _output.WriteLine($"  OwnerId: value='{owner.Value}', description='{owner.Description}'");
            }
        }
        
        [Fact]
        public async Task UpdateContact_EmptyGuid_CreatesWithAttributes()
        {
            // Arrange
            var contact = new ContactWithAttributes
            {
                Id = Guid.Empty,
                Attributes = new System.Collections.Generic.HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.Title, Value = "Test Multiple Attributes" },
                    new() { Key = Attributes.Document, Value = "55566677788" },
                    new() { Key = Attributes.EMail, Value = "multi@test.com" },
                    new() { Key = Attributes.Phone, Value = "3499112233", Description = Attributes.Cellular },
                    new() { Key = Attributes.Phone, Value = "3433445566", Description = Attributes.Business }
                }
            };
            
            _output.WriteLine("Creating contact with multiple attributes");
            
            // Act
            var result = await _apiClient.Contacts.Update(contact, CancellationToken.None);
            
            // Assert
            Assert.NotNull(result);
            
            var searchParams = new ContactSearchParameters
            {
                Value = new TextFilterWithKeys
                {
                    Text = "55566677788",
                    Keys = new System.Collections.Generic.HashSet<string> { Attributes.Document }
                }
            };
            
            var found = await _apiClient.Contacts.Search(searchParams, CancellationToken.None);
            var foundContact = found.FirstOrDefault();
            
            Assert.NotNull(foundContact);
            Assert.Equal("Test Multiple Attributes", foundContact.Title);
            
            // Verify multiple phone attributes
            var phones = foundContact.Attributes.Where(a => a.Key == Attributes.Phone).ToList();
            Assert.True(phones.Count >= 2, $"Expected at least 2 phone attributes, found {phones.Count}");
            
            _output.WriteLine($"Contact has {phones.Count} phone numbers");
        }
        
        public void Dispose()
        {
            // Cleanup: In production, would delete test contacts
            // For integration tests, manual cleanup or test database recommended
            _output.WriteLine($"Test contact ID for cleanup: {_testContactId}");
        }
    }
}
