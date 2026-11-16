using Sufficit.Client;
using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.IntegrationTests.Contacts
{
    /// <summary>
    /// Contacts created by integration tests against a real API. Every new contact carries the attribute
    /// <c>test</c> (description <c>integration</c>, value = run id) and is deleted when the test class finishes,
    /// even when a test fails. Updates of existing contacts pass through unchanged.
    /// </summary>
    public sealed class IntegrationTestContacts
    {
        public const string Key = "test";
        public const string Description = "integration";
        public static readonly string RunId = Guid.NewGuid().ToString("N");

        private readonly List<(APIClientService Client, Guid Id)> _created = new();

        public async Task<Guid?> Save(APIClientService client, ContactWithAttributes contact, CancellationToken cancellationToken = default)
        {
            var creating = contact.Id == Guid.Empty;
            if (creating)
                contact.Attributes.Add(new Sufficit.Contacts.Attribute { Key = Key, Value = RunId, Description = Description });
            var id = await client.Contacts.Update(contact, cancellationToken);
            if (creating && id is Guid created && created != Guid.Empty)
                _created.Add((client, created));
            return id;
        }

        /// <summary>Deletes every contact created through <see cref="Save"/>, newest first; failures are reported, not thrown.</summary>
        public async Task<IReadOnlyList<string>> CleanupAsync()
        {
            var failures = new List<string>();
            for (var i = _created.Count - 1; i >= 0; i--)
            {
                var (client, id) = _created[i];
                try { if (!await client.Contacts.Delete(id)) failures.Add($"{id}: not deleted"); }
                catch (Exception ex) { failures.Add($"{id}: {ex.Message}"); }
            }
            _created.Clear();
            return failures;
        }
    }
}
