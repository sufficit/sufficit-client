# Copilot Instructions
<!-- Version: 3.2 -->

### Common Guidelines
* code comments should always be in English;
* response to user queries should be in IDE current language;
* avoid to change code that was not related to the query;
* when agent has to change a method and it change the async status, the agent should update the method callers too;
* for extensions methods use always "source" as default parameter name
* use one file for each class
* for #region tags: no blank lines between consecutive regions, but always add one blank line after region opening and one blank line before region closing

### Project Specific Guidelines
* whenever you need to create a new query string (ParseQueryString) from a complex class, use ToQueryStringExtensions
* on ControllerSection(s) for the requests that doesn't require authentication, use the `AnonymousPaths` property to define the paths that can be accessed without authentication;
