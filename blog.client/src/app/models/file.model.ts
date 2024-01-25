export class FileModelResponse {
    id: number = 0;
    name: string = "";
    size: number = 0;
    contentType: string = "";
    postId: number = 0;

    constructor(init?: Partial<FileModelResponse>) {
        Object.assign(this, init);
    }
}