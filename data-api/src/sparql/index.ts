import { readableToObjectList } from "../util";
import { artworksClient } from "./client";
import { buildQuery } from "./query";

export const entityTypes = ['artist', 'city', 'movement'];

export default async (entityType: string, entityId: string) => {
  const { buildSubqueries, mapData } = require(`./queries/${entityType}`);

  const qs = buildSubqueries(entityId)
    .map((q: any) => buildQuery(q));

  console.log(qs[1]);

  const responses = await Promise.all(
    qs.map((q: string) => artworksClient.query.select(q).then(readableToObjectList))
  );

  return mapData(responses);
}
