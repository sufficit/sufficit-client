using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
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
    }
}
