import { Readable } from "stream";

export default async function readableToObjectList(readable: Readable): Promise<object[]> {
  let result: object[] = [];
  for await (const chunk of readable) {
    result.push(chunk)
  }
  return result;
}
