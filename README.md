# Translation api

REST service that will serve translated strings. Clients can query with a key (String) and a language id (also String), as well as insert new translations. The client can add new keys and strings for language id's. Keys and their translations should are persisted.

##### Example:
Client adds for language code ‘en’, buy-chips = “Buy chips”
Clients can then query buy-chips for language ‘en’ and retrieve “Buy chips”

## Technologies
- .Net core 3.1
- Entity framework core
- Sqlite as a persistent storage (for simplicity)
- Swagger
##### Testing
- xUnit
- AutoFixture
- Fluent Assertions

## EndPoints

|Method |url|Description|
| :------------ | :------------ | :------------ |
| GET | /api/{language}/translationitems   | Get all translation items for specific language |
|GET| /api/{language}/translationitems/{key}   | Get translation item by key for specific language|
|POST| /api/{language}/translationitems| Create a translation item for specific language. Request body Example {"key": "string", "value": "string" }|
|DELETE| /api/{language}/translationitems/{key}|Delete a translation item by key for specific language|

PS: language parameter is limited to (en and sv), you can add more supported langauges by adding more languages code/id to SupportedLanguagesEnum in the LanguageRouteConstraint Class
