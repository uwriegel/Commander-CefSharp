export const repeatKey = (repeated: boolean, process: () => void) => {
    let isLooping = false
    let lastTime: Date
    const scrollDelay = 5

    let processLoop = () => {
        lastTime = new Date()
        if (isLooping) {
            process()
            var timeNow = new Date()
            var diffTime = timeNow.getMilliseconds() - lastTime.getMilliseconds()
            var delay = diffTime < scrollDelay ? scrollDelay - diffTime : scrollDelay
            setTimeout(() => processLoop(), delay)
        }
    }

    if (!repeated) 
        process()
    else {
        let onkeyDown = function (evt: KeyboardEvent) {
            evt.stopPropagation()
            evt.preventDefault()
        }

        let onkeyUp = function () {
            isLooping = false
            document.removeEventListener("keydown", onkeyDown, true)
            document.removeEventListener("keyup", onkeyUp, true)
        }

        document.addEventListener("keydown", onkeyDown, true)
        document.addEventListener("keyup", onkeyUp, true)
        isLooping = true
        setTimeout(() => processLoop())
    }
}