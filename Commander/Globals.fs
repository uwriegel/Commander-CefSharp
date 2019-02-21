module Globals

let mutable private commanderUrl = "serve://commander/"
let mutable isAngularServing = false
let getCommanderUrl () = commanderUrl
let mutable showHidden = false

let initialize (cmdLine: string[]) = 
    if cmdLine.Length > 0 && cmdLine.[0] = "-serve" then isAngularServing <- true
    if isAngularServing then commanderUrl <- "http://localhost:4200/"
