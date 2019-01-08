import { stripGeneratedFileSuffix } from "@angular/compiler/src/aot/util";

export interface ICommander {
    setViewer(on: boolean)
}

export interface IViewer {
    setStatusRatio(ratio: number): any
    setViewerRatio(ratio: number) : any
    setFile(file: string): any
}

export interface IControl {
    onFocus(id: string): any
}
