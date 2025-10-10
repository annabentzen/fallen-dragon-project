krav til oblig 2:

- .NET Core 8.0
- MVC
- basic design, dynamic content, friendly navigation

code:

- neat code, structured
- with comments if not self explainatory

funksjonalitet:

- minst to tabeller i databasen som man kan gjøre CRUD på, dvs:
  create, read, update, delete

repository pattern and DAL:

- et ekstra lag mellom controller og databasen

eksempel:

---

CharacterRepository.cs:
public async Task<List<Character>> GetAllAsync()
public async Task<Character> GetByIdAsync(int id)
public async Task AddAsync(Character character)

---

asynchronous database access:

- når man henter eller lagrer data skal async/await brukes
- bruke async/await i repository eller controller

error handling and logging (server-side)

- bruk try/catch i controller eller repository
