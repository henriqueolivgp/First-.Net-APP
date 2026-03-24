// 1. Cria o "builder" da aplicação web. Isto serve para configurar a aplicação antes de a iniciar,
// como por exemplo injetar dependências (serviços) e configurar o comportamento do servidor.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// 2. Adiciona os serviços necessários para a geração do OpenAPI.
// O OpenAPI permite criar documentação interativa automática para a API (muito conhecido como "Swagger").
builder.Services.AddOpenApi();

// 3. Constrói a aplicação web com base na configuração definida acima. 
// A partir daqui, vamos configurar o "pipeline" de pedidos HTTP (o que acontece quando um pedido chega).
var app = builder.Build();

// Configure the HTTP request pipeline.
// 4. Verifica se a aplicação está a correr no ambiente de Desenvolvimento ("Development").
if (app.Environment.IsDevelopment())
{
    // Se for desenvolvimento, ativa o endpoint (URL) que devolve a documentação OpenAPI gerada.
    app.MapOpenApi();
}

// 5. Força o redirecionamento de todas as chamadas feitas por HTTP (inseguro) para HTTPS (seguro).
app.UseHttpsRedirection();

// 6. Cria um array (lista) fixo de strings contendo diferentes estados/descrições do tempo meteorológico.
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// 7. Define um novo "endpoint" (rota) do tipo GET no endereço HTTP "/weatherforecast". 
// Quando um cliente (ex: o teu Blazor) acede a este endereço, a função seguinte é executada.
app.MapGet("/weatherforecast", () =>
{
    // 8. Cria um leque numérico de 1 a 5, para gerar dados referentes aos próximos 5 dias.
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        // 9. Para cada dia, cria um novo objeto do tipo 'WeatherForecast' com dados aleatórios.
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)), // Calcula a data: Data atual + índice (1 a 5)
            Random.Shared.Next(-20, 55),                        // Define uma temperatura aleatória entre -20ºC e 55ºC
            summaries[Random.Shared.Next(summaries.Length)]     // Escolhe uma descrição aleatória (estado do tempo) da lista 'summaries' criada na linha 20
        ))
        .ToArray();
        
    // 10. Devolve os dados. A API trata de converter esta lista C# num formato JSON, que é enviado de volta ao cliente.
    return forecast;
})
// 11. Dá um nome a este endpoint. É útil para podermos criar referências no código para este URL e para a documentação do OpenAPI.
.WithName("GetWeatherForecast");

// 12. Inicia efetivamente a aplicação. O servidor fica bloqueado e começa a escutar os pedidos a partir de agora.
app.Run();

// 13. Declaração do modelo (estrutura de dados) da previsão meteorológica usando um "record".
// O 'record' é um tipo especial introduzido no C# perfeito para representar dados imutáveis de forma concisa.
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    // 14. Propriedade calculada ("Get-only"): Quando esta propriedade é acedida, 
    // pega no valor em Celsius e converte o mesmo para Fahrenheit automaticamente.
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
