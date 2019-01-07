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
}

export interface ICommanderView {
    setColumns(columns: Columns): any
    itemsChanged(): any
    setCurrentItem(item: string): any
}
