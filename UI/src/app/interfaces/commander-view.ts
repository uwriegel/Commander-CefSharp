import { Columns } from "../model/model"

export enum ProcessItemType
{
    Show,
    Properties,
    StartAs
}

export interface IProcessor {
    ready(): any
    changePath(path: string): any
    getItems(): string
    processItem(processItemType: ProcessItemType): any
    setIndex(index: number): any
    sort(index: number, ascending: boolean): any
    createFolder(item: string): any
    copy(targetPath: string): any
    setSelected(items: number[]): any
    getTestItems(): string[]
}

export interface ICommanderView {
    setColumns(columns: Columns): any
    itemsChanged(): any
    setCurrentItem(item: string): any
    createFolder(text: string): any
    copy(targetPath: string, text: string): any
}
