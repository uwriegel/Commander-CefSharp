namespace Commander

open System


module Control =
    let deferredExecution<'a> (action: unit->'a) delayInMilliseconds (dispatcher: System.Windows.Forms.Control) = async {
        do! Async.Sleep delayInMilliseconds
        return dispatcher.Invoke(Func<'a>(action)) :?> 'a
    }
       

