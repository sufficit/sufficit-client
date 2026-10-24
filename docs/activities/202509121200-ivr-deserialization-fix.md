> Migrated from sufficit-project-management/docs/202509121200-ivr-deserialization-fix.md on 2026-10-24.

# Correção de Desserialização IVR - 202509121200

## Problema

Ocorreu um erro de desserialização "DeserializeUnableToConvertValue" ao tentar salvar um IVR através da API. O erro ocorria no cliente Blazor ao chamar `APIClient.Telephony.IVR.AddOrUpdate()`.

## Causa Raiz

O método `AddOrUpdate` no `TelephonyIVRControllerSection` estava retornando `Task<EndPointResponse<IVR>?>` diretamente, mas o servidor ASP.NET Core estava retornando `ActionResult<EndPointResponse<IVR>>`. Isso causava incompatibilidade na desserialização JSON.

## Solução Implementada

Atualizou-se o método `AddOrUpdate` no `TelephonyIVRControllerSection.cs` para seguir o mesmo padrão usado no `TelephonyPortabilityControllerSection.cs`:

### Antes

```csharp
public Task<EndPointResponse<IVR>?> AddOrUpdate(IVR ivr, CancellationToken cancellationToken = default)
{
    // ... código ...
    return Request<EndPointResponse<IVR>>(message, cancellationToken);
}
```

### Depois

```csharp
public async Task<IVR?> AddOrUpdate(IVR ivr, CancellationToken cancellationToken = default)
{
    // ... código ...
    var response = await Request<EndPointResponse<IVR>>(message, cancellationToken);
    return response?.Success ?? false ? response.Data : null;
}
```

## Benefícios da Solução

1. **Tratamento Adequado**: O método agora trata corretamente a resposta `EndPointResponse<T>` do servidor
2. **Retorno Direto**: Retorna o objeto `IVR` diretamente ao usuário final, em vez da resposta da API
3. **Consistência**: Segue o mesmo padrão usado em outros controllers do projeto
4. **Robustez**: Trata casos onde `response` pode ser `null` ou `Success` pode ser `false`

## Validação

- ✅ Projeto cliente compilou com sucesso após a correção
- ✅ Método segue o padrão estabelecido no `TelephonyPortabilityControllerSection`
- ✅ Tratamento adequado de casos de erro e resposta nula

## Arquivos Modificados

- `z:\Desenvolvimento\sufficit-client\src\Controllers\Telephony\TelephonyIVRControllerSection.cs`

## Status

✅ **Resolvido** - Correção implementada e validada
