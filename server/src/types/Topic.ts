type URI = string

type Image = {
  url: URI
}

type LoadedTopic = {
  name: string
  description: string
  uri: URI
  images: [URI, Image][]
  next: [URI, Topic][]
}

type Topic = URI | LoadedTopic

function isLoaded(topic: Topic): boolean {
  return typeof topic !== 'string'
}

function asLoadedTopic(topic: Topic): topic is LoadedTopic {
  return isLoaded(topic)
}

function asURL(topic: Topic): topic is URI {
  return !isLoaded(topic)
}
