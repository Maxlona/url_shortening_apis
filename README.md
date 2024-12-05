URL Shortening Service (Similar to bitly.com)

**Assumbtions:**
- The Service uses local storage (Concurrent bag)
- The Service will validate the long url, and if failed to cast into a URI, will throw an exception, return bad request.
- The Service will use a GUID, and uses the first 10 digits, to return up to 1 Billion unique hash.
- If the short URL was found in the local storage, the service will try to generate a new one up to X number of times.
- The Request has (Created Date) to track expiration if expired in days was added.
- The APIs do support these 2 extra functionalities:
- 1) Future url Campaings (created date is a future date) similar to "adding sales campagin during holidays.
  2) Expiration in days, which is similar to an expired sales campaign.

**Tech Stack:**
- The Service uses (.NET Core 8) with DI
- Unit tested uses NSubstitute, 
