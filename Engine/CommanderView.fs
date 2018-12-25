namespace Engine

open System.Threading

type CommanderView(id: CommanderViewId) =
    member this.Test (text: string) = 
        Thread.Sleep(4000);
        "F#" + text + id.ToString()

