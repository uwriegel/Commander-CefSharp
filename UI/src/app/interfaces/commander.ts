import { stripGeneratedFileSuffix } from "@angular/compiler/src/aot/util";

export interface ICommander {
    setViewer(on: boolean)
}

export interface IProgram {
    setStatusRatio(ratio: number): any
    setViewerRatio(ratio: number) : any
    setFile(file: string): any
}