export class BaseResponse<T> {
    success: boolean = false;
    message: string | undefined = undefined;
    data: T | undefined = undefined;
}

export class PagedResponse<T> {
    pageNumber: number = 0;
    pageSize: number = 0;
    totalPagesCount: number = 0;
    totalItemsCount: number = 0;
    items: T[] = [];
}