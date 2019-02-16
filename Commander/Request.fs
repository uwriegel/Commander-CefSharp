namespace Commander 

open System.Threading

type Request(requestFactory: RequestFactory, number) = 
    member this.IsCancelled = number < requestFactory.Number

and RequestFactory() = 
    let mutable number = 0

    member this.Number with get () = number

    member this.create () = 
        Request(this, Interlocked.Increment(ref number)) 

