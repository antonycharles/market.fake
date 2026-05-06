# User.Api

API principal para cadastro e manutenção de usuários e foto de usuário.

## Endpoints

- `GET /v1/User`
- `GET /v1/User/{id}`
- `POST /v1/User`
- `PUT /v1/User/{id}`
- `DELETE /v1/User/{id}`
- `GET /v1/UserPhoto/{id}`
- `POST /v1/UserPhoto`

Os endpoints seguem a base de `User/UserPhoto` do `Accounts.Api` e usam os mesmos roles `1-user-list`, `1-user-create`, `1-user-update` e `1-user-delete`.
