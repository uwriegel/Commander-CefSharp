namespace Commander

type MouseWheelZoomControl(onMouseWheel: double->unit) = 
    member this.OnMouseWheel (delta: double) = onMouseWheel delta 
