using Sufficit.Client.Controllers;
using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public static class ContactsControllerSectionExtensions
    {
        /// <summary>
        /// Retrieves a collection of attributes associated with the specified contact.
        /// </summary>
        /// <remarks>This method sends an HTTP GET request to retrieve the attributes of a contact. Ensure
        /// that the provided  <paramref name="contactid"/> is valid and corresponds to an existing contact in the
        /// system.</remarks>
        /// <param name="contactid">The unique identifier of the contact whose attributes are to be retrieved.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This allows the operation to be canceled if needed.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 
        /// <see cref="ContactAttribute"/> objects associated with the specified contact.</returns>
        public static Task<IEnumerable<ContactAttribute>> GetAttributes(this ContactsControllerSection source, Guid contactid, CancellationToken cancellationToken)
        {
            var parameters = new AttributeWithKeysSearchParameters
            {
                ContactId = contactid
            };
            return source.GetAttributes(parameters, cancellationToken);
        }

        /// <summary>
        /// Retrieves a collection of contact markers (marker attribute descriptions) for the specified contact.
        /// </summary>
        /// <remarks>This method filters attributes by the "marcador" key (using Sufficit.Contacts.Attributes.Marker constant)
        /// and optionally by description, returning only the description values of the matching marker attributes.
        /// The description filter is applied at the API level for better performance.</remarks>
        /// <param name="source">The ContactsControllerSection instance.</param>
        /// <param name="contactId">The unique identifier of the contact whose markers are to be retrieved.</param>
        /// <param name="description">Optional description filter. If provided, only markers with matching description will be returned.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 
        /// marker description strings associated with the specified contact.</returns>
        public static async Task<IEnumerable<string>> GetContactMarkers(this ContactsControllerSection source, Guid contactId, CancellationToken cancellationToken = default)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var parameters = new AttributeWithKeysSearchParameters
            {
                ContactId = contactId,
                Keys = new HashSet<string> { Attributes.Marker },
                Value = new TextFilterWithKeys()
                {
                    Text = Attributes.UserMarker,
                    ExactMatch = true // Use exact match for marker keys
                }
            };

            var attributes = await source.GetAttributes(parameters, cancellationToken);
            
            // Since we're filtering by Marker key in the parameters, all results should have the correct key
            // Return only the description values (the actual marker names)
            return attributes.Select(a => a.Description);
        }
    }
}
