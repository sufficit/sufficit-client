# 202601071200 - Refatoração GenericLog Extension

## Resumo
Refatoração do método `GetEventsWithContent<T>` em `LoggingControllerSection.cs` para usar extensão `FromJsonLog<T>` recém-criada em `Sufficit.Base`, evitando duplicação de código de conversão entre `GenericLog<string>` e `GenericLog<T>`.

## Mudanças Implementadas

### 1. Criação de Extensão em Sufficit.Base
- **Arquivo**: `src/Logging/GenericLogExtensions.cs`
- **Extensão**: `FromJsonLog<T>(this GenericLog<string> source, JsonSerializerOptions? options = null)`
- **Propósito**: Converte `GenericLog<string>` para `GenericLog<T>` desserializando o campo `Content` usando `JsonSerializer.Deserialize`

### 2. Refatoração em Sufficit.Client
- **Arquivo**: `src/Controllers/LoggingControllerSection.cs`
- **Mudança**: Substituição do loop manual de conversão por uso de LINQ `Select` com a nova extensão
- **Benefício**: Código mais limpo e reutilizável

## Código Antes
```csharp
public async Task<IEnumerable<GenericLog<T>>> GetEventsWithContent<T>(LogSearchParameters parameters, CancellationToken cancellationToken) where T : class
{
    var stringResults = await GetEventsWithContent(parameters, cancellationToken);
    var convertedResults = new List<GenericLog<T>>();
    foreach (var item in stringResults)
    {
        var converted = new GenericLog<T>
        {
            Timestamp = item.Timestamp,
            ClassName = item.ClassName,
            ContextId = item.ContextId,
            UserId = item.UserId,
            Reference = item.Reference,
            Message = item.Message,
            Content = string.IsNullOrWhiteSpace(item.Content) 
                ? default 
                : JsonSerializer.Deserialize<T>(item.Content, _json)
        };
        convertedResults.Add(converted);
    }
    return convertedResults;
}
```

## Código Depois
```csharp
public async Task<IEnumerable<GenericLog<T>>> GetEventsWithContent<T>(LogSearchParameters parameters, CancellationToken cancellationToken) where T : class
{
    var stringResults = await GetEventsWithContent(parameters, cancellationToken);
    return stringResults.Select(item => item.FromJsonLog<T>(_json));
}
```

## Benefícios
- **Redução de Duplicação**: Eliminação de código repetitivo de conversão
- **Manutenibilidade**: Lógica centralizada em extensão reutilizável
- **Consistência**: Mesmo padrão usado em `Sufficit.EFData` (que mantém suas próprias extensões)
- **Performance**: Uso de LINQ mais eficiente que loop manual

## Compatibilidade
- Mantém compatibilidade total com APIs existentes
- Não quebra mudanças para consumidores do método
- Funciona com todos os tipos `T` que são classes

## Arquitetura Final
- **`Sufficit.Base`**: Contém apenas `FromJsonLog<T>` (conversão string → typed)
- **`Sufficit.EFData`**: Mantém `ToJsonLog<T>` e `FromJsonLog` (conversões bidirecionais para EF)
- **`Sufficit.Client`**: Usa a extensão do Base para conversões necessárias

## Testes
- Projeto compila com sucesso em todas as plataformas alvo (.NET 7.0, 8.0, 9.0, netstandard2.0)
- Lógica de conversão validada através da compilação e dependências corretas