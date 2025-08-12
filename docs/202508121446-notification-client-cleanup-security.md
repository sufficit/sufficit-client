# Sincronização do NotificationControllerSection

**Data:** 12 de agosto de 2025  
**Projeto:** sufficit-client  
**Componente:** NotificationControllerSection  

## Objetivo

Atualizar o `NotificationControllerSection` para corresponder aos endpoints disponíveis no `NotificationController` da API (sufficit-endpoints).

## Métodos Adicionados

### 1. Notify  
```csharp
public async Task<ActionResult> Notify(object body, CancellationToken cancellationToken = default)
{
    var message = new HttpRequestMessage(HttpMethod.Post, "notification/boardnotify");
    await AddJsonBody(message, body, cancellationToken);
    return await ExecuteRequest(message, cancellationToken);
}
```

### 2. CleanUp (Administrator Only)
```csharp
public async Task<ActionResult> CleanUp(CancellationToken cancellationToken = default)
{
    var message = new HttpRequestMessage(HttpMethod.Get, "notification/cleanup");
    return await ExecuteRequest(message, cancellationToken);
}
```

## Estado Anterior vs Atual

### Antes (Apenas 3 métodos)
- `GetNotifications`
- `GetEvents` 
- `Subscribe`

### Agora (5 métodos completos)
- `GetNotifications` ✅
- `GetEvents` ✅
- `Subscribe` ✅
- **`Notify`** 🆕
- **`CleanUp`** 🆕 (Requer admin)

## Segurança - CleanUp

O método `CleanUp` no cliente **requer autorização de administrador** no backend:

- **API Endpoint:** Protegido com `[Authorize(Roles = AdministratorRole.NormalizedName)]`
- **Cliente:** Executa a requisição normalmente (autorização verificada no backend)
- **Comportamento:** Retorna 403 Forbidden se usuário não for administrador

## Padrões Utilizados

### Base Class
- **Herda de:** `AuthenticatedControllerSection`
- **Namespace:** `Sufficit.Client.Controllers`

### HTTP Patterns
```csharp
// GET requests
var message = new HttpRequestMessage(HttpMethod.Get, "route");

// POST requests with JSON body  
var message = new HttpRequestMessage(HttpMethod.Post, "route");
await AddJsonBody(message, body, cancellationToken);

// Execution
return await ExecuteRequest(message, cancellationToken);
```

## Mapeamento de Rotas

| Método Cliente | HTTP | Rota API | Autorização |
|---|---|---|---|
| `GetNotifications` | GET | `/notification/boardnotifications` | User |
| `GetEvents` | GET | `/notification/events` | User |
| `Subscribe` | POST | `/notification/subscribe` | User |
| `Notify` | POST | `/notification/boardnotify` | User |
| `CleanUp` | GET | `/notification/cleanup` | **Admin** |

## Dependências

- **Sufficit.Identity:** Para constantes de autorização
- **AuthenticatedControllerSection:** Classe base para clientes autenticados
- **HttpRequestMessage:** Para construção de requests HTTP

## Status dos Endpoints

1. **Notify** - ✅ Totalmente funcional
2. **CleanUp** - ✅ Funcional (apenas para administradores)

## Notas de Implementação

- Todos os métodos seguem o padrão async/await
- CancellationToken é suportado em todas as operações
- Tratamento de erros delegado para a classe base
- Serialização JSON automática para objetos de request

## Relacionado

- Documentação da API: `/sufficit-endpoints/docs/202508121445-notification-cleanup-security.md`
- Documentação multi-projeto: `/docs/202508111430-notification-client-api-sync.md`
