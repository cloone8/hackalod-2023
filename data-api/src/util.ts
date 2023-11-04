import { Readable } from "stream";

export async function readableToObjectList(readable: Readable): Promise<object[]> {
  let result: object[] = [];
  for await (const chunk of readable) {
    result.push(chunk)
  }
  return result;
}

export const parseDate = (date: string): string => {
  return new Date(date).toLocaleDateString('nl-NL');
}
