namespace Engine

open System.Threading

type CommanderView(id: CommanderViewId) =
    member this.ready() = ()