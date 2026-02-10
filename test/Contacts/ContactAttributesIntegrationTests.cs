using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.EndPoints.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Contacts
{
    /// <summary>
    /// Integration tests for Contact Attributes management
    /// Tests endpoints:
    /// - POST /contact/attributes (get attributes with filters)
    /// - GET /contact/attribute (get first attribute)
    /// - POST /contact/attribute (create/update attribute)
    /// - DELETE /contact/attribute (remove attribute)
    /// - GET /contact/attribute/value (get attribute value)
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Database", "Transactional")]
    public class ContactAttributesIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly IConfiguration _configuration;
        
        public ContactAttributesIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
            
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var endpointsUrl = _configuration["Sufficit:EndPoints:BaseAddress"] 
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured");
            
            var token = _configuration["Sufficit:Authentication:Tokens:Manager"] 
                ?? throw new InvalidOperationException("Manager token not configured");

            var timeout = uint.TryParse(_configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

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
        public async Task GetAttributes_WithKeys_ReturnsFilteredAttributes()
        {
            // Arrange
            var parameters = new AttributeWithKeysSearchParameters
            {
                Keys = new HashSet<string> { Attributes.Title, Attributes.Document },
                Limit = 10
            };
            
            _output.WriteLine("Testing POST /contact/attributes with key filters");
            
            // Act
            var attributes = await _apiClient.Contacts.GetAttributes(parameters, CancellationToken.None);
            var attributeList = attributes.ToList();
            
            // Assert
            Assert.NotNull(attributeList);
            
            _output.WriteLine($"Found {attributeList.Count} attributes");
            
            // All returned attributes should have keys in the filter
            foreach (var attr in attributeList)
            {
                Assert.Contains(attr.Key, new[] { Attributes.Title, Attributes.Document });
                _output.WriteLine($"Attribute: key='{attr.Key}', value='{attr.Value}', contact={attr.ContactId}");
            }
        }
        
        [Fact]
        public async Task GetAttributes_ForSpecificContact_ReturnsContactAttributes()
        {
            // Arrange - Use extension method
            var testContactId = Guid.Parse(_configuration["Sufficit:TestData:Contacts:KnownContactId"] 
                ?? "3e350477-06ae-4e66-b83a-63103fdbb46e"); // TECHWEL from investigation
            
            _output.WriteLine($"Getting attributes for contact {testContactId}");
            
            // Act
            var attributes = await _apiClient.Contacts.GetAttributes(testContactId, CancellationToken.None);
            var attributeList = attributes.ToList();
            
            // Assert
            Assert.NotNull(attributeList);
            Assert.NotEmpty(attributeList); // Known contact should have attributes
            
            _output.WriteLine($"Contact has {attributeList.Count} attributes:");
            
            // Group by key for better output
            var grouped = attributeList.GroupBy(a => a.Key);
            foreach (var group in grouped)
            {
                _output.WriteLine($"  {group.Key}: {group.Count()} entries");
            }
            
            // Verify all attributes belong to the same contact
            Assert.All(attributeList, attr => Assert.Equal(testContactId, attr.ContactId));
        }
        
        [Fact]
        public async Task GetFirstAttribute_WithFilters_ReturnsSingleAttribute()
        {
            // Arrange
            var parameters = new AttributeWithKeysSearchParameters
            {
                Keys = new HashSet<string> { Attributes.Title },
                Value = new TextFilterWithKeys
                {
                    Text = "sufficit",
                    ExactMatch = false
                }
            };
            
            _output.WriteLine("Testing GET /contact/attribute (first match)");
            
            // Act
            var attribute = await _apiClient.Contacts.GetFirstAttribute(parameters, CancellationToken.None);
            
            // Assert - May or may not find a match
            if (attribute != null)
            {
                _output.WriteLine($"Found attribute: key='{attribute.Key}', value='{attribute.Value}'");
                Assert.Equal(Attributes.Title, attribute.Key);
                Assert.Contains("sufficit", attribute.Value, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                _output.WriteLine("No attribute found matching filter (database dependent)");
            }
        }
        
        [Fact]
        public async Task CreateOrUpdateAttribute_ForContact_AddsAttribute()
        {
            // Arrange - Create test contact first
            var testContact = new ContactWithAttributes
            {
                Id = Guid.Empty,
                Attributes = new HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.Title, Value = "Attribute Test Contact" },
                    new() { Key = Attributes.Document, Value = "99988877766" }
                }
            };
            
            var contactId = await _apiClient.Contacts.Update(testContact, CancellationToken.None);
            Assert.NotNull(contactId);
            
            _output.WriteLine($"Created test contact {contactId} for attribute operations");
            
            // Act - Add new attribute
            var newAttribute = new Sufficit.Contacts.Attribute
            {
                Key = Attributes.EMail,
                Value = "attributetest@example.com",
                Description = ""
            };
            
            await _apiClient.Contacts.CreateOrUpdateAttribute(contactId.Value, newAttribute, CancellationToken.None);
            
            _output.WriteLine("Added email attribute via POST /contact/attribute");
            
            // Assert - Verify attribute was added
            var attributes = await _apiClient.Contacts.GetAttributes(contactId.Value, CancellationToken.None);
            var emailAttrs = attributes.Where(a => a.Key == Attributes.EMail).ToList();
            
            Assert.NotEmpty(emailAttrs);
            Assert.Contains(emailAttrs, a => a.Value == "attributetest@example.com");
            
            _output.WriteLine($"Verified email attribute exists in contact");
        }
        
        [Fact]
        public async Task GetAttributeValue_ForKnownAttribute_ReturnsValue()
        {
            // Arrange
            var testContactId = Guid.Parse(_configuration["Sufficit:TestData:Contacts:KnownContactId"] 
                ?? "3e350477-06ae-4e66-b83a-63103fdbb46e");
            
            _output.WriteLine($"Testing GET /contact/attribute/value for contact {testContactId}");
            
            // Act
            var value = await _apiClient.Contacts.GetAttributeValue(
                testContactId, 
                Attributes.Document, 
                "", 
                CancellationToken.None);
            
            // Assert - Should have cadastro (Document) attribute
            if (value != null)
            {
                _output.WriteLine($"Found document value: '{value}'");
                Assert.NotEmpty(value);
            }
            else
            {
                _output.WriteLine("Attribute value not found (may not have matching description)");
            }
        }
        
        [Fact]
        public async Task RemoveAttribute_ExistingAttribute_RemovesSuccessfully()
        {
            // Arrange - Create contact with attribute
            var testContact = new ContactWithAttributes
            {
                Id = Guid.Empty,
                Attributes = new HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.Title, Value = "Remove Test" },
                    new() { Key = Attributes.EMail, Value = "remove@test.com", Description = "test-email" }
                }
            };
            
            var contactId = await _apiClient.Contacts.Update(testContact, CancellationToken.None);
            Assert.NotNull(contactId);
            
            _output.WriteLine($"Created contact {contactId} with test email attribute");
            
            // Act - Remove the attribute
            await _apiClient.Contacts.RemoveAttribute(
                contactId.Value, 
                Attributes.EMail, 
                "test-email", 
                CancellationToken.None);
            
            _output.WriteLine("Removed email attribute via DELETE /contact/attribute");
            
            // Assert - Verify attribute was removed
            var attributes = await _apiClient.Contacts.GetAttributes(contactId.Value, CancellationToken.None);
            var emailAttrs = attributes.Where(a => a.Key == Attributes.EMail && a.Description == "test-email").ToList();
            
            Assert.Empty(emailAttrs);
            
            _output.WriteLine("Verified attribute was removed");
        }
        
        [Fact]
        public async Task GetContactMarkers_ForContact_ReturnsMarkerDescriptions()
        {
            // Arrange - Create contact with markers
            var testContact = new ContactWithAttributes
            {
                Id = Guid.Empty,
                Attributes = new HashSet<Sufficit.Contacts.Attribute>
                {
                    new() { Key = Attributes.Title, Value = "Marker Test Contact" },
                    new() { Key = Attributes.Marker, Value = Attributes.UserMarker, Description = "vip" },
                    new() { Key = Attributes.Marker, Value = Attributes.UserMarker, Description = "priority" }
                }
            };
            
            var contactId = await _apiClient.Contacts.Update(testContact, CancellationToken.None);
            Assert.NotNull(contactId);
            
            _output.WriteLine($"Created contact {contactId} with markers");
            
            // Act - Use extension method to get markers
            var markers = await _apiClient.Contacts.GetContactMarkers(contactId.Value, CancellationToken.None);
            var markerList = markers.ToList();
            
            // Assert
            Assert.NotEmpty(markerList);
            Assert.Contains("vip", markerList);
            Assert.Contains("priority", markerList);
            
            _output.WriteLine($"Contact has markers: {string.Join(", ", markerList)}");
        }
    }
}
