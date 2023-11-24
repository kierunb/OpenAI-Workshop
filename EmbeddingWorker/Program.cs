using Microsoft.KernelMemory;

Console.WriteLine("Hello, KernelMemory !");


string path = @"d:\111millenium\Docs\Regulamin_ogolny_od_25_04_2022-1683887657060.pdf";

var memory = new KernelMemoryBuilder()
    .WithAzureOpenAIEmbeddingGeneration(new AzureOpenAIConfig 
            {
                APIKey = "",
                Endpoint = "",
                Deployment = ""
            })   
    .WithAzureCognitiveSearch("", "key")
    //.WithAzureFormRecognizer // TODO
    .Build<MemoryServerless>();



await memory.ImportDocumentAsync(path, index: "regulamin-1");

var answer = await memory.AskAsync("W jakiej sytuacji odnowiona zostanie lokata? ");

Console.WriteLine("Done!");
