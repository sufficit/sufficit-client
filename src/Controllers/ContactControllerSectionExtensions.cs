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
        /// Retrieves all attributes associated with the specified contact.
        /// </summary>
        public static Task<IEnumerable<ContactAttribute>> GetAttributes(this ContactsControllerSection source, Guid contactid, CancellationToken cancellationToken)
        {
            var parameters = new AttributeWithKeysSearchParameters
            {
                ContactId = contactid
            };
            return source.Attribute.Search(parameters, cancellationToken);
        }

        /// <summary>
        /// Retrieves the marker descriptions assigned to the specified contact.
        /// Filters by the <see cref="Attributes.Marker"/> key and <see cref="Attributes.UserMarker"/> value.
        /// </summary>
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
                    ExactMatch = true
                }
            };

            var attributes = await source.Attribute.Search(parameters, cancellationToken);
            return attributes.Select(a => a.Description);
        }
    }
}
