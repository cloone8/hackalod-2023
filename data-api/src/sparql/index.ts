import readableToObjectList from "../util/readableToObjectList";
import { artworksClient } from "./client";
import { buildQuery } from "./query";

export default async (entityType: string, entityId: string) => {
  const { buildSubquery } = require(`./queries/${entityType}`);

  const q = buildQuery(buildSubquery(entityId));

  return artworksClient.query.select(q)
    .then(readableToObjectList)
    .then(mapPaintingDataQuery);
}

const mapPaintingDataQuery = (data: any[]) => data.map((d: any) => ({
  name: d.name.value,
  description: d.description.value,
  contentUrl: d.contentUrl.value
}))

