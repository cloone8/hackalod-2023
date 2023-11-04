import readableToObjectList from "../util/readableToObjectList";
import { artworksClient } from "./client";
import { buildQuery } from "./query";

export default async (entityType: string, entityId: string) => {
  const { buildSubquery, mapData } = require(`./queries/${entityType}`);

  const q = buildQuery(buildSubquery(entityId));

  console.log(q);

  return artworksClient.query.select(q)
    .then(readableToObjectList)
    .then(mapData);
}
