// Learn more about F# at http://fsharp.org

open FSharp.Control
open FSharp.CosmosDb
open System.IO

type Diabetes = {
    id: string
    Age: string
    Gender: string
    Plyuria: string
    Polydipsia: string
    SuddenWeightLoss: string
    Weakness: string
    Polyphagia: string
    GenitalThrush: string
    VisualBlurring: string
    Itching: string
    Irritability: string
    DelayedHealing: string
    PartialParesis: string
    MuscleStiffness: string
    Alopecia: string
    Obesity: string
    Class: string
}

let insertData connectionString data =
    connectionString 
        |> Cosmos.fromConnectionString 
        |> Cosmos.database "diabetes" 
        |> Cosmos.container "diabetes"
        |> Cosmos.insertMany data
        |> Cosmos.execAsync

let readData filePath = 
    let data = File.ReadAllLines filePath

    data |> Array.skip 1 |> Array.toList

[<EntryPoint>]
let main argv =
    let data = readData "./diabetes_data.csv"

    let connection = "AccountEndpoint=https://mlnetdb.documents.azure.com:443/;AccountKey=KJGhLmEsTyQ9RKMTpzxHKietydA3dyfO2HIVEqrvGHTaUUidox2vPXtM2U6lB2K8AMjQ8yKtfkctRPki6QF7xA==;";

    let newData = data |> List.mapi (fun i d ->
            let row = d.Split(',')

            { 
                id = i.ToString()
                Age = row.[0] 
                Gender = row.[1]
                Plyuria = row.[2]
                Polydipsia = row.[3]
                SuddenWeightLoss = row.[4]
                Weakness = row.[5]
                Polyphagia = row.[6]
                GenitalThrush = row.[7]
                VisualBlurring = row.[8]
                Itching = row.[9]
                Irritability = row.[10]
                DelayedHealing = row.[11]
                PartialParesis = row.[12]
                MuscleStiffness = row.[13]
                Alopecia = row.[14]
                Obesity = row.[15]
                Class = row.[16]
        })

    async {
        let insert = insertData connection newData
        do! insert |> AsyncSeq.iter (fun i -> printfn "Inserted %A" i)
    } |> Async.RunSynchronously

    0 // return an integer exit code
