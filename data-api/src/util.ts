import { Readable } from "stream";

export async function readableToObjectList(readable: Readable): Promise<object[]> {
  let result: object[] = [];
  for await (const chunk of readable) {
    result.push(chunk)
  }
  return result;
}

export const parseDate = (date: string | undefined): string | undefined => {
  if (!date) {
    return undefined;
  }

  return new Date(date).toLocaleDateString('nl-NL');
}

export const getLastPathSegment = (url: string): string => {
  return url.substring(url.lastIndexOf('/') + 1);
}
