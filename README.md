# Translation api

REST service that will serve translated strings. Clients can query with a key (String) and a language id (also String), as well as insert new translations. The client can add new keys and strings for language id's. Keys and their translations should are persisted.

##### Example:
Client adds for language code ‘en’, buy-chips = “Buy chips”
Clients can then query buy-chips for language ‘en’ and retrieve “Buy chips”

## Technologies
- .Net core 3.1
- Enitiy framework core
- Sqlite as a persistent storage (for simplicity)
- Swagger
##### Testing
- xUnit
- AutoFixture
- Nsubstitute
- Fluent Assertions
