
export const delay = async (timeout: number) => {
    return new Promise((res, rej) => {
        setTimeout(() => res(), timeout)
    })
}