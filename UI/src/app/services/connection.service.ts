import { Injectable } from '@angular/core'
import { Subject, Observable, BehaviorSubject } from 'rxjs'
import { Response } from '../model/model'

function formatParams(params) {
    return "?" + Object
        .keys(params)
        .filter(key => params[key])
        .map(key => key+"="+encodeURIComponent(params[key]))
        .join("&")
}

@Injectable({
    providedIn: 'root'
})
export class ConnectionService {
    // get serverEvents(): Observable<CommanderUpdate>  {
    //     return this.serverEventsSubject
    // }

    get ready(): Observable<boolean>  {
        return this.readySubject
    }

    constructor() {
        this.source.onopen = () => this.readySubject.next(true)

        // this.source.addEventListener("updates", (evtString: MessageEvent) => {
        //     const evt = JSON.parse(evtString.data) as CommanderUpdate
        //     this.serverEventsSubject.next(evt)
        // })
    }

    // get(callerId: string, path: string, columnsName?: string, basePath = "") {
    //     const requestId = ++seed;
    //     const get: Get = {
    //         requestId: requestId,
    //         callerId: callerId,
    //         columnsName: columnsName,
    //         path: path,
    //         basePath: basePath
    //     }
    //     return this.post<Response>("get", formatParams(get))
    // }

    // process(commanderView: CommanderView, index: number) {
    //     const process: Process = {
    //         index: index || 0,
    //         commanderView: commanderView
    //     } 
    //     return this.post<Response>("process", formatParams(process))
    // }

    private post<T>(method: string, param = "") {
        return new Promise<T>((res, rej) => {
            const request = new XMLHttpRequest()
            request.open('POST', `${this.baseUrl}/request/${method}${param}`, true)
            request.setRequestHeader('Content-Type', 'application/json; charset=utf-8')
            request.onload = evt => {
                if (request.status == 200) {
                    var result = <T>JSON.parse(request.responseText)
                    res(result)
                }
                else
                    rej(request)
            }
            request.send()
        })
    }

    private readonly source = new EventSource("events")
    private readonly baseUrl = "http://localhost:20000"
    private readonly readySubject = new BehaviorSubject<boolean>(false)
}

var seed = 0