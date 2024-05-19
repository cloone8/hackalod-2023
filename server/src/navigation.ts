async function resolveTopic(topic: Topic): Promise<LoadedTopic> {
  if (asLoadedTopic(topic)) return topic
  return getTopic(topic)
}

async function getTopic(uri: URI): Promise<LoadedTopic> {
  const topic = await resolve(uri)

  const config = getTopicConfig(uri)

  const result: Topic = {
    name: topic.name,
    description: topic.description,
    uri: uri,
    next: topic.links
      .filter(([link, _]) => link in config.topics),
    images: topic.links
      .filter(([link, _]) => link in config.images)
      .map(([link, image]) => [link, { url : image }]),
  }

  return result
}

async function getNext(uri: URI): Promise<Topic> {
  const topic: LoadedTopic = await getTopic(uri)

  const resolved: [URI, LoadedTopic][] = []

  for (const [link, uri] of topic.next) {
    const next = await resolveTopic(uri)
    resolved.push([link, next])
  }

  topic.next = resolved
  return topic
}

