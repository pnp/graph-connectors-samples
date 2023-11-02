import { ReadableStream } from "stream/web";

export async function streamToJson(stream: ReadableStream<any>): Promise<any> {
    const { value } = await stream.getReader().read();
    const str = new TextDecoder().decode(value);
    return JSON.parse(str);
}