using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.KernelMemory;

Console.WriteLine("Hello, KernelMemory !");


string path = @"d:\111millenium\Docs\Regulamin_ogolny_od_25_04_2022-1683887657060.pdf";

var memory = new KernelMemoryBuilder()
    //.WithOpenAIDefaults(GetApiKey())
    .WithAzureOpenAITextGeneration(new AzureOpenAIConfig
    {
        APIKey = "",
        Endpoint = "",
        Deployment = "gpt-4",
        Auth = AzureOpenAIConfig.AuthTypes.APIKey
    })
    .WithAzureOpenAIEmbeddingGeneration(new AzureOpenAIConfig
    {
        APIKey = "",
        Endpoint = "",
        Deployment = "text-embedding-ada-002",
        Auth = AzureOpenAIConfig.AuthTypes.APIKey
    })
    .WithAzureCognitiveSearch("", "")
    //.WithAzureFormRecognizer // TODO
    .Build<MemoryServerless>();



Console.WriteLine("Wait...");
await memory.ImportDocumentAsync(path, index: "<NazwaGrupyInicjały>");

var answer = await memory.AskAsync("Czy mogę zabrać psa na pokład samolotu?", index: "<NazwaGrupyInicjały>");

Console.WriteLine(answer.Result);

Console.WriteLine("Done!");

